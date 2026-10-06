using Pentagoland.ServerDoctor;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader;

var assertions = new Assertions();
RunYamlEditorTests(assertions);
RunPathGuardTests(assertions);
RunServerRootLocatorTests(assertions);
RunRootCatalogTests(assertions);
RunFileWorkflowTests(assertions);
RunEditorAdapterTests(assertions);
RunScpslConfigEditorTests(assertions);
RunScpslGameplayValidatorTests(assertions);
RunRuntimeHealthTests(assertions);
RunRuntimePluginInventoryTests(assertions);
RunNonInterferenceTests(assertions);
Console.WriteLine($"Passed {assertions.Count} Server Doctor checks.");

static void RunYamlEditorTests(Assertions assertions)
{
    const string original = "feature:\r\n  enabled: false # keep this comment\r\n  title: old\r\n";
    assertions.True(
        YamlScalarEditor.TrySetScalar(original, "feature.enabled", "true", out string updated, out _),
        "nested YAML scalar can be changed");
    assertions.Contains("enabled: true # keep this comment", updated, "inline comment is preserved");
    assertions.Contains("title: old", updated, "sibling setting is preserved");
    assertions.True(updated.Contains("\r\n", StringComparison.Ordinal), "CRLF is preserved");

    assertions.True(
        YamlScalarEditor.TrySetScalar("message: old\n", "message", "text # value", out string quoted, out _),
        "unsafe plain text is quoted");
    assertions.Contains("message: \"text # value\"", quoted, "comment syntax in value cannot escape its YAML scalar");

    const string duplicate = "first:\n  enabled: true\nsecond:\n  enabled: false\n";
    assertions.False(
        YamlScalarEditor.TrySetScalar(duplicate, "enabled", "false", out string unchanged, out _),
        "ambiguous duplicate keys are rejected");
    assertions.Equal(duplicate, unchanged, "ambiguous edit leaves the original unchanged");
    assertions.False(
        YamlScalarEditor.TrySetScalar("values: [a, b]\n", "values", "c", out _, out _),
        "YAML collection values are not edited");
    const string sequence = "items:\n  - name: sample\n    enabled: false\n";
    assertions.False(
        YamlScalarEditor.TrySetScalar(sequence, "items.enabled", "true", out string sequenceUnchanged, out _),
        "scalar-like properties nested inside YAML sequences are not edited");
    assertions.Equal(sequence, sequenceUnchanged, "a sequence edit attempt leaves the full document unchanged");
}

static void RunPathGuardTests(Assertions assertions)
{
    string rootPath = Path.Combine(Path.GetTempPath(), "server doctor path tests " + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(rootPath);
    try
    {
        var root = new AccessRoot("server", rootPath);
        assertions.Equal(
            Path.Combine(rootPath, "config.yml"),
            SafePath.ResolveFile(root, "config.yml", mustExist: false),
            "relative file resolves below the allowed root");
        assertions.Throws<ArgumentException>(
            () => SafePath.ResolveFile(root, "..\\outside.yml", mustExist: false),
            "parent traversal is rejected");
        assertions.Throws<ArgumentException>(
            () => SafePath.ResolveFile(root, Path.Combine(rootPath, "outside.yml"), mustExist: false),
            "absolute paths are rejected");
    }
    finally
    {
        Directory.Delete(rootPath, recursive: true);
    }
}

static void RunServerRootLocatorTests(Assertions assertions)
{
    string root = Path.Combine(Path.GetTempPath(), "server doctor install " + Guid.NewGuid().ToString("N"));
    string managed = Path.Combine(root, "SCPSL_Data", "Managed");
    Directory.CreateDirectory(managed);
    try
    {
        assertions.Equal(
            Path.GetFullPath(root),
            ServerRootLocator.Locate(managed, Path.Combine(root, "SCPSL_Data"), string.Empty),
            "server installation is found from a Unity Managed directory");
        assertions.Equal(
            Path.GetFullPath(Path.Combine(root, "SCPSL_Data")),
            ServerRootLocator.Locate(root, managed, "SCPSL_Data"),
            "configured relative server root is honored");
    }
    finally
    {
        Directory.Delete(root, recursive: true);
    }
}

static void RunRootCatalogTests(Assertions assertions)
{
    string root = Path.Combine(Path.GetTempPath(), "server doctor roots " + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(root);
    try
    {
        var config = new ServerDoctorConfig
        {
            AdditionalRoots =
            [
                new AdditionalRoot { Name = "custom", Path = "Plugins/Custom" },
                new AdditionalRoot { Name = "server", Path = "another" },
                new AdditionalRoot { Name = "invalid name", Path = "elsewhere" }
            ]
        };
        RootCatalog catalog = RootCatalog.Discover(root, config);
        assertions.True(catalog.TryGet("custom", out AccessRoot custom), "custom roots can be registered");
        assertions.Equal(Path.GetFullPath(Path.Combine(root, "Plugins", "Custom")), custom.Path,
            "relative custom roots resolve under the server root");
        assertions.Equal(2, catalog.Warnings.Count, "duplicate and invalid root aliases are reported");

        string apiAppData = Path.Combine(root, "managed-appdata");
        string secretLabRoot = Path.Combine(apiAppData, "SCP Secret Laboratory");
        string labApiRoot = Path.Combine(secretLabRoot, "LabAPI");
        string pluginRoot = Path.Combine(labApiRoot, "plugins");
        string dependencyRoot = Path.Combine(labApiRoot, "dependencies");
        string configuredPluginPath = Path.Combine(pluginRoot, "custom", "7777");
        string configuredDependencyPath = Path.Combine(dependencyRoot, "extra");
        Directory.CreateDirectory(configuredPluginPath);
        Directory.CreateDirectory(configuredDependencyPath);
        File.WriteAllText(Path.Combine(configuredPluginPath, "CustomPlugin.dll"), "plugin");
        File.WriteAllText(Path.Combine(configuredDependencyPath, "CustomDependency.dll"), "dependency");

        var labApiPaths = new LabApiPathSettings(
            apiAppData,
            secretLabRoot,
            Path.Combine(secretLabRoot, "config"),
            labApiRoot,
            Path.Combine(labApiRoot, "configs"),
            pluginRoot,
            dependencyRoot,
            ["custom/$port"],
            ["extra"],
            "7777",
            usesHostedAppData: false);
        var configuredScan = new ServerDoctorConfig
        {
            PluginScanRoots = [],
            DependencyScanRoots = []
        };
        RootCatalog configuredCatalog = RootCatalog.Discover(root, configuredScan, labApiPaths);
        assertions.True(
            configuredCatalog.TryGet("labapi-plugin-path-1", out AccessRoot configuredPluginRoot),
            "LabAPI custom plugin load paths become inspectable roots");
        assertions.Equal(configuredPluginPath, configuredPluginRoot.Path,
            "the configured $port token resolves to the active server port");
        var configuredFiles = new FileAccessService(root, configuredCatalog, configuredScan);
        assertions.Contains("CustomPlugin.dll", string.Join("\n", configuredFiles.ScanPluginFiles()),
            "plugin inventory includes the active LabAPI custom load path");
        assertions.Contains("CustomDependency.dll", string.Join("\n", configuredFiles.ScanDependencyFiles()),
            "dependency inventory includes the active LabAPI custom load path");

        var escapingPaths = new LabApiPathSettings(
            apiAppData,
            secretLabRoot,
            Path.Combine(secretLabRoot, "config"),
            labApiRoot,
            Path.Combine(labApiRoot, "configs"),
            pluginRoot,
            dependencyRoot,
            ["../outside"],
            [],
            "7777",
            usesHostedAppData: false);
        RootCatalog guardedCatalog = RootCatalog.Discover(root, configuredScan, escapingPaths);
        assertions.False(guardedCatalog.TryGet("labapi-plugin-path-1", out _),
            "LabAPI load paths cannot create inspection roots outside PathManager.Plugins");
        assertions.Contains("sale de la carpeta oficial", string.Join("\n", guardedCatalog.Warnings),
            "escaped LabAPI load paths are reported explicitly");
    }
    finally
    {
        Directory.Delete(root, recursive: true);
    }
}

static void RunFileWorkflowTests(Assertions assertions)
{
    string serverRoot = Path.Combine(Path.GetTempPath(), "server doctor workflow " + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(serverRoot);
    string previousDirectory = Environment.CurrentDirectory;
    try
    {
        Environment.CurrentDirectory = serverRoot;
        File.WriteAllText(Path.Combine(serverRoot, "hoster_policy.txt"), "gamedir_for_configs: true");
        string labApiRoot = Path.Combine(serverRoot, "AppData", "SCP Secret Laboratory", "LabAPI");
        string pluginDirectory = Path.Combine(labApiRoot, "plugins", "global");
        string nestedPluginDirectory = Path.Combine(labApiRoot, "plugins", "custom", "nested");
        string dependencyDirectory = Path.Combine(labApiRoot, "dependencies", "global");
        string logsDirectory = Path.Combine(serverRoot, "Logs");
        string extraPluginDirectory = Path.Combine(serverRoot, "ThirdParty", "Plugins", "nested");
        Directory.CreateDirectory(pluginDirectory);
        Directory.CreateDirectory(nestedPluginDirectory);
        Directory.CreateDirectory(dependencyDirectory);
        Directory.CreateDirectory(logsDirectory);
        Directory.CreateDirectory(extraPluginDirectory);
        File.WriteAllText(Path.Combine(pluginDirectory, "ExamplePlugin.dll"), "not loaded");
        File.WriteAllText(Path.Combine(pluginDirectory, "UppercasePlugin.DLL"), "not loaded");
        File.WriteAllText(Path.Combine(nestedPluginDirectory, "NestedPlugin.dll"), "not loaded");
        File.WriteAllText(Path.Combine(extraPluginDirectory, "ExtraPlugin.dll"), "not loaded");
        File.WriteAllText(Path.Combine(dependencyDirectory, "ExampleDependency.dll"), "not loaded");
        File.WriteAllText(Path.Combine(logsDirectory, "server.log"),
            "private_key: | # generated key\n  tail-confidential-line\n  another-line\nround started\n");
        string configPath = Path.Combine(serverRoot, "settings.yml");
        File.WriteAllText(configPath,
            "feature:\n  enabled: false\napi_token: sample-secret\nendpoint: 203.0.113.7\nprivate_key: | # generated key\n  confidential-line\n  another-confidential-line\n");

        var config = new ServerDoctorConfig
        {
            AllowConfigChanges = true,
            WritableRoots = ["server"],
            PluginScanRoots = ["labapi-plugins", "custom-plugins"],
            AdditionalRoots = [new AdditionalRoot { Name = "custom-plugins", Path = "ThirdParty/Plugins" }]
        };
        RootCatalog roots = RootCatalog.Discover(serverRoot, config);
        var files = new FileAccessService(serverRoot, roots, config);
        assertions.Equal(
            Path.Combine(labApiRoot, "plugins"),
            roots.TryGet("labapi-plugins", out AccessRoot labApiPlugins) ? labApiPlugins.Path : string.Empty,
            "LabAPI hoster-policy path is resolved from the server working directory");
        assertions.Equal(
            Path.Combine(serverRoot, "AppData", "SCP Secret Laboratory"),
            roots.TryGet("scpsl-data", out AccessRoot gameData) ? gameData.Path : string.Empty,
            "game-data root follows the active LabAPI AppData policy");
        assertions.Contains("ExamplePlugin.dll", string.Join("\n", files.ScanPluginFiles()),
            "plugin inventory reports installed DLL names only");
        assertions.Contains("UppercasePlugin.DLL", string.Join("\n", files.ScanPluginFiles()),
            "plugin inventory finds DLL extensions case-insensitively on Linux");
        assertions.Contains("NestedPlugin.dll | labapi-plugins:custom",
            string.Join("\n", files.ScanPluginFiles()),
            "plugin inventory traverses bounded nested LabAPI plugin paths");
        assertions.Contains("ExtraPlugin.dll | custom-plugins:nested",
            string.Join("\n", files.ScanPluginFiles()),
            "configured additional plugin roots can be included in the inventory");
        assertions.Contains("ExampleDependency.dll", string.Join("\n", files.ScanDependencyFiles()),
            "dependency inventory reports dependency DLL names only");
        string pluginDetails = string.Join("\n", files.InspectPluginFile("labapi-plugins", "global/ExamplePlugin.dll"));
        assertions.Contains("SHA-256:", pluginDetails, "plugin inspection includes a reproducible file fingerprint");
        assertions.Contains("no carga ni ejecuta la DLL", pluginDetails,
            "plugin inspection explicitly avoids loading or executing the binary");
        string compatibilitySummary = string.Join("\n", PluginCompatibilityInspector.Analyze(
            typeof(Assertions).Assembly.Location,
            Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)));
        assertions.Contains("Ensamblado .NET:", compatibilitySummary,
            "compatibility analyzer reads assembly identity from PE metadata");
        assertions.Contains("Revisión estática de referencias", compatibilitySummary,
            "compatibility analyzer examines declared references without executing plugin methods");
        assertions.Contains("no certifica compatibilidad ABI/API", compatibilitySummary,
            "compatibility analyzer reports the limits of static analysis");
        string unreadableCandidateSummary = string.Join("\n", PluginCompatibilityInspector.Analyze(
            typeof(Assertions).Assembly.Location,
            [Path.Combine(pluginDirectory, "ExamplePlugin.dll")]));
        assertions.Contains("1 DLL candidatas no se pudieron leer", unreadableCandidateSummary,
            "compatibility analyzer reports unreadable candidate DLLs instead of silently ignoring them");
        assertions.Throws<UnauthorizedAccessException>(
            () => files.InspectPluginFile("labapi-plugins", "global/ExamplePlugin.txt"),
            "plugin inspection refuses non-DLL files");
        assertions.Throws<BadImageFormatException>(
            () => files.AnalyzePluginCompatibility("labapi-plugins", "global/ExamplePlugin.dll"),
            "compatibility analysis reports non-managed or invalid DLLs instead of claiming success");

        string preview = files.Read("server", "settings.yml");
        assertions.False(preview.Contains("sample-secret", StringComparison.Ordinal), "config preview redacts secret-like values");
        assertions.False(preview.Contains("203.0.113.7", StringComparison.Ordinal), "config preview redacts IPv4 addresses");
        assertions.False(preview.Contains("confidential-line", StringComparison.Ordinal), "multiline secret blocks are redacted");
        string logTail = string.Join("\n", files.Tail("logs", "server.log", 10));
        assertions.False(logTail.Contains("tail-confidential-line", StringComparison.Ordinal),
            "log tails redact continuation lines from YAML block secrets");
        assertions.Contains("round started", logTail, "log tail preserves ordinary lines after a redacted block");
        string reportResult = files.CreateSupportReport("test-admin", ["Health snapshot: test"]);
        string reportPath = reportResult.Substring(
            "Informe local creado: ".Length,
            reportResult.IndexOf(". Revísalo", StringComparison.Ordinal) - "Informe local creado: ".Length);
        string reportContents = File.ReadAllText(reportPath);
        assertions.Contains("Health snapshot: test", reportContents, "support report includes the supplied runtime snapshot");
        assertions.Contains("ExamplePlugin.dll", reportContents, "support report includes plugin inventory");
        assertions.False(reportContents.Contains("tail-confidential-line", StringComparison.Ordinal),
            "support report redacts secrets in attached logs");

        assertions.Throws<InvalidOperationException>(
            () => files.SetConfigValue("server", "settings.yml", "feature.enabled", "true", "test-admin", confirmed: false),
            "YAML changes require an explicit confirmation");
        string editResult = files.SetConfigValue("server", "settings.yml", "feature.enabled", "true", "test-admin", confirmed: true);
        assertions.Contains("Copia previa:", editResult, "YAML edit creates an automatic backup");
        assertions.Contains("enabled: true", File.ReadAllText(configPath), "allowed YAML scalar is updated");
        assertions.Contains("api_token: sample-secret", File.ReadAllText(configPath), "unrelated secret value stays on disk unchanged");

        string backupId = editResult.Split("Copia previa: ")[1].Split('.')[0];
        assertions.Throws<InvalidOperationException>(
            () => files.RestoreBackup(backupId, "test-admin", confirmed: false),
            "restores require an explicit confirmation");
        string restoreResult = files.RestoreBackup(backupId, "test-admin", confirmed: true);
        assertions.Contains("Estado anterior guardado como:", restoreResult, "restore creates a rollback copy");
        assertions.Contains("enabled: false", File.ReadAllText(configPath), "restore returns the original file");
        assertions.True(files.ListBackups().Count >= 2, "both source and rollback backups are inventoried");

        string oversizedBackupId = files.CreateBackup("server", "settings.yml", "test-admin");
        File.WriteAllBytes(
            Path.Combine(serverRoot, "PentagolandServerDoctor", "backups", oversizedBackupId + ".bin"),
            new byte[1048577]);
        assertions.Throws<IOException>(
            () => files.RestoreBackup(oversizedBackupId, "test-admin", confirmed: true),
            "oversized backup is rejected before hashing");

        File.WriteAllText(
            Path.Combine(serverRoot, "PentagolandServerDoctor", "backups", new string('a', 32) + ".meta"),
            "not-base64\nnot-a-hash");
        assertions.Contains("[manifiesto inválido]", string.Join("\n", files.ListBackups()),
            "a damaged backup manifest does not hide the rest of the backup inventory");

        var readOnlyConfig = new ServerDoctorConfig();
        var readOnlyFiles = new FileAccessService(serverRoot, roots, readOnlyConfig);
        string independentBackup = readOnlyFiles.CreateBackup("server", "settings.yml", "test-admin");
        assertions.True(independentBackup.Length == 32, "manual backups remain available while edits are disabled");
        assertions.Throws<UnauthorizedAccessException>(
            () => readOnlyFiles.SetConfigValue("server", "settings.yml", "feature.enabled", "true", "test-admin", confirmed: true),
            "writes stay disabled by default");
    }
    finally
    {
        Environment.CurrentDirectory = previousDirectory;
        Directory.Delete(serverRoot, recursive: true);
    }
}

static void RunEditorAdapterTests(Assertions assertions)
{
    string serverRoot = Path.Combine(Path.GetTempPath(), "server doctor adapter " + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(serverRoot);
    try
    {
        File.WriteAllText(Path.Combine(serverRoot, "plugin.cfg"), "old");
        var config = new ServerDoctorConfig { AllowConfigChanges = true, WritableRoots = ["server"] };
        RootCatalog roots = RootCatalog.Discover(serverRoot, config);
        var files = new FileAccessService(serverRoot, roots, config, [new TestConfigEditorAdapter()]);
        files.SetConfigValue("server", "plugin.cfg", "setting", "new", "test-admin", confirmed: true);
        assertions.Equal("setting=new", File.ReadAllText(Path.Combine(serverRoot, "plugin.cfg")),
            "additional config formats can be supplied through an editor adapter");
    }
    finally
    {
        Directory.Delete(serverRoot, recursive: true);
    }
}

static void RunScpslConfigEditorTests(Assertions assertions)
{
    string serverRoot = Path.Combine(Path.GetTempPath(), "server doctor gameplay config " + Guid.NewGuid().ToString("N"));
    string previousDirectory = Environment.CurrentDirectory;
    Directory.CreateDirectory(serverRoot);
    try
    {
        Environment.CurrentDirectory = serverRoot;
        File.WriteAllText(Path.Combine(serverRoot, "hoster_policy.txt"), "gamedir_for_configs: true");
        string configDirectory = Path.Combine(serverRoot, "AppData", "SCP Secret Laboratory", "config", "7777");
        Directory.CreateDirectory(configDirectory);
        string configPath = Path.Combine(configDirectory, "config_gameplay.txt");
        File.WriteAllText(configPath, "friendly_fire: false # keep this note\nserver_name: Pentagoland\n");

        var config = new ServerDoctorConfig
        {
            AllowConfigChanges = true,
            WritableRoots = ["scpsl-config"]
        };
        RootCatalog roots = RootCatalog.Discover(serverRoot, config);
        var files = new FileAccessService(serverRoot, roots, config, [new ScpslTextConfigEditor()]);
        assertions.Equal(
            Path.Combine(serverRoot, "AppData", "SCP Secret Laboratory", "config"),
            roots.TryGet("scpsl-config", out AccessRoot scpslConfig) ? scpslConfig.Path : string.Empty,
            "SCP:SL server configuration root follows LabAPI hoster policy");

        string result = files.SetConfigValue(
            "scpsl-config",
            "7777/config_gameplay.txt",
            "friendly_fire",
            "true",
            "test-admin",
            confirmed: true);
        assertions.Contains("SCP:SL config scalar", result, "supported SCP:SL text setting is edited by the scoped adapter");
        assertions.Contains("friendly_fire: true # keep this note", File.ReadAllText(configPath),
            "game config edit preserves inline comments");
        assertions.Throws<InvalidOperationException>(
            () => files.SetConfigValue(
                "scpsl-config",
                "7777/config_gameplay.txt",
                "friendly_fire",
                "false",
                "test-admin",
                confirmed: false),
            "game config writes cannot skip explicit confirmation");

        var yamlOnly = new FileAccessService(serverRoot, roots, config, [new YamlScalarConfigEditor()]);
        assertions.Throws<UnauthorizedAccessException>(
            () => yamlOnly.SetConfigValue(
                "scpsl-config",
                "7777/config_gameplay.txt",
                "friendly_fire",
                "false",
                "test-admin",
                confirmed: true),
            "SCP:SL text adapter cannot edit outside the explicitly registered adapter set");
        assertions.False(
            new ScpslTextConfigEditor().Supports("scpsl-config", "7777/config_remoteadmin.txt", ".txt"),
            "SCP:SL gameplay adapter refuses permission and Remote Admin files");
    }
    finally
    {
        Environment.CurrentDirectory = previousDirectory;
        Directory.Delete(serverRoot, recursive: true);
    }
}

static void RunRuntimeHealthTests(Assertions assertions)
{
    Server.Port = 7788;
    Server.PlayerCount = 7;
    Server.MaxPlayers = 24;
    Server.Tps = 59.5;
    Server.MaxTps = 60;
    string status = string.Join("\n", RuntimeHealth.Snapshot());
    assertions.Contains("Puerto: 7788", status, "runtime health shows live server port");
    assertions.Contains("Jugadores: 7/24", status, "runtime health shows online and maximum players");
    assertions.Contains("TPS: 59.5/60", status, "runtime health shows measured and configured TPS");
    assertions.False(status.Contains("Memoria residente: 0 B", StringComparison.Ordinal),
        "runtime health does not report an unavailable resident-memory metric as zero");
    assertions.False(status.Contains("Memoria privada: 0 B", StringComparison.Ordinal),
        "runtime health does not report an unavailable private-memory metric as zero");

    Server.Tps = double.NaN;
    assertions.Contains("TPS: no disponible/60", string.Join("\n", RuntimeHealth.Snapshot()),
        "runtime health handles invalid TPS samples without reporting infinity or NaN");

    Server.Port = 7777;
    Server.PlayerCount = 4;
    Server.MaxPlayers = 20;
    Server.Tps = 60;
    Server.MaxTps = 60;
}

static void RunRuntimePluginInventoryTests(Assertions assertions)
{
    bool previousInitialized = PluginLoader.Initialized;
    PluginLoader.Plugins.Clear();
    PluginLoader.EnabledPlugins.Clear();
    PluginLoader.Dependencies.Clear();
    try
    {
        PluginLoader.Initialized = true;
        var enabledPlugin = new TestLabApiPlugin { FilePath = Path.Combine("plugins", "Enabled.dll") };
        var disabledPlugin = new TestLabApiPlugin { FilePath = Path.Combine("plugins", "Disabled.dll") };
        PluginLoader.Plugins.Add(enabledPlugin, typeof(Assertions).Assembly);
        PluginLoader.Plugins.Add(disabledPlugin, typeof(string).Assembly);
        PluginLoader.EnabledPlugins.Add(enabledPlugin);
        PluginLoader.Dependencies.Add(typeof(int).Assembly);

        string inventory = string.Join("\n", RuntimePluginInventory.Snapshot());
        assertions.Contains("[HABILITADO]", inventory,
            "runtime inventory distinguishes enabled plugins using LabAPI state");
        assertions.Contains("[NO HABILITADO]", inventory,
            "runtime inventory distinguishes loaded plugins that were not enabled");
        assertions.Contains("Enabled.dll", inventory,
            "runtime inventory reports the plugin file name without exposing its full path");
        assertions.Contains("[DEPENDENCIA]", inventory,
            "runtime inventory includes dependencies registered by LabAPI");
    }
    finally
    {
        PluginLoader.Initialized = previousInitialized;
        PluginLoader.Plugins.Clear();
        PluginLoader.EnabledPlugins.Clear();
        PluginLoader.Dependencies.Clear();
    }
}

static void RunScpslGameplayValidatorTests(Assertions assertions)
{
    string valid = "server_ip: auto\nipv4_bind_ip: 0.0.0.0\nipv6_bind_ip: ::\n";
    string validReport = string.Join("\n", ScpslGameplayConfigValidator.Validate(valid));
    assertions.Contains("coinciden con las comprobaciones básicas", validReport,
        "valid SCP:SL connection settings pass the read-only verifier");

    string invalid = "server_ip: unknown\nipv4_bind_ip: 127.0.0.1\n";
    string invalidReport = string.Join("\n", ScpslGameplayConfigValidator.Validate(invalid));
    assertions.Contains("server_ip: usa 'auto'", invalidReport, "invalid server_ip produces a targeted finding");
    assertions.Contains("ipv4_bind_ip: el valor esperado", invalidReport, "unexpected IPv4 bind produces an advisory");
    assertions.Contains("ipv6_bind_ip: no está definida", invalidReport, "missing settings are not silently treated as valid");
    assertions.Contains("server_ip: usa 'auto' o una dirección IPv4 válida",
        string.Join("\n", ScpslGameplayConfigValidator.Validate(
            "server_ip: ::1\nipv4_bind_ip: 0.0.0.0\nipv6_bind_ip: ::\n")),
        "server_ip validator distinguishes IPv4 from IPv6 literals");

    string duplicateReport = string.Join("\n", ScpslGameplayConfigValidator.Validate(
        "server_ip: auto\nserver_ip: 127.0.0.1\nipv4_bind_ip: 0.0.0.0\nipv6_bind_ip: ::\n"));
    assertions.Contains("server_ip: clave duplicada", duplicateReport, "duplicate network settings are reported");
}

static void RunNonInterferenceTests(Assertions assertions)
{
    string serverRoot = Path.Combine(Path.GetTempPath(), "server doctor passive " + Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(serverRoot);
    try
    {
        object[] handlers = typeof(ServerDoctorCommand)
            .GetCustomAttributes(typeof(CommandHandlerAttribute), inherit: false);
        assertions.Equal(2, handlers.Length,
            "command is registered for Remote Admin and the dedicated server console");

        var plugin = new ServerDoctorPlugin
        {
            Config = new ServerDoctorConfig { ServerRootPath = serverRoot }
        };
        plugin.Enable();
        try
        {
            assertions.True(plugin.IsTransparent,
                "administrative-only plugin reports its non-gameplay status to LabAPI");
            assertions.Equal(
                PathManager.Plugins.FullName,
                plugin.Roots!.TryGet("labapi-plugins", out AccessRoot activePlugins) ? activePlugins.Path : string.Empty,
                "active LabAPI plugin root comes from the official PathManager");
            string activeGameplayConfigRoot = Directory.GetParent(
                Directory.GetParent(PathManager.GameplayConfigPath)!.FullName)!.FullName;
            assertions.Equal(
                activeGameplayConfigRoot,
                plugin.Roots.TryGet("scpsl-config", out AccessRoot gameplayConfigRoot) ? gameplayConfigRoot.Path : string.Empty,
                "SCP:SL config root follows LabAPI's active GameplayConfigPath");
            assertions.True(plugin.Roots.LabApiPluginScanRoots.Count > 0,
                "active LabAPI PluginPaths are added to the inventory roots");
            assertions.False(Directory.Exists(Path.Combine(serverRoot, "PentagolandServerDoctor")),
                "plugin enable creates no data directories");

            PermissionExtensions.IsAllowed = false;
            var command = new ServerDoctorCommand();
            bool result = command.Execute(Array.Empty<string>(), new TestCommandSender(), out string response);
            assertions.False(result, "commands fail closed without the configured permission");
            assertions.Contains("No tienes permiso", response, "permission failure is explicit");
            PermissionExtensions.IsAllowed = true;
            bool wasLoaderInitialized = PluginLoader.Initialized;
            try
            {
                PluginLoader.Initialized = true;
                bool inventoryResult = command.Execute(
                    new ArraySegment<string>(["loaded-plugins"]),
                    new TestCommandSender(),
                    out string inventoryResponse);
                assertions.True(inventoryResult, "loaded-plugin inventory is reachable through the protected command");
                assertions.Contains("Plugins conocidos:", inventoryResponse,
                    "loaded-plugin command returns LabAPI runtime state");

                bool offsetArgumentsResult = command.Execute(
                    new ArraySegment<string>(["ignored", "help", "ignored"], 1, 1),
                    new TestCommandSender(),
                    out string offsetArgumentsResponse);
                assertions.True(offsetArgumentsResult,
                    "command parser respects the offset of an ArraySegment from LabAPI");
                assertions.Contains("Pentagoland Server Doctor", offsetArgumentsResponse,
                    "offset command arguments resolve the requested subcommand");
                }
            finally
            {
                PluginLoader.Initialized = wasLoaderInitialized;
            }
        }
        finally
        {
            PermissionExtensions.IsAllowed = true;
            plugin.Disable();
        }

        var editingPlugin = new ServerDoctorPlugin
        {
            Config = new ServerDoctorConfig { AllowConfigChanges = true }
        };
        assertions.False(editingPlugin.IsTransparent,
            "plugin does not claim transparent status when configuration writes are enabled");
    }
    finally
    {
        Directory.Delete(serverRoot, recursive: true);
    }
}

sealed class TestCommandSender : ICommandSender { }

sealed class TestLabApiPlugin : LabApi.Loader.Features.Plugins.Plugin
{
    public override string Name => "test";
    public override string Description => "test";
    public override string Author => "test";
    public override Version RequiredApiVersion => new(1, 0);
}

sealed class TestConfigEditorAdapter : IConfigEditorAdapter
{
    public string Name => "test";
    public bool Supports(string rootName, string relativePath, string extension) =>
        extension.Equals(".cfg", StringComparison.OrdinalIgnoreCase);
    public bool TrySetValue(string content, string key, string value, out string updatedContent, out string error)
    {
        updatedContent = $"{key}={value}";
        error = string.Empty;
        return true;
    }
}

sealed class Assertions
{
    public int Count { get; private set; }

    public void True(bool value, string description)
    {
        Count++;
        if (!value)
            throw new InvalidOperationException("FAILED: " + description);
    }

    public void False(bool value, string description) => True(!value, description);

    public void Equal<T>(T expected, T actual, string description) =>
        True(EqualityComparer<T>.Default.Equals(expected, actual),
            $"{description} (expected '{expected}', got '{actual}')");

    public void Contains(string expected, string actual, string description) =>
        True(actual.Contains(expected, StringComparison.Ordinal),
            $"{description} (missing '{expected}')");

    public void Throws<TException>(Action action, string description) where TException : Exception
    {
        Count++;
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"FAILED: {description} (expected {typeof(TException).Name})");
    }
}

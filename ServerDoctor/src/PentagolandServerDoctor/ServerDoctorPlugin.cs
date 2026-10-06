using System;
using System.Linq;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;

namespace Pentagoland.ServerDoctor;

public sealed class ServerDoctorPlugin : Plugin<ServerDoctorConfig>
{
    public static ServerDoctorPlugin? Instance { get; private set; }

    public override string Name => "Pentagoland Server Doctor";
    public override string Description => "Read-only diagnostics and guarded server configuration tools.";
    public override string Author => "Pentagoland";
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
    public override bool IsTransparent => !Config.AllowConfigChanges;

    public string ServerRoot { get; private set; } = string.Empty;
    public RootCatalog? Roots { get; private set; }
    public ServerDoctorConfig ActiveConfig { get; private set; } = new();
    public FileAccessService? FileAccess { get; private set; }

    public override void Enable()
    {
        Instance = this;
        ActiveConfig = Config ?? new ServerDoctorConfig();
        ServerRoot = ServerRootLocator.Locate(
            Environment.CurrentDirectory,
            AppDomain.CurrentDomain.BaseDirectory,
            ActiveConfig.ServerRootPath);
        string appDataPath = PathManager.AppData.FullName;
        string hostedAppDataPath = System.IO.Path.GetFullPath(
            System.IO.Path.Combine(Environment.CurrentDirectory, "AppData"));
        bool usesHostedAppData = string.Equals(
            System.IO.Path.GetFullPath(appDataPath).TrimEnd(
                System.IO.Path.DirectorySeparatorChar,
                System.IO.Path.AltDirectorySeparatorChar),
            hostedAppDataPath.TrimEnd(
                System.IO.Path.DirectorySeparatorChar,
                System.IO.Path.AltDirectorySeparatorChar),
            StringComparison.OrdinalIgnoreCase);
        DirectoryInfo? portConfigDirectory = System.IO.Path.GetDirectoryName(PathManager.GameplayConfigPath) is string gameplayDirectory
            ? new DirectoryInfo(gameplayDirectory)
            : null;
        string scpslConfigPath = portConfigDirectory?.Parent?.FullName ??
                                 System.IO.Path.Combine(PathManager.SecretLab.FullName, "config");
        var labApiPaths = new LabApiPathSettings(
            appDataPath,
            PathManager.SecretLab.FullName,
            scpslConfigPath,
            PathManager.LabApi.FullName,
            PathManager.Configs.FullName,
            PathManager.Plugins.FullName,
            PathManager.Dependencies.FullName,
            PluginLoader.Config.PluginPaths,
            PluginLoader.Config.DependencyPaths,
            Server.Port.ToString(System.Globalization.CultureInfo.InvariantCulture),
            usesHostedAppData);
        Roots = RootCatalog.Discover(ServerRoot, ActiveConfig, labApiPaths);
        FileAccess = new FileAccessService(
            ServerRoot,
            Roots,
            ActiveConfig,
            [new YamlScalarConfigEditor(), new ScpslTextConfigEditor()]);
    }

    public override void Disable()
    {
        if (Instance == this)
            Instance = null;

        Roots = null;
        FileAccess = null;
    }
}

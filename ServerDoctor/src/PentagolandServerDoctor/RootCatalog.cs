using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pentagoland.ServerDoctor;

public sealed class AccessRoot
{
    public AccessRoot(string name, string path)
    {
        Name = name;
        Path = System.IO.Path.GetFullPath(path);
    }

    public string Name { get; }
    public string Path { get; }
}

public sealed class RootCatalog
{
    private const int MaximumConfiguredLoaderPaths = 32;
    private static readonly Regex SafeName = new("^[a-z][a-z0-9_-]{0,31}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly Dictionary<string, AccessRoot> _roots;
    private readonly List<string> _warnings;
    private readonly List<string> _labApiPluginScanRoots;
    private readonly List<string> _labApiDependencyScanRoots;

    private RootCatalog(
        Dictionary<string, AccessRoot> roots,
        List<string> warnings,
        List<string> labApiPluginScanRoots,
        List<string> labApiDependencyScanRoots)
    {
        _roots = roots;
        _warnings = warnings;
        _labApiPluginScanRoots = labApiPluginScanRoots;
        _labApiDependencyScanRoots = labApiDependencyScanRoots;
    }

    public IEnumerable<AccessRoot> All => _roots.Values.OrderBy(root => root.Name, StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<string> Warnings => _warnings;
    public IReadOnlyList<string> LabApiPluginScanRoots => _labApiPluginScanRoots;
    public IReadOnlyList<string> LabApiDependencyScanRoots => _labApiDependencyScanRoots;

    public bool TryGet(string name, out AccessRoot root) => _roots.TryGetValue(name, out root!);

    public static RootCatalog Discover(
        string serverRoot,
        ServerDoctorConfig config,
        LabApiPathSettings? labApiPaths = null)
    {
        string fullServerRoot = System.IO.Path.GetFullPath(serverRoot);
        var roots = new Dictionary<string, AccessRoot>(StringComparer.OrdinalIgnoreCase);
        var warnings = new List<string>();
        var labApiPluginScanRoots = new List<string>();
        var labApiDependencyScanRoots = new List<string>();
        Add(roots, "server", fullServerRoot);
        Add(roots, "labapi-server-plugins", System.IO.Path.Combine(fullServerRoot, "LabAPI", "plugins"));
        Add(roots, "labapi-server-dependencies", System.IO.Path.Combine(fullServerRoot, "LabAPI", "dependencies"));
        Add(roots, "exiled-server-plugins", System.IO.Path.Combine(fullServerRoot, "EXILED", "Plugins"));
        Add(roots, "logs", System.IO.Path.Combine(fullServerRoot, "Logs"));
        Add(roots, "configs", System.IO.Path.Combine(fullServerRoot, "Configs"));

        string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appDataPath;
        string secretLabDataRoot;
        string scpslConfigPath;
        string labApiDataRoot;
        string labApiConfigsPath;
        string labApiPluginsPath;
        string labApiDependenciesPath;
        bool usesHostedAppData;
        if (labApiPaths is null)
        {
            string labApiAppData = GetLabApiAppDataDirectory(fullServerRoot, out usesHostedAppData);
            appDataPath = labApiAppData;
            secretLabDataRoot = System.IO.Path.Combine(labApiAppData, "SCP Secret Laboratory");
            scpslConfigPath = System.IO.Path.Combine(secretLabDataRoot, "config");
            labApiDataRoot = System.IO.Path.Combine(secretLabDataRoot, "LabAPI");
            labApiConfigsPath = System.IO.Path.Combine(labApiDataRoot, "configs");
            labApiPluginsPath = System.IO.Path.Combine(labApiDataRoot, "plugins");
            labApiDependenciesPath = System.IO.Path.Combine(labApiDataRoot, "dependencies");
        }
        else
        {
            appDataPath = labApiPaths.AppDataPath;
            secretLabDataRoot = labApiPaths.SecretLabPath;
            scpslConfigPath = labApiPaths.ScpslConfigPath;
            labApiDataRoot = labApiPaths.LabApiPath;
            labApiConfigsPath = labApiPaths.ConfigsPath;
            labApiPluginsPath = labApiPaths.PluginsPath;
            labApiDependenciesPath = labApiPaths.DependenciesPath;
            usesHostedAppData = labApiPaths.UsesHostedAppData;
        }

        if (!string.IsNullOrWhiteSpace(appDataPath))
        {
            Add(roots, "scpsl-data", secretLabDataRoot);
            Add(roots, "scpsl-config", scpslConfigPath);
            Add(roots, "labapi-configs", labApiConfigsPath);
            Add(roots, "labapi-data", labApiDataRoot);
            Add(roots, "labapi-plugins", labApiPluginsPath);
            Add(roots, "labapi-dependencies", labApiDependenciesPath);
            if (labApiPaths is not null)
            {
                AddConfiguredRoots(
                    roots,
                    warnings,
                    labApiPluginScanRoots,
                    labApiPaths.PluginPaths,
                    labApiPaths.PluginsPath,
                    "labapi-plugin-path-",
                    labApiPaths.ServerPort);
                AddConfiguredRoots(
                    roots,
                    warnings,
                    labApiDependencyScanRoots,
                    labApiPaths.DependencyPaths,
                    labApiPaths.DependenciesPath,
                    "labapi-dependency-path-",
                    labApiPaths.ServerPort);
            }

            if (usesHostedAppData && !string.IsNullOrWhiteSpace(userAppData))
            {
                string userSecretLabRoot = System.IO.Path.Combine(userAppData, "SCP Secret Laboratory");
                Add(roots, "scpsl-user-data", userSecretLabRoot);
                Add(roots, "scpsl-user-config", System.IO.Path.Combine(userSecretLabRoot, "config"));
                string userLabApiRoot = System.IO.Path.Combine(userSecretLabRoot, "LabAPI");
                Add(roots, "labapi-user-configs", System.IO.Path.Combine(userLabApiRoot, "configs"));
                Add(roots, "labapi-user-data", userLabApiRoot);
                Add(roots, "labapi-user-plugins", System.IO.Path.Combine(userLabApiRoot, "plugins"));
                Add(roots, "labapi-user-dependencies", System.IO.Path.Combine(userLabApiRoot, "dependencies"));
            }
        }

        if (!string.IsNullOrWhiteSpace(userAppData))
        {
            string exiledDataRoot = System.IO.Path.Combine(userAppData, "EXILED");
            Add(roots, "exiled-plugins", System.IO.Path.Combine(exiledDataRoot, "Plugins"));
            Add(roots, "exiled-configs", System.IO.Path.Combine(exiledDataRoot, "Configs"));
            Add(roots, "exiled-data", exiledDataRoot);
        }

        foreach (AdditionalRoot extra in config.AdditionalRoots ?? [])
        {
            string name = extra.Name ?? string.Empty;
            if (!SafeName.IsMatch(name))
            {
                warnings.Add($"Se ignoró una raíz adicional con nombre inválido: '{name}'.");
                continue;
            }
            if (string.IsNullOrWhiteSpace(extra.Path))
            {
                warnings.Add($"Se ignoró la raíz adicional '{name}': falta Path.");
                continue;
            }
            if (roots.ContainsKey(name))
            {
                warnings.Add($"Se ignoró la raíz adicional '{name}': el alias ya está en uso.");
                continue;
            }

            try
            {
                string path = System.IO.Path.IsPathRooted(extra.Path)
                    ? extra.Path
                    : System.IO.Path.Combine(fullServerRoot, extra.Path);
                Add(roots, name, path);
            }
            catch (Exception exception) when (
                exception is ArgumentException ||
                exception is NotSupportedException ||
                exception is PathTooLongException)
            {
                warnings.Add($"Se ignoró la raíz adicional '{name}': {exception.Message}");
            }
        }

        return new RootCatalog(roots, warnings, labApiPluginScanRoots, labApiDependencyScanRoots);
    }

    private static void AddConfiguredRoots(
        Dictionary<string, AccessRoot> roots,
        List<string> warnings,
        List<string> scanRoots,
        IEnumerable<string> configuredPaths,
        string basePath,
        string aliasPrefix,
        string serverPort)
    {
        string fullBasePath = System.IO.Path.GetFullPath(basePath);
        var addedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        int inspectedPathCount = 0;
        foreach (string configuredPath in configuredPaths)
        {
            if (inspectedPathCount >= MaximumConfiguredLoaderPaths)
            {
                warnings.Add($"Solo se inspeccionan las primeras {MaximumConfiguredLoaderPaths} rutas de carga de LabAPI ({aliasPrefix}).");
                break;
            }
            inspectedPathCount++;

            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                warnings.Add($"Se ignoró una ruta de carga vacía de LabAPI en {aliasPrefix}.");
                continue;
            }

            string resolvedPath = configuredPath.Replace("$port", serverPort);
            if (System.IO.Path.IsPathRooted(resolvedPath))
            {
                warnings.Add($"Se ignoró la ruta de carga absoluta de LabAPI '{configuredPath}'; solo se inspeccionan rutas bajo su carpeta oficial.");
                continue;
            }

            string normalized = resolvedPath.Replace(
                System.IO.Path.AltDirectorySeparatorChar,
                System.IO.Path.DirectorySeparatorChar);
            if (normalized.Split(System.IO.Path.DirectorySeparatorChar).Any(part => part == ".."))
            {
                warnings.Add($"Se ignoró la ruta de carga de LabAPI '{configuredPath}' porque sale de la carpeta oficial.");
                continue;
            }

            try
            {
                string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(fullBasePath, normalized));
                if (!IsWithinOrEqual(fullBasePath, fullPath))
                {
                    warnings.Add($"Se ignoró la ruta de carga de LabAPI '{configuredPath}' porque sale de la carpeta oficial.");
                    continue;
                }

                if (!addedPaths.Add(fullPath))
                    continue;

                string alias = aliasPrefix + (scanRoots.Count + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
                Add(roots, alias, fullPath);
                if (roots.ContainsKey(alias))
                    scanRoots.Add(alias);
            }
            catch (Exception exception) when (
                exception is ArgumentException ||
                exception is NotSupportedException ||
                exception is PathTooLongException)
            {
                warnings.Add($"Se ignoró la ruta de carga de LabAPI '{configuredPath}': {exception.Message}");
            }
        }
    }

    private static bool IsWithinOrEqual(string root, string path)
    {
        string fullRoot = System.IO.Path.GetFullPath(root).TrimEnd(
            System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.AltDirectorySeparatorChar);
        string fullPath = System.IO.Path.GetFullPath(path).TrimEnd(
            System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.AltDirectorySeparatorChar);
        if (string.Equals(fullRoot, fullPath, StringComparison.OrdinalIgnoreCase))
            return true;

        string prefix = fullRoot + System.IO.Path.DirectorySeparatorChar;
        return fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetLabApiAppDataDirectory(string serverRoot, out bool usesHostedAppData)
    {
        string workingDirectory = Environment.CurrentDirectory;
        string policyPath = System.IO.Path.Combine(workingDirectory, "hoster_policy.txt");
        usesHostedAppData = File.Exists(policyPath) &&
                            File.ReadAllLines(policyPath).Any(line =>
                                line.Contains("gamedir_for_configs: true", StringComparison.OrdinalIgnoreCase));

        if (usesHostedAppData)
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(workingDirectory, "AppData"));

        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return string.IsNullOrWhiteSpace(appData) ? serverRoot : appData;
    }

    private static void Add(Dictionary<string, AccessRoot> roots, string name, string path)
    {
        if (!SafeName.IsMatch(name) || roots.ContainsKey(name))
            return;

        roots.Add(name, new AccessRoot(name, path));
    }

}

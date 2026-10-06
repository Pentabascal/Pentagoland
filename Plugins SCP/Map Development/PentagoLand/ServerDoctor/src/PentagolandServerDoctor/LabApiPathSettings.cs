using System.Collections.Generic;

namespace Pentagoland.ServerDoctor;

public sealed class LabApiPathSettings
{
    public LabApiPathSettings(
        string appDataPath,
        string secretLabPath,
        string scpslConfigPath,
        string labApiPath,
        string configsPath,
        string pluginsPath,
        string dependenciesPath,
        IEnumerable<string> pluginPaths,
        IEnumerable<string> dependencyPaths,
        string serverPort,
        bool usesHostedAppData)
    {
        AppDataPath = appDataPath;
        SecretLabPath = secretLabPath;
        ScpslConfigPath = scpslConfigPath;
        LabApiPath = labApiPath;
        ConfigsPath = configsPath;
        PluginsPath = pluginsPath;
        DependenciesPath = dependenciesPath;
        PluginPaths = pluginPaths;
        DependencyPaths = dependencyPaths;
        ServerPort = serverPort;
        UsesHostedAppData = usesHostedAppData;
    }

    public string AppDataPath { get; }
    public string SecretLabPath { get; }
    public string ScpslConfigPath { get; }
    public string LabApiPath { get; }
    public string ConfigsPath { get; }
    public string PluginsPath { get; }
    public string DependenciesPath { get; }
    public IEnumerable<string> PluginPaths { get; }
    public IEnumerable<string> DependencyPaths { get; }
    public string ServerPort { get; }
    public bool UsesHostedAppData { get; }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandSystem;

namespace CommandSystem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CommandHandlerAttribute : Attribute
    {
        public CommandHandlerAttribute(Type handlerType) { }
    }

    public sealed class RemoteAdminCommandHandler { }
    public sealed class GameConsoleCommandHandler { }

    public interface ICommand
    {
        bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response);
        string Command { get; }
        string[] Aliases { get; }
        string Description { get; }
    }

    public interface ICommandSender { }
}

namespace LabApi.Features
{
    public static class LabApiProperties
    {
        public const string CompiledVersion = "1.1.7";
        public static readonly Version CurrentVersion = Version.Parse(CompiledVersion);
    }
}

namespace LabApi.Loader.Features.Plugins
{
    public abstract class Plugin
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string Author { get; }
        public abstract Version RequiredApiVersion { get; }
        public virtual bool IsTransparent => false;
        public string FilePath { get; set; } = string.Empty;
        public virtual void Enable() { }
        public virtual void Disable() { }
    }

    public abstract class Plugin<TConfig> : Plugin where TConfig : class, new()
    {
        public TConfig Config { get; set; } = new();
    }
}

namespace LabApi.Loader.Features.Configuration
{
    public sealed class LabApiConfig
    {
        public List<string> PluginPaths { get; set; } = ["global", "$port"];
        public List<string> DependencyPaths { get; set; } = ["global", "$port"];
    }
}

namespace LabApi.Loader
{
    public static class PluginLoader
    {
        public static LabApi.Loader.Features.Configuration.LabApiConfig Config { get; set; } = new();
        public static bool Initialized { get; set; }
        public static Dictionary<LabApi.Loader.Features.Plugins.Plugin, Assembly> Plugins { get; } = [];
        public static HashSet<LabApi.Loader.Features.Plugins.Plugin> EnabledPlugins { get; } = [];
        public static HashSet<Assembly> Dependencies { get; } = [];
    }
}

namespace LabApi.Loader.Features.Paths
{
    public static class PathManager
    {
        public static DirectoryInfo AppData
        {
            get
            {
                string policyPath = Path.Combine(Environment.CurrentDirectory, "hoster_policy.txt");
                bool usesHostedAppData = File.Exists(policyPath) &&
                    File.ReadAllLines(policyPath).Any(line =>
                        line.Contains("gamedir_for_configs: true", StringComparison.OrdinalIgnoreCase));
                return new DirectoryInfo(usesHostedAppData
                    ? Path.Combine(Environment.CurrentDirectory, "AppData")
                    : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }

        public static DirectoryInfo SecretLab => new(Path.Combine(AppData.FullName, "SCP Secret Laboratory"));
        public static DirectoryInfo LabApi => new(Path.Combine(SecretLab.FullName, "LabAPI"));
        public static DirectoryInfo Plugins => new(Path.Combine(LabApi.FullName, "plugins"));
        public static DirectoryInfo Dependencies => new(Path.Combine(LabApi.FullName, "dependencies"));
        public static DirectoryInfo Configs => new(Path.Combine(LabApi.FullName, "configs"));
        public static string GameplayConfigPath => Path.Combine(
            SecretLab.FullName,
            "config",
            global::LabApi.Features.Wrappers.Server.Port.ToString(),
            "config_gameplay.txt");
    }
}

namespace LabApi.Features.Permissions
{
    public static class PermissionExtensions
    {
        public static bool IsAllowed { get; set; } = true;
        public static bool HasAnyPermission(this ICommandSender sender, params string[] permissions) => IsAllowed;
    }
}

namespace LabApi.Features.Wrappers
{
    public static class Server
    {
        public static ushort Port { get; set; } = 7777;
        public static int PlayerCount { get; set; } = 4;
        public static int MaxPlayers { get; set; } = 20;
        public static double Tps { get; set; } = 60;
        public static short MaxTps { get; set; } = 60;
    }

    public sealed class Player
    {
        public string UserId { get; set; } = string.Empty;
        public static Player? Get(ICommandSender sender) => null;
    }
}

namespace LabApi.Features.Console
{
    public static class Logger
    {
        public static void Error(string message) { }
    }
}

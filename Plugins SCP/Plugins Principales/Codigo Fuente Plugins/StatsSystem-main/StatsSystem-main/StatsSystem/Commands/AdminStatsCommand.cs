using System;
using System.Linq;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace StatsSystem.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public sealed class AdminStatsCommand : ParentCommand
{
    public AdminStatsCommand()
    {
        LoadGeneratedCommands();
    }

    public override string Command => "ss";
    public override string[] Aliases { get; } = ["statsadmin"];
    public override string Description => "Comandos de administración de StatsSystem. Ejecuta 'ss help' para ver los subcomandos.";

    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new SaveSubcommand());
        RegisterCommand(new ReloadSubcommand());
        RegisterCommand(new ResetPlayerSubcommand());
        RegisterCommand(new InfoSubcommand());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Uso: ss <save | reload | resetplayer | info>";
        return false;
    }

    internal static bool AdminCheck(ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player == null)
        {
            response = null;
            return true;
        }

        if (!player.HasPermissions("stat.manage"))
        {
            response = "Falta el permiso: stat.manage";
            return false;
        }

        response = null;
        return true;
    }
}

internal sealed class SaveSubcommand : ICommand
{
    public string Command => "save";
    public string[] Aliases => ["s"];
    public string Description => "Guarda inmediatamente todas las estadísticas en disco.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!AdminStatsCommand.AdminCheck(sender, out response)) return false;
        StatsSystemPlugin.Stats.Save();
        response = "Estadísticas guardadas en disco.";
        return true;
    }
}

internal sealed class ReloadSubcommand : ICommand
{
    public string Command => "reload";
    public string[] Aliases => ["r"];
    public string Description => "Vuelve a cargar las estadísticas desde el disco y descarta los cambios sin guardar.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!AdminStatsCommand.AdminCheck(sender, out response)) return false;
        StatsSystemPlugin.Stats.Reload();
        response = "Estadísticas recargadas desde el disco.";
        return true;
    }
}

internal sealed class ResetPlayerSubcommand : ICommand, IUsageProvider
{
    public string Command => "resetplayer";
    public string[] Aliases => ["reset", "rp"];
    public string Description => "Elimina todas las estadísticas de un jugador. Uso: ss resetplayer <userId>";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!AdminStatsCommand.AdminCheck(sender, out response)) return false;
        if (arguments.Count == 0)
        {
            response = "Uso: ss resetplayer <userId>";
            return false;
        }

        var userId = arguments.At(0);
        if (!StatsSystemPlugin.Stats.DeletePlayerStats(userId))
        {
            response = $"No se encontraron estadísticas para '{userId}'.";
            return false;
        }

        response = $"Se han eliminado todas las estadísticas de '{userId}'.";
        return true;
    }

    public string[] Usage => ["<userId>"];
}

internal sealed class InfoSubcommand : ICommand
{
    public string Command => "info";
    public string[] Aliases => ["i"];
    public string Description => "Muestra información sobre el estado de StatsSystem.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!AdminStatsCommand.AdminCheck(sender, out response)) return false;

        var snapshot = StatsSystemPlugin.Stats.GetAllStatsSnapshot();
        var allKeys = snapshot.Values
            .Where(s => s != null)
            .SelectMany(s => s.GetAllKeys())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var keyLine = allKeys.Count > 0 ? $"  Claves: {string.Join(", ", allKeys)}" : "  Claves: (ninguna)";
        response = string.Join("\n",
            $"=== StatsSystem v{StatsSystemPlugin.Singleton.Version} ===",
            $"  Jugadores registrados: {snapshot.Count}",
            $"  Claves de estadísticas: {allKeys.Count}",
            keyLine,
            $"  Guardado automático cada: {StatsSystemPlugin.Singleton.Config.AutoSaveIntervalSeconds} s"
        );
        return true;
    }
}
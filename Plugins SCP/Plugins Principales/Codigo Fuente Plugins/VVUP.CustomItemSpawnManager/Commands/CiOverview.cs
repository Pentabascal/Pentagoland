using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace VVUP.CustomItemSpawnManager.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class CiOverviewCommand : ParentCommand
{
    public CiOverviewCommand()
    {
        LoadGeneratedCommands();
    }

    public override string Command { get; } = "vvupci";

    public override string[] Aliases { get; } = Array.Empty<string>();

    public override string Description { get; } = "Lists VVUP custom items that naturally spawned and remain on the map.";

    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new CiOverview());
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Subcommand required: overview";
        return false;
    }
}

public sealed class CiOverview : ICommand
{
    public string Command { get; } = "overview";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description { get; } = "Lists naturally spawned custom items that remain on the map.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
            return false;

        if (arguments.Count != 0)
        {
            response = "Usage: vvupci overview";
            return false;
        }

        List<string> lines = NaturalSpawnTracker.GetOverviewLines();
        if (lines.Count == 0)
        {
            response = "No naturally spawned custom items remain on the map.";
            return true;
        }

        response = $"Naturally spawned custom items on map: {lines.Count}\n- " +
                   string.Join("\n- ", lines);
        return true;
    }
}

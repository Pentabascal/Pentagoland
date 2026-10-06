using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Inventory
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Inventory : ParentCommand, IUsageProvider
    {
        public Inventory() => LoadGeneratedCommands();

        public override string Command { get; } = "inventory";

        public override string[] Aliases { get; } = new string[] { "inv" };

        public override string Description { get; } = "Gestiona los inventarios de los jugadores";

        public string[] Usage { get; } = new string[] { "drop / see", };

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Drop());
            RegisterCommand(new See());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            response = "Subcomando no válido. Subcomandos disponibles: drop, see";
            return false;
        }
    }
}

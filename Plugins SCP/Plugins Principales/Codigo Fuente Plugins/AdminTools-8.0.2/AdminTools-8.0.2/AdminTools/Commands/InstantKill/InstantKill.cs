using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.InstantKill
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class InstantKill : ParentCommand, IUsageProvider
    {
        public InstantKill() => LoadGeneratedCommands();
        public override string Command { get; } = "instantkill";

        public override string[] Aliases { get; } = new string[] { "ik", "instakill" };

        public override string Description { get; } = "Gestiona la eliminación instantánea de jugadores";

        public string[] Usage { get; } = new string[] { "Add or Remove or List", };

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Add());
            RegisterCommand(new Remove());
            RegisterCommand(new List());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            response = "Uso:" +
                "\nInstantKill add ((ID / nombre del jugador) o (all / *))" +
                "\nInstantKill list" +
                "\nInstantKill remove (ID / nombre del jugador) o (all / *))";
            return false;

        }
    }
}

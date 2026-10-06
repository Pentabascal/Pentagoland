using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Mute
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Mute : ParentCommand
    {
        public Mute() => LoadGeneratedCommands();

        public override string Command { get; } = "pmute";

        public override string[] Aliases { get; } = Array.Empty<string>();

        public override string Description { get; } = "Silencia a todos los jugadores del servidor, también en el intercomunicador";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new All());
            RegisterCommand(new Com());
            RegisterCommand(new RoundStart());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            response = "Subcomando no válido. Subcomandos disponibles: icom, all, roundstart";
            return false;
        }
    }
}

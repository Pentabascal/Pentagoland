using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Unmute
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Unmute : ParentCommand, IUsageProvider
    {
        public Unmute() => LoadGeneratedCommands();

        public override string Command { get; } = "punmute";

        public override string[] Aliases { get; } = Array.Empty<string>();

        public override string Description { get; } = "Quita el silenciamiento general o del intercomunicador en el servidor";

        public string[] Usage { get; } = new string[] { "all/icom/roundstart" };

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

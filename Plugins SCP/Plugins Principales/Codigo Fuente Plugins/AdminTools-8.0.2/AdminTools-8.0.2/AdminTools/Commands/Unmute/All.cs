using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;
using System;

namespace AdminTools.Commands.Unmute
{
    public class All : ICommand
    {
        public string Command { get; } = "all";

        public string[] Aliases { get; } = new string[] { "*" };

        public string Description { get; } = "Quita el silenciamiento a todos los jugadores del servidor";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Uso: punmute all";
                return false;
            }

            foreach (Player ply in Player.List)
                if (!ply.ReferenceHub.serverRoles.RemoteAdmin)
                {
                    ply.IsIntercomMuted = false;
                    ply.IsMuted = false;
                }

            response = "Todos los jugadores del servidor que no forman parte del personal ya pueden hablar libremente";
            return true;
        }
    }
}

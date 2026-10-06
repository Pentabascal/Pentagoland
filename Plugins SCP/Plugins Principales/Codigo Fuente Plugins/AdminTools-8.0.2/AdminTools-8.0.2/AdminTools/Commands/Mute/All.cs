using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Mute
{
    public class All : ICommand
    {
        public string Command { get; } = "all";

        public string[] Aliases { get; } = new string[] { "*" };

        public string Description { get; } = "Silencia por completo a todos los jugadores del servidor";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Uso: pmute all";
                return false;
            }

            foreach (Player player in Player.List)
                if (!player.ReferenceHub.serverRoles.RemoteAdmin)
                    player.IsMuted = true;

            response = "Se ha silenciado por completo a todos los jugadores del servidor que no forman parte del personal";
            return true;
        }
    }
}

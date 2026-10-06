using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Mute
{
    public class Com : ICommand
    {
        public string Command { get; } = "icom";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Silencia el intercomunicador de todos los jugadores del servidor";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Uso: pmute icom";
                return false;
            }

            foreach (Player player in Player.List)
                if (!player.ReferenceHub.serverRoles.RemoteAdmin)
                    player.IsIntercomMuted = true;

            response = "Se ha silenciado el intercomunicador de todos los jugadores del servidor que no forman parte del personal";
            return true;
        }
    }
}

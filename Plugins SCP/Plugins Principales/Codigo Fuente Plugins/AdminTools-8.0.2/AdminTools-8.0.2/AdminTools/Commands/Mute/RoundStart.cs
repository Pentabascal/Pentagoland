using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Mute
{
    public class RoundStart : ICommand
    {
        public string Command { get; } = "roundstart";

        public string[] Aliases { get; } = new string[] { "rs" };

        public string Description { get; } = "Silencia a todos los jugadores hasta que empiece la ronda.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Uso: pmute roundstart";
                return false;
            }

            if (Round.IsStarted)
            {
                response = "No puedes usar este comando después de que haya empezado la ronda.";
                return false;
            }

            foreach (Player player in Player.List)
            {
                if (!player.IsMuted && !player.ReferenceHub.serverRoles.RemoteAdmin)
                {
                    player.IsMuted = true;
                    Main.RoundStartMutes.Add(player);
                }
            }

            response = "Se ha silenciado a todos los jugadores ajenos al personal hasta que empiece la ronda.";
            return true;
        }
    }
}

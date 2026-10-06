using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;

namespace AdminTools.Commands.Unmute
{
    public class RoundStart : ICommand
    {
        public string Command { get; } = "roundstart";

        public string[] Aliases { get; } = new string[] { "rs" };

        public string Description { get; } = "Quita el silenciamiento de todos los jugadores hasta que empiece la ronda.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Uso: pumute roundstart";
                return false;
            }

            foreach (Player player in Main.RoundStartMutes)
            {
                player.IsMuted = false;
            }
            Main.RoundStartMutes.Clear();

            response = "Se ha quitado el silenciamiento a todos los jugadores ajenos al personal que estaban silenciados hasta el inicio de la ronda.";
            return true;
        }
    }
}

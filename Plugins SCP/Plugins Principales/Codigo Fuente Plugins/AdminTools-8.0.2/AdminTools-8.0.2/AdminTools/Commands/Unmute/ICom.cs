using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;
using System;

namespace AdminTools.Commands.Unmute
{
    public class Com : ICommand
    {
        public string Command { get; } = "icom";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Quita el silenciamiento del intercomunicador a todos los jugadores del servidor";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.mute"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Uso: punmute icom";
                return false;
            }

            foreach (Player ply in Player.List)
                ply.IsIntercomMuted = false;

            response = "Todos los jugadores del servidor que no forman parte del personal ya pueden hablar por el intercomunicador";
            return true;
        }
    }
}

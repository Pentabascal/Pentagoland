using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast
{
    internal class Clear : ICommand
    {
        public string Command { get; } = "clear";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Borra los mensajes en pantalla de uno o varios jugadores";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count < 1)
            {
                response = "Uso: hbc clear (ID o nombres de usuario)";
                return false;
            }

            IEnumerable<Player> ply = Player.GetProcessedData(arguments);
            if (ply.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            foreach (Player player in ply)
            {
                player.ShowHint(string.Empty);
            }
            response = "Se han borrado los mensajes en pantalla de los jugadores";
            return true;
        }
    }
}

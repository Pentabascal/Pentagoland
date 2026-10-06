using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;
using Exiled.API.Features;

namespace AdminTools.Commands.HintBroadcast
{
    internal class User : ICommand
    {
        public string Command { get; } = "user";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Envía un mensaje en pantalla a varios jugadores";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
        {
            if (arguments.Count < 3)
            {
                response = "Uso: hbc user (ID o nombres de usuario) (duración) (mensaje)";
                return false;
            }

            IEnumerable<Player> ply = Player.GetProcessedData(arguments);
            if (ply.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (!ushort.TryParse(arguments.At(1), out ushort time) && time <= 0)
            {
                response = $"Duración no válida: {arguments.At(1)}";
                return false;
            }

            foreach (Player player in ply)
            {
                player.ShowHint(Extensions.FormatArguments(arguments, 2), time);
            }

            response = "Mensaje enviado a los jugadores";
            return true;
        }
    }
}

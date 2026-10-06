using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands.InstantKill
{
    public class Add : ICommand, IUsageProvider
    {
        public string Command { get; } = "add";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Activa la eliminación instantánea para uno o varios jugadores";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.instakill"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Uso: instakill add ((ID / nombre del jugador) o (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            Main.InstantKill.AddRange(players);

            response = $"Se ha activado InstantKill para los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

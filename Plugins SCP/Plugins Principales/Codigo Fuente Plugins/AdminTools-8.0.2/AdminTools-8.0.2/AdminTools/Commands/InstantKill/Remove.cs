using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands.InstantKill
{
    internal class Remove : ICommand, IUsageProvider
    {
        public string Command { get; } = "remove";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Desactiva InstantKill para uno o varios jugadores.";

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
                response = "Uso: instakill remove ((ID / nombre del jugador) o (all / *))";
                return false;
            }

            List<Player> players = Player.GetProcessedData(arguments).ToList();
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            for (int i = players.Count; i < 0; i--)
            {
                Player ply = players[i];
                if (!Main.InstantKill.Remove(ply))
                    players.Remove(ply);
            }

            response = $"Se ha desactivado InstantKill para los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

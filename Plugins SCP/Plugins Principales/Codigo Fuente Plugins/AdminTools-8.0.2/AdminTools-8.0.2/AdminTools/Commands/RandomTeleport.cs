using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class RandomTeleport : ICommand, IUsageProvider
    {
        public string Command { get; } = "randomtp";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Teletransporta al azar a uno o todos los jugadores a una sala de las instalaciones";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CommandProcessor.CheckPermissions(((CommandSender)sender), "randomtp", PlayerPermissions.PlayersManagement, "AdminTools", false))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Uso: randomtp ((ID / nombre del jugador) o (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            foreach (Player ply in players)
            {
                ply.RandomTeleport(typeof(Room));
            }

            response = $"Los siguientes jugadores se han teletransportado a una sala aleatoria de las instalaciones:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using Exiled.API.Extensions;
using System.Collections.Generic;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TeleportX : ICommand, IUsageProvider
    {
        public string Command { get; } = "teleportx";

        public string[] Aliases { get; } = new string[] { "tpx", "tpto" };

        public string Description { get; } = "Teletransporta a uno o varios jugadores hasta otro jugador";

        public string[] Usage { get; } = new string[] { "%player%", "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
                return false;

            if (arguments.Count != 2)
            {
                response = "Uso: teleportx (jugadores que se teletransportan: (ID / nombre del jugador) o (all / *)) (destino: (ID / nombre del jugador))";
                return false;
            }

            Player ply = Player.GetProcessedData(arguments, 1).GetRandomValue();
            if (ply == null)
            {
                response = $"No se ha encontrado al jugador: {arguments.At(1)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            foreach (Player plyr in players)
            {
                plyr.Position = ply.Position;
            }

            response = $"Los siguientes jugadores se han teletransportado hasta {ply.Nickname} ({ply.Id}):\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

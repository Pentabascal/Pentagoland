using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Explode : ICommand, IUsageProvider
    {
        public string Command { get; } = "explode";

        public string[] Aliases { get; } = new string[] { "expl", "boom" };

        public string Description { get; } = "Hace explotar al instante a un jugador o a todos";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.explode"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Uso: expl ((ID / nombre del jugador) o (all / *))";
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
                if (ply.IsDead)
                    continue;

                ply.Explode();
                ply.Kill("Exploded by admin.");
            }
            response = $"Han explotado los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

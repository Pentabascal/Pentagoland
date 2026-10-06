using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class BreakDoors : ICommand, IUsageProvider
    {
        public string Command { get; } = "breakdoors";

        public string[] Aliases { get; } = new string[] { "bd" };

        public string Description { get; } = "Gestiona la capacidad de los jugadores para romper puertas y compuertas";

        public string[] Usage { get; } = new string[] { "%player%", "IsEnable" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.bd"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count == 0)
            {
                response = "Uso: breakdoors (all / *) [IsEnable]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);

            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.ElementAtOrDefault(0)}";
                return false;
            }

            bool? isJail = null;
            if (bool.TryParse(arguments.ElementAtOrDefault(1), out bool result))
                isJail = result;

            foreach (Player player in players)
                if (isJail is true)
                {
                    if (Main.BreakDoors.Contains(player))
                        Main.BreakDoors.Add(player);
                }
                else if (isJail is false)
                {
                    Main.BreakDoors.Remove(player);
                }
                else
                {
                    if (!Main.BreakDoors.Remove(player))
                        Main.BreakDoors.Add(player);
                }

            response = $"Se ha actualizado la capacidad BreakDoor de los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

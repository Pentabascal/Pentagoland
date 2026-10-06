using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;

namespace AdminTools.Commands.Inventory
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Drop : ICommand, IUsageProvider
    {
        public string Command { get; } = "drop";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Suelta los objetos del inventario de uno o varios jugadores";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count != 1)
            {
                response = "Uso: inventory drop ((ID / nombre del jugador) o (all / *))";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            foreach (Player p in players)
                p.DropItems();

            response = $"Se han soltado todos los objetos de los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
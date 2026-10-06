using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using Exiled.API.Features.Pickups;
using System.Collections.Generic;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DropSize : ICommand, IUsageProvider
    {
        public string Command { get; } = "dropsize";

        public string[] Aliases { get; } = new string[] { "drops" };

        public string Description { get; } = "Genera un objeto del tamaño indicado para uno o todos los jugadores";

        public string[] Usage { get; } = new string[] { "%player%", "%item%", "tamaño", "[tamaño]", "[tamaño]" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
                return false;

            if (arguments.Count < 3)
            {
                response = "Uso:\ndropsize ((ID / nombre del jugador) o (all / *)) (ItemType) (tamaño)\ndropsize ((ID / nombre del jugador) o (all / *)) (ItemType) (tamaño x) (tamaño y) (tamaño z)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (!Enum.TryParse(arguments.At(1), true, out ItemType type))
            {
                response = $"Nombre de objeto no válido: {arguments.At(1)}";
                return false;
            }

            switch (arguments.Count)
            {
                case 3:
                    if (!float.TryParse(arguments.At(2), out float size))
                    {
                        response = $"Tamaño del objeto no válido: {arguments.At(2)}";
                        return false;
                    }
                    SpawnItem(players, type, size, out response);
                    return true;
                case 5:
                    if (!float.TryParse(arguments.At(2), out float xval))
                    {
                        response = $"Tamaño del objeto no válido: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float yval))
                    {
                        response = $"Tamaño del objeto no válido: {arguments.At(3)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(4), out float zval))
                    {
                        response = $"Tamaño del objeto no válido: {arguments.At(4)}";
                        return false;
                    }
                    SpawnItem(players, type, xval, yval, zval, out response);
                    return true;
                default:
                    response = "\nUso:\ndrops (all / *) (ItemType) (tamaño) \ndrops (all / *) (ItemType) (tamaño x) (tamaño y) (tamaño z)";
                    return false;
            }
        }

        private void SpawnItem(IEnumerable<Player> players, ItemType type, float size, out string message)
            => SpawnItem(players, type, size, size, size, out message);

        private void SpawnItem(IEnumerable<Player> players, ItemType type, float x, float y, float z, out string message)
        {
            foreach (Player ply in players)
            {
                if (ply.IsDead)
                    continue;

                Pickup.CreateAndSpawn(type, ply.Position, default, ply).Scale = new(x, y, z);
            }
            message = $"Se ha generado un objeto {type} de tamaño ({x}, {y}, {z}) para los siguientes jugadores:\n{players.LogPlayers()}";
        }
    }
}

using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Size : ICommand, IUsageProvider
    {
        public string Command { get; } = "size";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Establece el tamaño de uno o todos los jugadores";

        public string[] Usage { get; } = new string[] { "%player%", "x", "y", "z" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.size"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Uso:\nsize (ID / nombre del jugador) o (all / *)) (valor x) (valor y) (valor z)" +
                    "\nsize reset";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (arguments.Count != 4)
            {
                response = "Uso: size (all / *) (x) (y) (z)";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float x))
            {
                response = $"Valor no válido para el tamaño x: {arguments.At(1)}";
                return false;
            }

            if (!float.TryParse(arguments.At(2), out float y))
            {
                response = $"Valor no válido para el tamaño y: {arguments.At(2)}";
                return false;
            }

            if (!float.TryParse(arguments.At(3), out float z))
            {
                response = $"Valor no válido para el tamaño z: {arguments.At(3)}";
                return false;
            }
            Vector3 size = new(x, y, z);
            foreach (Player ply in players)
            {
                ply.Scale = size;
            }

            response = $"Se ha establecido el tamaño {size} para los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;

        }
    }
}

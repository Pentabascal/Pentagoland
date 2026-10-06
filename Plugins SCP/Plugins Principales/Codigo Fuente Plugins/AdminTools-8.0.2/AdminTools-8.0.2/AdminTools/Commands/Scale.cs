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
    public class Scale : ICommand, IUsageProvider
    {
        public string Command { get; } = "scale";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Cambia la escala de un jugador o de todos.";

        public string[] Usage { get; } = new string[] { "%player%", "tamaño" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.size"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Uso:\nscale ((ID / nombre del jugador) o (all / *)) (valor)" +
                    "\nscale reset";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float scale))
            {
                response = $"Valor no válido para la escala: {arguments.At(1)}";
                return false;
            }

            Vector3 size = Vector3.one * scale;
            foreach (Player ply in players)
                ply.Scale = size;

            response = $"Se ha establecido la escala {size} para los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}

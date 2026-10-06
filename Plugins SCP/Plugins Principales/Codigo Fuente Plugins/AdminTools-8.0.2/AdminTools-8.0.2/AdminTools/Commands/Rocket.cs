using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Rocket : ICommand, IUsageProvider
    {
        public string Command { get; } = "rocket";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Convierte al jugador en un cohete pirotécnico.";

        public string[] Usage { get; } = new string[] { "%player%", "velocidad" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.rocket"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = "Uso: rocket ((ID / nombre del jugador) o (all / *)) (velocidad)";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            if (!float.TryParse(arguments.At(1), out float speed) && speed <= 0)
            {
                response = $"La velocidad debe ser un número positivo: {arguments.At(1)}";
                return false;
            }

            foreach (Player ply in players)
                Timing.RunCoroutine(DoRocket(ply, speed));

            response = $"Los siguientes jugadores se han convertido en cohetes pirotécnicos:\n{Extensions.LogPlayers(players)}";
            return true;
        }
        public static IEnumerator<float> DoRocket(Player player, float speed)
        {
            const int maxAmnt = 50;
            int amnt = 0;
            while (player.IsAlive)
            {
                player.Position += Vector3.up * speed;
                amnt++;
                if (amnt >= maxAmnt)
                {
                    player.IsGodModeEnabled = false;
                    player.Explode();
                    player.Kill("Went on a trip in their favorite rocket ship.");
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}

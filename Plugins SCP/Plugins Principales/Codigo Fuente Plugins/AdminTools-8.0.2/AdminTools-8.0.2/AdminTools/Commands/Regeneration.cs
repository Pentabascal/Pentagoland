using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using InventorySystem.Items.Usables;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Regeneration : ICommand, IUsageProvider
    {
        public string Command { get; } = "regeneration";

        public string[] Aliases { get; } = new string[] { "reg" };

        public string Description { get; } = "Configura la regeneración de los jugadores";
        public string[] Usage { get; } = new string[] { "%player%", "duración", "frecuencia" };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.reg"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 3)
            {
                response = "Uso: regeneration ((ID / nombre del jugador) o (all / *)) (duración) (frecuencia)";
                return false;
            }

            if (float.TryParse(arguments.At(1), out float duration))
            {
                response = $"La duración debe ser un número decimal: {arguments.At(1)}";
                return false;
            }

            if (float.TryParse(arguments.At(2), out float rate))
            {
                response = $"La frecuencia debe ser un número decimal: {arguments.At(1)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }

            AnimationCurve animationCurve = AnimationCurve.Constant(0f, duration, rate);
            RegenerationProcess reg = new(animationCurve, 1f, 1f);
            foreach (Player player in players)
                UsableItemsController.GetHandler(player.ReferenceHub).ActiveRegenerations.Add(reg);
            response = $"Se ha aplicado la regeneración a los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
            return true;
        }
    }
}
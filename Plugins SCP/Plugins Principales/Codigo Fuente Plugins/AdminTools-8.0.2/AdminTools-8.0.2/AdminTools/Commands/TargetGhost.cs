using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using Exiled.API.Features.Roles;
using System.Collections.Generic;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class TargetGhost : ICommand
    {
        public const string HelpStr = "Uso: targetghost (ID / nombre del jugador) (ID / nombre del jugador) ...";

        public string Command { get; } = "targetghost";

        public string[] Aliases { get; } = new string[] { "tg" };

        public string Description { get; } = "Vuelve invisible a un jugador para otro jugador";

        public string[] Usage { get; } = new string[] { "%player%", "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.targetghost"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 2)
            {
                response = HelpStr;
                return false;
            }

            IEnumerable<Player> sourcePlayers = Player.GetProcessedData(arguments);

            if (sourcePlayers.IsEmpty())
            {
                response = $"Jugador de origen no válido: {arguments.At(0)}";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments, 1);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(1)}";
                return false;
            }

            foreach (Player sourcePlayer in sourcePlayers)
            {
                foreach (Player victim in players)
                {
                    if (sourcePlayer.Role.Is(out FpcRole role))
                    {
                        if (!role.IsInvisibleFor.Add(victim))
                            role.IsInvisibleFor.Remove(victim);
                    }
                }
            }

            response = "Se ha actualizado la invisibilidad.";
            return true;
        }
    }
}
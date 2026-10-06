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
    public class Ghost : ICommand, IUsageProvider
    {
        public string Command { get; } = "ghost";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Vuelve invisible a un jugador o a todos";

        public string[] Usage { get; } = new string[] { "%player% / Clear", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.ghost"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Uso:\nghost ((ID / nombre del jugador) o (all / *))" +
                    "\nghost clear";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    {
                        foreach (Player pl in Player.List)
                            if (pl.Role is FpcRole fpc)
                                fpc.IsInvisible = false;

                        response = "Todos han dejado de ser invisibles";
                        return true;
                    }
                default:
                    {
                        IEnumerable<Player> players = Player.GetProcessedData(arguments);
                        if (players.IsEmpty())
                        {
                            response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                            return false;
                        }
                        foreach (Player pl in players)
                            if (pl.Role is FpcRole fpc)
                                fpc.IsInvisible = !fpc.IsInvisible;
                        
                        response = $"Se ha cambiado el estado de invisibilidad de los siguientes jugadores:\n{Extensions.LogPlayers(players)}";
                        return true;
                    }
            }
        }
    }
}

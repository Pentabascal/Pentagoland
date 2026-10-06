using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdminTools.Commands.PryGate
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class PryGates : ICommand, IUsageProvider
    {
        public string Command { get; } = "prygate";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Permite a los jugadores forzar compuertas, retirarles esa capacidad y consultar quién la tiene";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.prygate"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Uso:\nprygate ((ID / nombre del jugador) o (all / *))" +
                    "\nprygate clear" +
                    "\nprygate list" +
                    "\nprygate remove (player id / name)";
                return false;
            }

            IEnumerable<Player> players;

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 1)
                    {
                        response = "Uso: prygates clear";
                        return false;
                    }

                    Main.PryGate.Clear();
                    response = "Se ha retirado a todos los jugadores la capacidad de forzar compuertas";
                    return true;
                case "list":
                    if (arguments.Count != 1)
                    {
                        response = "Uso: prygates list";
                        return false;
                    }

                    StringBuilder playerLister = StringBuilderPool.Shared.Rent(Main.PryGate.Count != 0 ? "Jugadores que pueden forzar compuertas:\n" : "Ningún jugador conectado puede forzar compuertas");
                    if (Main.PryGate.Count > 0)
                    {
                        foreach (Player ply in Main.PryGate)
                            playerLister.Append(ply.Nickname + ", ");

                        int length = playerLister.ToString().Length;
                        response = playerLister.ToString().Substring(0, length - 2);
                        StringBuilderPool.Shared.Return(playerLister);
                        return true;
                    }
                    else
                    {
                        response = playerLister.ToString();
                        StringBuilderPool.Shared.Return(playerLister);
                        return true;
                    }
                case "remove":
                    if (arguments.Count != 2)
                    {
                        response = "Uso: prygate remove (ID / nombre del jugador)";
                        return false;
                    }

                    players = Player.GetProcessedData(arguments, 1);

                    if (players.IsEmpty())
                    {
                        response = $"No se ha encontrado al jugador: {arguments.At(1)}";
                        return false;
                    }
                    response = string.Empty;
                    foreach (Player ply in players)
                    {
                        if (Main.PryGate.Remove(ply))
                        {
                            response += $"El jugador «{ply.Nickname}» ya no puede forzar compuertas";
                            continue;
                        }
                        response += $"El jugador {ply.Nickname} no puede forzar compuertas";
                    }
                    return true;
                default:
                    if (arguments.Count != 1)
                    {
                        response = "Uso: prygates (all / *)";
                        return false;
                    }
                    players = Player.GetProcessedData(arguments);

                    if (players.IsEmpty())
                    {
                        response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                        return false;
                    }

                    foreach (Player ply in players)
                        Main.PryGate.Add(ply);

                    response = "Todos los jugadores ya pueden forzar compuertas.";
                    return true;
            }
        }
    }
}

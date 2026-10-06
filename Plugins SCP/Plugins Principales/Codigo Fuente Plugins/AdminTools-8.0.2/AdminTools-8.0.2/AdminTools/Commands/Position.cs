using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using NorthwoodLib.Pools;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Position : ICommand, IUsageProvider
    {
        public string Command { get; } = "position";

        public string[] Aliases { get; } = new string[] { "pos" };

        public string Description { get; } = "Modifica o consulta la posición de uno o todos los jugadores";

        public string[] Usage { get; } = new string[] { "%player%", string.Join(", ", Enum.GetNames(typeof(PositionModifier))), };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response) // TODO: Make it ParentCommand
        {
            if (!sender.CheckPermission("at.tp"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = response = "\nUso:\nposition ((ID / nombre del jugador) o (all / *)) (set) (posición x) (posición y) (posición z)\nposition ((ID / nombre del jugador) o (all / *)) (get)\nposition ((ID / nombre del jugador) o (all / *)) (add) (x, y o z) (valor)";
                return false;
            }
            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }
            if (!Enum.TryParse(arguments.At(1), true, out PositionModifier mod))
            {
                response = $"Modificador de posición no válido: {arguments.At(0)}";
                return false;
            }

            switch (mod)
            {
                case PositionModifier.Set:
                    if (arguments.Count != 5)
                    {
                        response = "Uso: position (all / *) (set) (posición x) (posición y) (posición z)";
                        return false;
                    }
                    if (!float.TryParse(arguments.At(2), out float xval))
                    {
                        response = $"Valor no válido para la posición x: {arguments.At(2)}";
                        return false;
                    }
                    if (!float.TryParse(arguments.At(3), out float yval))
                    {
                        response = $"Valor no válido para la posición y: {arguments.At(3)}";
                        return false;
                    }
                    if (!float.TryParse(arguments.At(4), out float zval))
                    {
                        response = $"Valor no válido para la posición z: {arguments.At(4)}";
                        return false;
                    }
                    if (players.IsEmpty())
                    {
                        response = "No se han encontrado jugadores";
                        return false;
                    }
                    foreach (Player ply in players)
                    {
                        ply.Position = new Vector3(xval, yval, zval);
                    }
                    response = $"Se ha establecido la posición de todos los jugadores en {xval} {yval} {zval}";
                    return true;
                case PositionModifier.Get:
                    if (arguments.Count != 2)
                    {
                        response = "Uso: position (all / *) (get)";
                        return false;
                    }
                    StringBuilder positionBuilder = StringBuilderPool.Shared.Rent();
                    if (players.IsEmpty())
                    {
                        response = "Ningún jugador coincide con la búsqueda.";
                        return false;
                    }
                    positionBuilder.Append("\n");
                    foreach (Player ply in players)
                    {
                        positionBuilder.Append(ply.Nickname);
                        positionBuilder.Append(" (");
                        positionBuilder.Append(ply.Id);
                        positionBuilder.Append(")");
                        positionBuilder.Append(" - posición: ");
                        positionBuilder.Append(ply.Position.x);
                        positionBuilder.Append(" ");
                        positionBuilder.Append(ply.Position.y);
                        positionBuilder.Append(" ");
                        positionBuilder.AppendLine(ply.Position.z.ToString());
                    }
                    string message = positionBuilder.ToString();
                    StringBuilderPool.Shared.Return(positionBuilder);
                    response = message;
                    return true;
                case PositionModifier.Add:
                    if (arguments.Count != 4)
                    {
                        response = "Uso: position (all / *) (add) (x, y o z) (valor)";
                        return false;
                    }
                    if (!Enum.TryParse(arguments.At(2), true, out VectorAxis axis))
                    {
                        response = $"Valor no válido para el eje del vector ({string.Join(", ", Enum.GetNames(typeof(PositionModifier)))}): {arguments.At(2)}";
                        return false;
                    }
                    if (!float.TryParse(arguments.At(3), out float val))
                    {
                        response = $"Valor no válido para la posición: {arguments.At(3)}";
                        return false;
                    }
                    switch (axis)
                    {
                        case VectorAxis.X:
                            foreach (Player ply in players)
                                ply.Position = new Vector3(ply.Position.x + val, ply.Position.y, ply.Position.z);

                            response = $"La posición x de todos los jugadores se ha desplazado {val}";
                            return true;
                        case VectorAxis.Y:
                            foreach (Player ply in players)
                                ply.Position = new Vector3(ply.Position.x, ply.Position.y + val, ply.Position.z);

                            response = $"La posición y de todos los jugadores se ha desplazado {val}";
                            return true;
                        case VectorAxis.Z:
                            foreach (Player ply in players)
                                ply.Position = new Vector3(ply.Position.x, ply.Position.y, ply.Position.z + val);

                            response = $"La posición z de todos los jugadores se ha desplazado {val}";
                            return true;
                        default:
                            response = "\nUso:\nposition (all / *) (set) (posición x) (posición y) (posición z)\nposition (all / *) (get)\nposition (all / *) (add) (x, y o z) (valor)";
                            return false;
                    }
                    
                default:
                    response = "\nUso:\nposition (all / *) (set) (posición x) (posición y) (posición z)\nposition (all / *) (get)\nposition (all / *) (add) (x, y o z) (valor)";
                    return false;
            }
        }
    }
}

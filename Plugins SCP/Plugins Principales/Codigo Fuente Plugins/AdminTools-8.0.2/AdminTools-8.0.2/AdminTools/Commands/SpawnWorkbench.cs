using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerRoles;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnWorkbench : ICommand, IUsageProvider
    {
        public string Command { get; } = "bench";

        public string[] Aliases { get; } = new string[] { "sw", "wb", "workbench" };

        public string Description { get; } = "Genera un banco de trabajo para uno o todos los jugadores";

        public string[] Usage { get; } = new string[] { "%player%", };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.benches"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (!(sender is PlayerCommandSender plysend))
            {
                response = "Debes estar dentro del juego para ejecutar este comando.";
                return false;
            }

            Player player = Player.Get(plysend.ReferenceHub);

            if (arguments.Count < 1)
            {
                response = "Uso: bench ((ID / nombre del jugador) o (all / *)) (valor x) (valor y) (valor z)" +
                    "\nbench clear (ID / nombre del jugador) (índice mínimo) (índice máximo)" +
                    "\nbench clearall" +
                    "\nbench count (player id / name)";
                return false;
            }

            switch (arguments.At(0))
            {
                case "clear":
                    if (arguments.Count != 4)
                    {
                        response = "Uso:\nbench clear (ID / nombre del jugador) (índice mínimo) (índice máximo)\n\nNOTA: El índice mínimo debe ser menor que el máximo. Puedes eliminar un intervalo de los bancos que hayas generado (desde 1 hasta el número total de bancos).";
                        return false;
                    }

                    Player ply = Player.Get(arguments.At(1));
                    if (ply == null)
                    {
                        response = $"No se ha encontrado al jugador: {arguments.At(1)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(2), out int min) && min < 0)
                    {
                        response = $"Índice mínimo no válido: {arguments.At(2)}";
                        return false;
                    }

                    if (!int.TryParse(arguments.At(3), out int max) && max < 0)
                    {
                        response = $"Índice máximo no válido: {arguments.At(3)}";
                        return false;
                    }

                    if (max < min)
                    {
                        response = $"{max} no es mayor que {min}";
                        return false;
                    }

                    if (!Main.BchHubs.TryGetValue(ply, out List<GameObject> objs))
                    {
                        response = $"{ply.Nickname} no ha generado ningún banco de trabajo";
                        return false;
                    }

                    if (min > objs.Count)
                    {
                        response = $"{min} (mínimo) supera el número de bancos de trabajo generados por {ply.Nickname} ({objs.Count}).";
                        return false;
                    }

                    if (max > objs.Count)
                    {
                        response = $"{max} (máximo) supera el número de bancos de trabajo generados por {ply.Nickname} ({objs.Count}).";
                        return false;
                    }

                    min = min == 0 ? 0 : min - 1;
                    max = max == 0 ? 0 : max - 1;

                    for (int i = min; i <= max; i++)
                    {
                        UnityEngine.Object.Destroy(objs.ElementAt(i));
                        objs[i] = null;
                    }
                    objs.RemoveAll(r => r == null);

                    response = $"Se han eliminado del jugador {ply.Nickname} los bancos de trabajo del {min + 1} al {max + 1}.";
                    return true;
                case "clearall":
                    if (arguments.Count != 1)
                    {
                        response = "Uso: bench clearall";
                        return true;
                    }

                    foreach (KeyValuePair<Player, List<GameObject>> bch in Main.BchHubs)
                    {
                        foreach (GameObject bench in bch.Value)
                            UnityEngine.Object.Destroy(bench);
                        bch.Value.Clear();
                    }

                    Main.BchHubs.Clear();
                    response = "Se han eliminado todos los bancos de trabajo generados.";
                    return true;
                case "count":
                    if (arguments.Count != 2)
                    {
                        response = "Uso: bench count (ID / nombre del jugador)";
                        return false;
                    }

                    Player plyr = Player.Get(arguments.At(1));
                    if (plyr == null)
                    {
                        response = $"No se ha encontrado al jugador: {arguments.At(1)}";
                        return false;
                    }

                    if (!Main.BchHubs.TryGetValue(plyr, out List<GameObject> obj) || obj.Count == 0)
                    {
                        response = $"{plyr.Nickname} no ha generado ningún banco de trabajo";
                        return false;
                    }

                    response = $"{plyr.Nickname} ha generado {(obj.Count != 1 ? $"{obj.Count} bancos de trabajo" : $"{obj.Count} banco de trabajo")}";
                    return true;
                case "*":
                case "all":
                    if (arguments.Count != 4)
                    {
                        response = "Uso: bench (all / *) (valor x) (valor y) (valor z)";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(1), out float xval))
                    {
                        response = $"Valor no válido para el tamaño x: {arguments.At(1)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(2), out float yval))
                    {
                        response = $"Valor no válido para el tamaño y: {arguments.At(2)}";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(3), out float zval))
                    {
                        response = $"Valor no válido para el tamaño z: {arguments.At(3)}";
                        return false;
                    }

                    int index = 0;
                    foreach (Player p in Player.List)
                    {
                        if (p.Role == RoleTypeId.Spectator || p.Role == RoleTypeId.None)
                            continue;

                        Extensions.SpawnWorkbench(player, p.Position + p.ReferenceHub.PlayerCameraReference.forward * 2, p.GameObject.transform.rotation.eulerAngles, new Vector3(xval, yval, zval), out int benchIndex);
                        index = benchIndex;
                    }

                    response = $"Se han generado bancos de trabajo para todos. Has generado {(index != 1 ? $"{index} bancos de trabajo" : $"{index} banco de trabajo")} en total.";
                    return true;
                default:
                    if (arguments.Count != 4)
                    {
                        response = "Uso: bench (ID / nombre del jugador) (valor x) (valor y) (valor z)";
                        return false;
                    }

                    Player pl = Player.Get(arguments.At(0));
                    if (pl == null)
                    {
                        response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                        return false;
                    }
                    else if (pl.Role.IsDead)
                    {
                        response = "El rol de este jugador no permite generar un banco de trabajo.";
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

                    Extensions.SpawnWorkbench(player, pl.Position + pl.ReferenceHub.PlayerCameraReference.forward * 2, pl.GameObject.transform.rotation.eulerAngles, new Vector3(x, y, z), out int benchI);
                    response = $"Se ha generado un banco de trabajo para {pl.Nickname}. Has generado {(benchI != 1 ? $"{benchI} bancos de trabajo" : $"{benchI} banco de trabajo")} en total.";
                    return true;
            }
        }
    }
}

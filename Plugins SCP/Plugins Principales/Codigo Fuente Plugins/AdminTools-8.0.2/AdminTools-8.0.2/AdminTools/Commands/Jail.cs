using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminTools.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class Jail : ICommand, IUsageProvider
    {
        public string Command { get; } = Main.Instance.Config.JailCommandName;

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = "Encarcela o libera a un jugador";

        public string[] Usage { get; } = new string[] { "%player%", "[IsJail]"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("at.jail"))
            {
                response = "No tienes permiso para usar este comando";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Uso: jail (ID / nombre del jugador) [true/false]";
                return false;
            }

            IEnumerable<Player> players = Player.GetProcessedData(arguments);
            if (players.IsEmpty())
            {
                response = $"No se ha encontrado al jugador: {arguments.At(0)}";
                return false;
            }
            
            bool? isJail = null;
            if (bool.TryParse(arguments.ElementAtOrDefault(1), out bool result))
                isJail = result;

            foreach (Player ply in players)
            {
                
                if (isJail is true)
                    DoJail(ply);
                else if (isJail is false)
                    DoUnJail(ply);
                else
                {
                    if (Main.JailedPlayers.ContainsKey(ply.UserId))
                        DoUnJail(ply);
                    else
                        DoJail(ply);
                }
            }
            response = $"El comando de encarcelamiento se ha ejecutado correctamente.\n{Extensions.LogPlayers(players)}";
            return true;
        }
        public static void DoJail(Player player, bool skipadd = false)
        {
            if (Main.JailedPlayers.ContainsKey(player.UserId))
                return;
            if (!skipadd)
            {
                Main.JailedPlayers.Add(player.UserId, new Jailed
                {
                    Health = player.Health,
                    RelativePosition = player.RelativePosition,
                    Items = player.Items.ToList(),
                    Effects = player.ActiveEffects.Select(x => new Effect(x)).ToList(),
                    Name = player.Nickname,
                    Role = player.Role.Type,
                    CurrentRound = true,
                    Ammo = player.Ammo.ToDictionary(x => x.Key.GetAmmoType(), x => x.Value),
                });
            }

            if (player.IsOverwatchEnabled)
                player.IsOverwatchEnabled = false;
            player.Ammo.Clear();
            player.Inventory.SendAmmoNextFrame = true;

            player.ClearInventory(false);
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.UseSpawnpoint);
        }

        public static void DoUnJail(Player player)
        {
            if (!Main.JailedPlayers.TryGetValue(player.UserId, out Jailed jail))
                return;
            if (jail.CurrentRound)
            {
                player.Role.Set(jail.Role, RoleSpawnFlags.None);
                try
                {
                    player.ResetInventory(jail.Items);
                    player.Health = jail.Health;
                    player.Position = jail.RelativePosition.Position;
                    foreach (KeyValuePair<AmmoType, ushort> kvp in jail.Ammo)
                        player.Ammo[kvp.Key.GetItemType()] = kvp.Value;
                    player.SyncEffects(jail.Effects);

                    player.Inventory.SendItemsNextFrame = true;
                    player.Inventory.SendAmmoNextFrame = true;
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(DoUnJail)}: {e}");
                }
            }
            else
            {
                player.Role.Set(RoleTypeId.Spectator);
            }
            Main.JailedPlayers.Remove(player.UserId);
        }
    }
}

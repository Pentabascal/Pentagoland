using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using MEC;
using SwiftArcadeMode.Features.Humans.Perks.Content.Gambler;
using SwiftArcadeMode.Features.Humans.Perks.Content.SixthSense;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Features
{
    public static class PerkManager
    {
        public const string PerkNameSpace = "base";

        public struct PerkProfile(Rarity r, string name, string desc)
        {
            public Rarity Rarity = r;
            public string Name = name;
            public string Description = desc;

            public readonly string FancyName => Name.FancifyPerkName(Rarity);
        }

        public static readonly Dictionary<Player, PerkInventory> Inventories = [];
        public static readonly Dictionary<string, PerkAttribute> RegisteredPerks = [];

        public static void Enable()
        {
            if (Core.Instance.Config.AllowBaseContent)
            {
                RegisterPerks(PerkNameSpace);
                Gambler.RegisterEffects();
                SixthSense.RegisterSenses();
            }

            PlayerEvents.Death += OnPlayerDeath;
            PlayerEvents.ChangedSpectator += OnChangedSpectator;
            PlayerEvents.ChangedRole += OnChangedRole;
            ServerEvents.RoundRestarted += OnRoundRestarted;
        }

        public static void Disable()
        {
            PlayerEvents.Death -= OnPlayerDeath;
            PlayerEvents.ChangedSpectator -= OnChangedSpectator;
            PlayerEvents.ChangedRole -= OnChangedRole;
            ServerEvents.RoundRestarted -= OnRoundRestarted;
        }

        public static bool HasRestrictions(this Player player, IPerkInfo perk) => player.IsAlive && ((player.IsHuman && perk.Restriction == PerkRestriction.SCP) || (player.IsSCP && perk.Restriction == PerkRestriction.Human));

        private static void OnChangedRole(PlayerChangedRoleEventArgs ev)
        {
            ev.Player.SendHint("", 1f);

            if (ev.Player.TryGetPerkInventory(out PerkInventory inv))
            {
                inv.BaseLimit = ev.Player.IsSCP ? 2 : 5;

                if (inv.Perks.Count > 0)
                {
                    List<PerkBase> perksToRemove = [];
                    foreach (PerkBase p in inv.Perks)
                        if (ev.Player.HasRestrictions(p))
                            perksToRemove.Add(p);
                    if (perksToRemove.Count > 0)
                    {
                        foreach (PerkBase p in perksToRemove)
                            inv.RemovePerk(p);

                        ev.Player.SendHint("Se han eliminado " + perksToRemove.Count + " ventajas porque son incompatibles con tu rol.", 5f);
                    }
                }
            }
        }

        private static void OnChangedSpectator(PlayerChangedSpectatorEventArgs ev)
        {
            if (ev.NewTarget == null)
                return;

            ev.Player.UpdateSpectatorDisplay(ev.NewTarget);
        }

        public static void UpdateSpectatorDisplay(this Player player, Player target)
        {
            StringBuilder builder = new($"<align=\"left\">\n\n\nVentajas de {target.DisplayName}\n");

            if (Inventories.ContainsKey(target))
                foreach (PerkBase perk in Inventories[target].Perks)
                    builder.AppendLine($"- {perk.FancyName}");

            builder.Append("</align>");

            player.SendHint(builder.ToString(), 120f);
        }

        private static void OnRoundRestarted() => Inventories.Clear();

        private static void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (TryGetPerkInventory(ev.Player, out PerkInventory inv) && inv.Perks.Count > 0)
            {
                inv.RemoveRandom();
                inv.UpgradeQueue.Upgrades.Clear();
            }
        }

        public static string FancifyPerkName(this string perkName, Rarity rarity) => $"<color={rarity.GetColor()}><b>{perkName}</b></color>";

        public static void RegisterPerks(string nameSpace)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            Dictionary<Type, PerkAttribute> atts = callingAssembly
                .GetTypes()
                .Select(t => (type: t, attr: t.GetCustomAttribute<PerkAttribute>()))
                .Where(pair => pair.attr != null)
                .ToDictionary(pair => pair.type, pair => pair.attr);

            foreach (KeyValuePair<Type, PerkAttribute> attr in atts)
            {
                try
                {
                    attr.Value.Perk = attr.Key;
                    RegisteredPerks.Add((RegisteredPerks.ContainsKey(attr.Value.ID) ? nameSpace.ToLower() + "." : "") + attr.Value.ID.ToLower(), attr.Value);
                    PerkBase p = (PerkBase)Activator.CreateInstance(attr.Key, new PerkInventory(null));
                    attr.Value.Profile = new(attr.Value.Rarity, p.Name, p.Description);
                }
                catch (Exception ex)
                {
                    Logger.Error("Failed to load perk \"" + attr.Key + "\": \n" + ex);
                }
            }
        }

        public static bool TryGetPerkInventory(this Player p, out PerkInventory inv)
        {
            inv = p.GetPerkInventory();
            return inv != null;
        }

        public static PerkInventory GetPerkInventory(this Player p) => p == null ? null : Inventories.ContainsKey(p) ? Inventories[p] : Register(p);

        public static PerkAttribute GetRandomPerk(Func<PerkAttribute, bool> f) => RegisteredPerks.Values.Where(f).ToArray().GetWeightedRandom();

        public static PerkAttribute GetPerk(string id) => RegisteredPerks.ContainsKey(id) ? RegisteredPerks[id] : null;

        public static PerkAttribute GetPerk(Type type) => RegisteredPerks.Values.FirstOrDefault(att => att.Perk == type);

        public static bool TryGetPerk(string id, out PerkAttribute t)
        {
            t = GetPerk(id);
            return t != null;
        }

        public static bool TryGetPerk(Type type, out PerkAttribute t)
        {
            t = GetPerk(type);
            return t != null;
        }

        public static bool GivePerk(this Player p, Type t) => TryGetPerk(t, out PerkAttribute att) && p.GivePerk(att);

        public static bool GivePerk(this Player p, PerkAttribute t)
        {
            if (!Inventories.ContainsKey(p))
                Register(p);

            return Inventories[p].AddPerk(t);
        }

        public static void RemovePerk(this Player p, Type t)
        {
            if (!Inventories.ContainsKey(p))
            {
                Register(p);
                return;
            }

            Inventories[p].RemovePerk(t);
        }

        public static bool HasPerk(this Player p, Type perk) => Inventories.ContainsKey(p) && Inventories[p].HasPerk(perk);

        public static PerkInventory Register(Player p)
        {
            if (p == null)
                return null;

            if (Inventories.ContainsKey(p))
                return Inventories[p];

            PerkInventory inv = new(p);
            Inventories.Add(p, inv);
            return inv;
        }

        public static void Remove(Player p)
        {
            if (p != null)
                Inventories.Remove(p);
        }

        public static void Tick()
        {
            foreach (PerkInventory inv in Inventories.Values)
                inv?.Tick();
        }
    }
}

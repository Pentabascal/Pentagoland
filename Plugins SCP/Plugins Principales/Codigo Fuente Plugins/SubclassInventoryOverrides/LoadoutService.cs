using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassInventoryOverrides;

internal static class LoadoutService
{
    private const float PollIntervalSeconds = 0.25f;
    private const float AssignmentSettleSeconds = 1.0f;

    private static readonly Dictionary<string, CompiledLoadout> Loadouts = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<Player, PendingAssignment> Pending = new();
    private static readonly Dictionary<Player, int> AppliedAssignmentStamps = new();
    private static readonly Dictionary<Player, SpawnInventorySnapshot> SpawnInventories = new();
    private static readonly HashSet<string> MissingCustomItemWarnings = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> MissingSpawnInventoryWarnings = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> FailedCustomItemWarnings = new(StringComparer.OrdinalIgnoreCase);
    private static CoroutineHandle watcher;

    public static bool Initialize(Config config)
    {
        Loadouts.Clear();
        Pending.Clear();
        AppliedAssignmentStamps.Clear();
        MissingCustomItemWarnings.Clear();
        FailedCustomItemWarnings.Clear();

        bool valid = true;
        var duplicateIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (LoadoutDefinition? definition in config.Loadouts ?? new List<LoadoutDefinition>())
        {
            if (definition is null)
            {
                Log.Error("A null subclass inventory override entry was found; that entry will be ignored.");
                valid = false;
                continue;
            }

            if (string.IsNullOrWhiteSpace(definition.SubclassId))
            {
                Log.Error("A subclass inventory override has an empty subclass_id; that entry will be ignored.");
                valid = false;
                continue;
            }

            if (Loadouts.ContainsKey(definition.SubclassId) || !duplicateIds.Add(definition.SubclassId))
            {
                Log.Error($"Duplicate subclass inventory override '{definition.SubclassId}'; all entries with that ID will be ignored.");
                Loadouts.Remove(definition.SubclassId);
                valid = false;
                continue;
            }

            if (!TryCompile(definition, out CompiledLoadout? compiled) || compiled is null)
            {
                valid = false;
                continue;
            }

            Loadouts.Add(definition.SubclassId, compiled);
        }

        foreach (string duplicateId in duplicateIds)
            Loadouts.Remove(duplicateId);

        return valid;
    }

    public static void Start()
    {
        Exiled.Events.Handlers.Player.Spawned += OnPlayerSpawned;
        Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
        watcher = Timing.RunCoroutine(WatchAssignments());
    }

    public static void Stop()
    {
        Exiled.Events.Handlers.Player.Spawned -= OnPlayerSpawned;
        Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
        Timing.KillCoroutines(watcher);
        Pending.Clear();
        AppliedAssignmentStamps.Clear();
        SpawnInventories.Clear();
        MissingCustomItemWarnings.Clear();
        MissingSpawnInventoryWarnings.Clear();
        FailedCustomItemWarnings.Clear();
        Loadouts.Clear();
    }

    private static void OnPlayerSpawned(SpawnedEventArgs ev)
    {
        Player? player = ev?.Player;
        if (player is null || player is Npc)
            return;

        Pending.Remove(player);
        AppliedAssignmentStamps.Remove(player);
        RoleTypeId role = player.Role.Type;

        Timing.CallDelayed(0.1f, () =>
        {
            if (player is null || !player.IsConnected || !player.IsAlive || player.Role.Type != role)
                return;

            List<ItemType> items = player.Items
                .Where(item => item is not null)
                .Select(item => item.Type)
                .ToList();
            Dictionary<AmmoType, ushort> ammo = Enum.GetValues(typeof(AmmoType))
                .Cast<AmmoType>()
                .Where(ammoType => ammoType != AmmoType.None)
                .ToDictionary(ammoType => ammoType, player.GetAmmo);

            SpawnInventories[player] = new SpawnInventorySnapshot(role, items, ammo);
        });
    }

    private static void OnPlayerLeft(LeftEventArgs ev)
    {
        Player? player = ev?.Player;
        if (player is null)
            return;

        Pending.Remove(player);
        AppliedAssignmentStamps.Remove(player);
        SpawnInventories.Remove(player);
    }

    private static IEnumerator<float> WatchAssignments()
    {
        while (true)
        {
            ReconcileAssignments();
            yield return Timing.WaitForSeconds(PollIntervalSeconds);
        }
    }

    private static void ReconcileAssignments()
    {
        SubclassManager? manager = SubclassManager.Instance;
        if (manager is null)
        {
            Pending.Clear();
            AppliedAssignmentStamps.Clear();
            return;
        }

        List<Player> alivePlayers = Player.List.Where(player => player is not null && player.IsAlive).ToList();
        var aliveSet = new HashSet<Player>(alivePlayers);

        foreach (Player stalePlayer in Pending.Keys.Where(player => !aliveSet.Contains(player)).ToList())
            Pending.Remove(stalePlayer);

        foreach (Player stalePlayer in AppliedAssignmentStamps.Keys.Where(player => !aliveSet.Contains(player)).ToList())
            AppliedAssignmentStamps.Remove(stalePlayer);

        foreach (Player player in alivePlayers)
        {
            SubclassFramework.Core.Subclass? subclass = manager.GetSubclass(player);
            if (subclass is null)
            {
                Pending.Remove(player);
                if (AppliedAssignmentStamps.Remove(player))
                    RestoreSpawnInventory(player);

                continue;
            }

            Loadouts.TryGetValue(subclass.Id, out CompiledLoadout? loadout);
            int stamp = manager.GetAssignmentStamp(player);
            if (stamp == 0 || AppliedAssignmentStamps.TryGetValue(player, out int appliedStamp) && appliedStamp == stamp)
                continue;

            if (!Pending.TryGetValue(player, out PendingAssignment? pending) ||
                pending.Stamp != stamp ||
                !string.Equals(pending.SubclassId, subclass.Id, StringComparison.OrdinalIgnoreCase))
            {
                Pending[player] = new PendingAssignment(subclass.Id, stamp, Time.time);
                continue;
            }

            if (Time.time - pending.FirstObservedAt < AssignmentSettleSeconds)
                continue;

            if (!IsCurrentAssignment(manager, player, subclass.Id, stamp))
            {
                Pending.Remove(player);
                continue;
            }

            if (loadout is not null && !CustomItemsAreAvailable(loadout, player, stamp))
                continue;

            if (!ApplyLoadout(player, subclass, loadout, stamp, pending))
                continue;

            AppliedAssignmentStamps[player] = stamp;
            Pending.Remove(player);
            if (loadout is not null && loadout.CustomItemIds.Count > 0)
                Log.Debug($"Reconciled inventory for subclass '{subclass.Id}' to {player.Nickname}.");
        }
    }

    private static bool IsCurrentAssignment(SubclassManager manager, Player player, string subclassId, int stamp)
    {
        SubclassFramework.Core.Subclass? current = manager.GetSubclass(player);
        return player.IsAlive &&
               manager.GetAssignmentStamp(player) == stamp &&
               current is not null &&
               string.Equals(current.Id, subclassId, StringComparison.OrdinalIgnoreCase);
    }

    private static bool CustomItemsAreAvailable(CompiledLoadout loadout, Player player, int stamp)
    {
        foreach (uint customItemId in loadout.CustomItemIds)
        {
            List<CustomItem> matches = CustomItem.Registered
                .Where(item => item.Id == customItemId)
                .ToList();

            if (matches.Count == 1)
                continue;

            string warningKey = $"{player.UserId}:{stamp}:{customItemId}";
            if (MissingCustomItemWarnings.Add(warningKey))
            {
                string reason = matches.Count == 0
                    ? "has not registered yet"
                    : $"is ambiguous because {matches.Count} registered items use that ID";
                Log.Warn($"Subclass inventory override for '{player.Nickname}' is waiting: custom item ID {customItemId} {reason} (assignment {stamp}).");
            }

            return false;
        }

        return true;
    }

    private static bool ApplyLoadout(
        Player player,
        SubclassFramework.Core.Subclass subclass,
        CompiledLoadout? loadout,
        int stamp,
        PendingAssignment pending)
    {
        if (!pending.BaseLoadoutApplied)
        {
            if (loadout is null || !loadout.ReplaceInventory)
            {
                if (!ApplyFrameworkLoadout(player, subclass, stamp))
                    return false;
            }
            else
            {
                ClearInventoryAndAmmo(player);
            }

            if (loadout is not null)
            {
                foreach (ItemType itemType in loadout.Items)
                    player.AddItem(itemType);

                foreach (KeyValuePair<AmmoType, ushort> ammo in loadout.Ammo)
                    player.SetAmmo(ammo.Key, ammo.Value);
            }

            pending.BaseLoadoutApplied = true;
        }

        if (loadout is null)
            return true;

        if (Time.time < pending.NextCustomItemRetryAt)
            return false;

        foreach (uint customItemId in loadout.CustomItemIds)
        {
            if (pending.GivenCustomItemIds.Contains(customItemId))
                continue;

            string warningKey = $"{player.UserId}:{stamp}:{customItemId}";
            if (!CustomItem.TryGive(player, customItemId, displayMessage: false))
            {
                if (FailedCustomItemWarnings.Add(warningKey))
                    Log.Error($"Failed to give configured custom item ID {customItemId} to {player.Nickname} for subclass '{subclass.Id}' (assignment {stamp}); it will be retried.");

                pending.NextCustomItemRetryAt = Time.time + 1f;
                return false;
            }

            pending.GivenCustomItemIds.Add(customItemId);
            FailedCustomItemWarnings.Remove(warningKey);
        }

        return true;
    }

    private static bool ApplyFrameworkLoadout(
        Player player,
        SubclassFramework.Core.Subclass subclass,
        int stamp)
    {
        List<ItemType> items;

        if (subclass.OverrideInventory)
        {
            items = (subclass.Items ?? Array.Empty<ItemType>()).ToList();
        }
        else if (SpawnInventories.TryGetValue(player, out SpawnInventorySnapshot? snapshot) &&
                 snapshot.Role == player.Role.Type)
        {
            items = new List<ItemType>(snapshot.Items);
        }
        else
        {
            string warningKey = $"{player.UserId}:{stamp}:{player.Role.Type}";
            if (MissingSpawnInventoryWarnings.Add(warningKey))
                Log.Error($"Cannot rebuild inventory for subclass '{subclass.Id}' assigned to {player.Nickname}: no spawn inventory snapshot exists for role '{player.Role.Type}'. The current inventory was left untouched.");

            return false;
        }

        ClearInventoryAndAmmo(player);

        foreach (ItemType itemType in items)
            player.AddItem(itemType);

        foreach (ItemType itemType in subclass.ExtraItems ?? Array.Empty<ItemType>())
            player.AddItem(itemType);

        foreach (KeyValuePair<AmmoType, ushort> ammo in subclass.Ammo ?? Array.Empty<KeyValuePair<AmmoType, ushort>>())
            player.SetAmmo(ammo.Key, ammo.Value);

        AddAssignmentGrantedItems(player, subclass);
        return true;
    }

    private static void RestoreSpawnInventory(Player player)
    {
        if (!SpawnInventories.TryGetValue(player, out SpawnInventorySnapshot? snapshot) ||
            snapshot.Role != player.Role.Type)
        {
            string warningKey = $"{player.UserId}:{player.Role.Type}:unassigned";
            if (MissingSpawnInventoryWarnings.Add(warningKey))
                Log.Error($"Cannot restore {player.Nickname}'s role inventory after subclass removal: no spawn inventory snapshot exists for role '{player.Role.Type}'.");

            return;
        }

        ClearInventoryAndAmmo(player);

        foreach (ItemType itemType in snapshot.Items)
            player.AddItem(itemType);

        foreach (KeyValuePair<AmmoType, ushort> ammo in snapshot.Ammo)
            player.SetAmmo(ammo.Key, ammo.Value);
    }

    private static void ClearInventoryAndAmmo(Player player)
    {
        player.ClearInventory(true);
        foreach (AmmoType ammoType in Enum.GetValues(typeof(AmmoType)))
        {
            if (ammoType != AmmoType.None)
                player.SetAmmo(ammoType, 0);
        }
    }

    private static void AddAssignmentGrantedItems(Player player, SubclassFramework.Core.Subclass subclass)
    {
        if (string.Equals(subclass.Id, "demoledor", StringComparison.OrdinalIgnoreCase))
        {
            player.AddItem((int)player.Role.Team == 1
                ? ItemType.KeycardMTFOperative
                : ItemType.KeycardChaosInsurgency);
        }
        else if (string.Equals(subclass.Id, "infiltrado", StringComparison.OrdinalIgnoreCase) &&
                 (int)player.Role.Team == 2)
        {
            player.AddItem(ItemType.KeycardMTFCaptain);
        }
    }

    private static bool TryCompile(LoadoutDefinition definition, out CompiledLoadout? compiled)
    {
        bool valid = true;
        var itemTypes = new List<ItemType>();
        var customItemIds = new List<uint>();
        var ammo = new Dictionary<AmmoType, ushort>();

        foreach (string? itemName in definition.Items ?? new List<string>())
        {
            if (string.IsNullOrWhiteSpace(itemName))
            {
                Log.Error($"An empty item entry was found in subclass '{definition.SubclassId}'.");
                valid = false;
                continue;
            }

            if (itemName.StartsWith("custom:", StringComparison.OrdinalIgnoreCase))
            {
                string idText = itemName.Substring("custom:".Length);
                if (uint.TryParse(idText, out uint customItemId))
                    customItemIds.Add(customItemId);
                else
                {
                    Log.Error($"Invalid custom item entry '{itemName}' in subclass '{definition.SubclassId}'. Use custom:<id>.");
                    valid = false;
                }

                continue;
            }

            if (Enum.TryParse(itemName, ignoreCase: true, out ItemType itemType) &&
                Enum.IsDefined(typeof(ItemType), itemType))
            {
                itemTypes.Add(itemType);
            }
            else
            {
                Log.Error($"Invalid ItemType '{itemName}' in subclass '{definition.SubclassId}'.");
                valid = false;
            }
        }

        foreach (KeyValuePair<string, ushort> ammoEntry in definition.Ammo ?? new Dictionary<string, ushort>())
        {
            if (Enum.TryParse(ammoEntry.Key, ignoreCase: true, out AmmoType ammoType) &&
                Enum.IsDefined(typeof(AmmoType), ammoType))
            {
                ammo[ammoType] = ammoEntry.Value;
            }
            else
            {
                Log.Error($"Invalid AmmoType '{ammoEntry.Key}' in subclass '{definition.SubclassId}'.");
                valid = false;
            }
        }

        if (!valid)
        {
            compiled = null;
            return false;
        }

        compiled = new CompiledLoadout(definition.ReplaceInventory, itemTypes, customItemIds, ammo);
        return true;
    }

    private sealed class CompiledLoadout
    {
        public CompiledLoadout(
            bool replaceInventory,
            IReadOnlyList<ItemType> items,
            IReadOnlyList<uint> customItemIds,
            IReadOnlyDictionary<AmmoType, ushort> ammo)
        {
            ReplaceInventory = replaceInventory;
            Items = items;
            CustomItemIds = customItemIds;
            Ammo = ammo;
        }

        public bool ReplaceInventory { get; }

        public IReadOnlyList<ItemType> Items { get; }

        public IReadOnlyList<uint> CustomItemIds { get; }

        public IReadOnlyDictionary<AmmoType, ushort> Ammo { get; }
    }

    private sealed class SpawnInventorySnapshot
    {
        public SpawnInventorySnapshot(
            RoleTypeId role,
            IReadOnlyList<ItemType> items,
            IReadOnlyDictionary<AmmoType, ushort> ammo)
        {
            Role = role;
            Items = items;
            Ammo = ammo;
        }

        public RoleTypeId Role { get; }

        public IReadOnlyList<ItemType> Items { get; }

        public IReadOnlyDictionary<AmmoType, ushort> Ammo { get; }
    }

    private sealed class PendingAssignment
    {
        public PendingAssignment(string subclassId, int stamp, float firstObservedAt)
        {
            SubclassId = subclassId;
            Stamp = stamp;
            FirstObservedAt = firstObservedAt;
        }

        public string SubclassId { get; }

        public int Stamp { get; }

        public float FirstObservedAt { get; }

        public bool BaseLoadoutApplied { get; set; }

        public HashSet<uint> GivenCustomItemIds { get; } = new();

        public float NextCustomItemRetryAt { get; set; }
    }
}

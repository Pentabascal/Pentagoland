using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomItemSpawnManager;

internal static class SpawnService
{
    public static void Apply(Config config)
    {
        if (config.ManagedAssemblies is null || (!config.UseSourceCodeSpawnCatalog && config.Items is null))
        {
            Log.Error("Spawn manager configuration is invalid: managed_assemblies and the selected item catalog must not be null.");
            return;
        }

        List<ItemSpawnOverride> itemOverrides = config.UseSourceCodeSpawnCatalog
            ? Config.CreateSourceCodeSpawnCatalog()
            : config.Items;

        var managedAssemblies = new HashSet<string>(
            config.ManagedAssemblies.Where(name => !string.IsNullOrWhiteSpace(name)),
            StringComparer.OrdinalIgnoreCase);

        if (managedAssemblies.Count == 0)
        {
            Log.Error("Spawn manager has no managed assemblies; no custom-item spawn settings were applied.");
            return;
        }

        List<CustomItem> managedItems = CustomItem.Registered
            .Where(item => item is not null && managedAssemblies.Contains(item.GetType().Assembly.GetName().Name))
            .OrderBy(item => item.GetType().Assembly.GetName().Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Id)
            .ToList();

        if (managedItems.Count == 0)
        {
            Log.Error($"No registered custom items were found in the managed assemblies: {string.Join(", ", managedAssemblies)}.");
            return;
        }

        var byKey = managedItems
            .GroupBy(item => MakeKey(item.GetType().Assembly.GetName().Name, item.Id), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);

        var seenOverrides = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var duplicateOverrides = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (ItemSpawnOverride? itemOverride in itemOverrides)
        {
            if (itemOverride is null || string.IsNullOrWhiteSpace(itemOverride.AssemblyName))
            {
                Log.Error("A custom-item spawn entry has no assembly_name and will be ignored.");
                continue;
            }

            string key = MakeKey(itemOverride.AssemblyName, itemOverride.ItemId);
            if (duplicateOverrides.Contains(key))
                continue;

            if (!seenOverrides.Add(key))
            {
                duplicateOverrides.Add(key);
                Log.Error($"Duplicate spawn configuration for '{key}'; that item will have no map spawns.");
            }
        }

        var assignedRooms = new Dictionary<RoomType, string>();
        var configuredKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (ItemSpawnOverride itemOverride in itemOverrides.Where(entry => entry is not null))
        {
            string key = MakeKey(itemOverride.AssemblyName, itemOverride.ItemId);
            if (!configuredKeys.Add(key))
                continue;

            if (!byKey.TryGetValue(key, out List<CustomItem>? matches))
            {
                if (config.Debug)
                    Log.Warn($"Configured custom item '{key}' is not registered; its entry is waiting for that plugin.");
                continue;
            }

            if (duplicateOverrides.Contains(key) || matches.Count != 1)
            {
                foreach (CustomItem item in matches)
                    item.SpawnProperties = EmptySpawnProperties();

                if (matches.Count != 1)
                    Log.Error($"More than one registered custom item matches '{key}'; all matches were disabled for round-start map spawning.");
                continue;
            }

            ApplyOverride(matches[0], itemOverride, assignedRooms, config.EnforceOneCustomItemPerRoom);
        }

        foreach (KeyValuePair<string, List<CustomItem>> entry in byKey)
        {
            if (configuredKeys.Contains(entry.Key))
                continue;

            if (config.DisableUnconfiguredItems)
            {
                foreach (CustomItem item in entry.Value)
                    item.SpawnProperties = EmptySpawnProperties();
            }
            else if (config.EnforceOneCustomItemPerRoom)
            {
                Log.Warn($"'{entry.Key}' retains its original spawn points because disable_unconfigured_items is false; the one-item-per-room guarantee cannot cover it.");
            }
        }

        string catalogSource = config.UseSourceCodeSpawnCatalog ? "source code" : "EXILED YAML";
        Log.Info($"Applied round-start spawn settings from {catalogSource} to {managedItems.Count} custom item(s) from {managedAssemblies.Count} managed assembly/assemblies.");
    }

    private static void ApplyOverride(
        CustomItem item,
        ItemSpawnOverride itemOverride,
        Dictionary<RoomType, string> assignedRooms,
        bool enforceOnePerRoom)
    {
        var dynamicPoints = new List<DynamicSpawnPoint>();
        var rolePoints = new List<RoleSpawnPoint>();
        var roomPoints = new List<RoomSpawnPoint>();
        var staticPoints = new List<StaticSpawnPoint>();
        var lockerPoints = new List<LockerSpawnPoint>();

        if (itemOverride.Enabled)
        {
            foreach (DynamicSpawnPoint? point in itemOverride.DynamicSpawnPoints ?? new List<DynamicSpawnPoint>())
            {
                if (point is null || !IsValidChance(point.Chance))
                {
                    Log.Error($"Custom item {item.Id}: a dynamic spawn point has an invalid chance.");
                    continue;
                }

                var locationField = typeof(SpawnLocationType).GetField(point.Location.ToString());
                if (!Enum.IsDefined(typeof(SpawnLocationType), point.Location) ||
                    locationField is null ||
                    Attribute.GetCustomAttribute(locationField, typeof(ObsoleteAttribute)) is ObsoleteAttribute)
                {
                    Log.Warn($"Custom item {item.Id}: dynamic location '{point.Location}' is invalid or retired and was skipped.");
                    continue;
                }

                Room room = Room.Get(point.Position);
                if (room is null)
                {
                    Log.Warn($"Custom item {item.Id}: dynamic location '{point.Location}' did not resolve to a room and was skipped.");
                    continue;
                }

                if (!TryAssignRoom(room.Type, item.Id, assignedRooms, enforceOnePerRoom))
                    continue;

                dynamicPoints.Add(new DynamicSpawnPoint
                {
                    Location = point.Location,
                    Chance = point.Chance,
                });
            }

            foreach (RoleSpawnPoint? point in itemOverride.RoleSpawnPoints ?? new List<RoleSpawnPoint>())
            {
                if (point is null || !IsValidChance(point.Chance))
                {
                    Log.Error($"Custom item {item.Id}: a role spawn point has an invalid chance.");
                    continue;
                }

                if (!TryGetFixedScpRoom(point.Role, out RoomType roomType))
                {
                    Log.Warn($"Custom item {item.Id}: role spawn point '{point.Role}' is not a fixed SCP containment room and was skipped.");
                    continue;
                }

                if (Room.Get(roomType) is null)
                {
                    Log.Warn($"Custom item {item.Id}: role spawn room '{roomType}' is absent from this map and was skipped.");
                    continue;
                }

                if (!TryAssignRoom(roomType, item.Id, assignedRooms, enforceOnePerRoom))
                    continue;

                rolePoints.Add(new RoleSpawnPoint
                {
                    Role = point.Role,
                    Chance = point.Chance,
                });
            }

            foreach (RoomSpawnPoint? point in itemOverride.RoomSpawnPoints ?? new List<RoomSpawnPoint>())
            {
                if (point is null || !Enum.IsDefined(typeof(RoomType), point.Room) ||
                    point.Room == RoomType.Unknown || !IsValidChance(point.Chance) || !IsFinite(point.Offset))
                {
                    Log.Error($"Custom item {item.Id}: a room spawn point has an invalid room, chance, or offset.");
                    continue;
                }

                if (Room.Get(point.Room) is null)
                {
                    Log.Warn($"Custom item {item.Id}: room '{point.Room}' is absent from this map and was skipped.");
                    continue;
                }

                if (!TryAssignRoom(point.Room, item.Id, assignedRooms, enforceOnePerRoom))
                    continue;

                roomPoints.Add(new RoomSpawnPoint
                {
                    Room = point.Room,
                    Offset = point.Offset,
                    Chance = point.Chance,
                });
            }

            foreach (StaticCustomItemSpawnPoint? point in itemOverride.StaticSpawnPoints ?? new List<StaticCustomItemSpawnPoint>())
            {
                if (point is null || !IsValidChance(point.Chance) || !IsFinite(point.Position))
                {
                    Log.Error($"Custom item {item.Id}: a static spawn point has an invalid chance or position.");
                    continue;
                }

                Room room = Room.Get(point.Position);
                if (room is not null && !TryAssignRoom(room.Type, item.Id, assignedRooms, enforceOnePerRoom))
                    continue;

                staticPoints.Add(new StaticSpawnPoint
                {
                    Name = point.Name,
                    Chance = point.Chance,
                    Position = point.Position,
                });
            }

            foreach (LockerCustomItemSpawnPoint? point in itemOverride.LockerSpawnPoints ?? new List<LockerCustomItemSpawnPoint>())
            {
                if (point is null || !IsValidChance(point.Chance) || !IsFinite(point.Offset))
                {
                    Log.Error($"Custom item {item.Id}: a locker spawn point has an invalid chance or offset.");
                    continue;
                }

                lockerPoints.Add(new LockerSpawnPoint
                {
                    Zone = point.Zone,
                    UseChamber = point.UseChamber,
                    Offset = point.Offset,
                    Type = point.Type,
                    Chance = point.Chance,
                });
            }
        }

        item.SpawnProperties = new SpawnProperties
        {
            Limit = itemOverride.SpawnLimit,
            DynamicSpawnPoints = dynamicPoints,
            RoleSpawnPoints = rolePoints,
            RoomSpawnPoints = roomPoints,
            StaticSpawnPoints = staticPoints,
            LockerSpawnPoints = lockerPoints,
        };

        if (itemOverride.Enabled &&
            dynamicPoints.Count + rolePoints.Count + roomPoints.Count + staticPoints.Count + lockerPoints.Count == 0)
            Log.Warn($"Custom item {item.Id} is enabled but has no valid spawn points on this map.");
    }

    private static bool TryAssignRoom(
        RoomType room,
        uint itemId,
        Dictionary<RoomType, string> assignedRooms,
        bool enforceOnePerRoom)
    {
        if (room == RoomType.Unknown || !Enum.IsDefined(typeof(RoomType), room))
        {
            Log.Error($"Custom item {itemId}: a spawn point resolved to an unknown room and was skipped.");
            return false;
        }

        if (!enforceOnePerRoom)
            return true;

        if (assignedRooms.TryGetValue(room, out string? firstItem))
        {
            Log.Warn($"Custom item {itemId}: room '{room}' is already assigned to {firstItem}; this duplicate point was skipped.");
            return false;
        }

        assignedRooms.Add(room, itemId.ToString());
        return true;
    }

    private static bool TryGetFixedScpRoom(RoleTypeId role, out RoomType room)
    {
        room = role switch
        {
            RoleTypeId.Scp049 or RoleTypeId.Scp173 => RoomType.Hcz049,
            RoleTypeId.Scp079 => RoomType.Hcz079,
            RoleTypeId.Scp096 => RoomType.Hcz096,
            RoleTypeId.Scp106 => RoomType.Hcz106,
            RoleTypeId.Scp939 => RoomType.Hcz939,
            _ => RoomType.Unknown,
        };

        return room != RoomType.Unknown;
    }

    private static bool IsValidChance(float chance)
    {
        return !float.IsNaN(chance) && !float.IsInfinity(chance) && chance >= 0f && chance <= 100f;
    }

    private static bool IsFinite(Vector3 value)
    {
        return IsFinite(value.x) && IsFinite(value.y) && IsFinite(value.z);
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private static SpawnProperties EmptySpawnProperties()
    {
        return new SpawnProperties
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(),
            RoleSpawnPoints = new List<RoleSpawnPoint>(),
            RoomSpawnPoints = new List<RoomSpawnPoint>(),
            StaticSpawnPoints = new List<StaticSpawnPoint>(),
            LockerSpawnPoints = new List<LockerSpawnPoint>(),
        };
    }

    private static string MakeKey(string assemblyName, uint itemId)
    {
        return $"{assemblyName.Trim()}:{itemId}";
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.API.Interfaces;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomItemSpawnManager;

public sealed class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;

    public bool Debug { get; set; }

    [Description("When enabled, only the first configured VVUP custom-item point for a room type is kept. Disable to allow multiple custom-item opportunities in the same room.")]
    public bool EnforceOneCustomItemPerRoom { get; set; } = false;

    [Description("When enabled, VVUP custom items missing from Items have all round-start spawn points disabled. Disabled by default so source-configured spawn points remain active.")]
    public bool DisableUnconfiguredItems { get; set; } = false;

    [Description("When enabled, the source-level layout in Config.cs replaces the Items list in this YAML file.")]
    public bool UseSourceCodeSpawnCatalog { get; set; } = true;

    [Description("Exact assembly names whose custom items are managed. Other plugins are untouched.")]
    public List<string> ManagedAssemblies { get; set; } = new()
    {
        "VVUP.CustomItems",
    };

    [Description("Per-item round-start spawn settings. The order determines which item keeps a room if points conflict.")]
    public List<ItemSpawnOverride> Items { get; set; } = CreateSourceCodeSpawnCatalog();

    // Edit this source catalog for full C# control; room points use named RoomType values.
    public static List<ItemSpawnOverride> CreateSourceCodeSpawnCatalog()
    {
        return new List<ItemSpawnOverride>
        {
            DynamicItem(20,
                Dynamic(SpawnLocationType.InsideLczArmory, 20),
                Dynamic(SpawnLocationType.InsideLczCafe, 10),
                Dynamic(SpawnLocationType.InsideLczWc, 10),
                Dynamic(SpawnLocationType.Inside173Armory, 10)),
            DynamicItem(21,
                Dynamic(SpawnLocationType.InsideGateA, 15),
                Dynamic(SpawnLocationType.InsideGateB, 15),
                Dynamic(SpawnLocationType.InsideEscapePrimary, 10),
                Dynamic(SpawnLocationType.InsideEscapeSecondary, 10)),
            DynamicItem(22,
                Dynamic(SpawnLocationType.Inside049Armory, 15),
                Dynamic(SpawnLocationType.InsideHczArmory, 10),
                Dynamic(SpawnLocationType.Inside079Armory, 10)),
            DynamicAndRoomItem(23, 1,
                new[]
                {
                    Dynamic(SpawnLocationType.InsideEscapePrimary, 20),
                    Dynamic(SpawnLocationType.InsideEscapeSecondary, 10),
                },
                Array.Empty<RoleSpawnPoint>(),
                new[] { Room(RoomType.EzShelter, 15, new Vector3(0f, 1f, 0f)) }),
            DynamicAndRoomItem(24, 1,
                new[]
                {
                    Dynamic(SpawnLocationType.InsideHidChamber, 5),
                    Dynamic(SpawnLocationType.InsideGr18, 5),
                    Dynamic(SpawnLocationType.InsideHidLab, 5),
                },
                new[] { Role(RoleTypeId.Scp106, 10) },
                new[]
                {
                    Room(RoomType.HczTestRoom, 5, new Vector3(0.885f, 0.749f, -4.874f)),
                    Room(RoomType.Hcz096, 5, new Vector3(-3.25f, 1f, -0.78f)),
                }),
            DynamicAndRoomItem(26, 1,
                new[]
                {
                    Dynamic(SpawnLocationType.Inside939Cryo, 15),
                    Dynamic(SpawnLocationType.Inside096, 5),
                },
                Array.Empty<RoleSpawnPoint>(),
                new[] { Room(RoomType.Hcz096, 10, new Vector3(-3.25f, 1f, -0.78f)) }),
            DynamicItem(25,
                Dynamic(SpawnLocationType.Inside127Lab, 20),
                Dynamic(SpawnLocationType.InsideHczArmory, 5),
                Dynamic(SpawnLocationType.Inside049Armory, 5)),
            DynamicItem(27,
                Dynamic(SpawnLocationType.Inside079First, 15),
                Dynamic(SpawnLocationType.Inside079Secondary, 10),
                Dynamic(SpawnLocationType.Inside079Armory, 5)),
            DynamicItem(28,
                Dynamic(SpawnLocationType.Inside914, 25),
                Dynamic(SpawnLocationType.InsideLczCafe, 5),
                Dynamic(SpawnLocationType.InsideLczWc, 5)),
            DynamicWithStaticItem(29,
                new[]
                {
                    Dynamic(SpawnLocationType.InsideHidChamber, 5),
                    Dynamic(SpawnLocationType.Inside079Secondary, 5),
                    Dynamic(SpawnLocationType.InsideGateA, 5),
                },
                new[] { Static("Edificio de la sala de escape", 10, new Vector3(133f, 289f, 24f)) }),
            DynamicItem(32,
                1,
                Dynamic(SpawnLocationType.InsideLczArmory, 5),
                Dynamic(SpawnLocationType.InsideHczArmory, 8),
                Dynamic(SpawnLocationType.Inside049Armory, 5),
                Dynamic(SpawnLocationType.Inside079Armory, 5),
                Dynamic(SpawnLocationType.InsideSurfaceNuke, 10)),
            DynamicItem(33,
                Dynamic(SpawnLocationType.InsideHidChamber, 8),
                Dynamic(SpawnLocationType.Inside079Armory, 8),
                Dynamic(SpawnLocationType.InsideHidLab, 5)),
            DynamicAndRoomItem(34, 1,
                new[] { Dynamic(SpawnLocationType.Inside939Cryo, 10) },
                Array.Empty<RoleSpawnPoint>(),
                new[] { Room(RoomType.HczTestRoom, 15, new Vector3(0.885f, 0.749f, -4.874f)) },
                new[] { Locker(ZoneType.Unspecified, LockerType.Misc, true, Vector3.zero, 10) }),
            DynamicItem(36,
                1,
                Dynamic(SpawnLocationType.InsideLczArmory, 8),
                Dynamic(SpawnLocationType.InsideHczArmory, 8),
                Dynamic(SpawnLocationType.Inside049Armory, 8),
                Dynamic(SpawnLocationType.InsideSurfaceNuke, 8),
                Dynamic(SpawnLocationType.Inside079Armory, 8)),
            DynamicItem(30,
                Dynamic(SpawnLocationType.Inside939Cryo, 15),
                Dynamic(SpawnLocationType.Inside096, 5),
                Dynamic(SpawnLocationType.Inside106Primary, 5)),
            DynamicItem(31,
                Dynamic(SpawnLocationType.Inside096, 15),
                Dynamic(SpawnLocationType.InsideHczArmory, 5),
                Dynamic(SpawnLocationType.InsideGr18Glass, 5)),
            DynamicItem(37,
                Dynamic(SpawnLocationType.Inside106Primary, 15),
                Dynamic(SpawnLocationType.Inside106Secondary, 10),
                Dynamic(SpawnLocationType.InsideHczArmory, 5)),
            DynamicItem(38,
                Dynamic(SpawnLocationType.InsideSurfaceNuke, 10),
                Dynamic(SpawnLocationType.InsideHczArmory, 5),
                Dynamic(SpawnLocationType.Inside079Armory, 5)),
            DynamicItem(39,
                Dynamic(SpawnLocationType.Inside173Armory, 10),
                Dynamic(SpawnLocationType.InsideGr18Glass, 10),
                Dynamic(SpawnLocationType.InsideGr18, 5),
                Dynamic(SpawnLocationType.Inside173Connector, 5)),
            DynamicItem(41,
                Dynamic(SpawnLocationType.InsideHidChamber, 10),
                Dynamic(SpawnLocationType.InsideHidLab, 5)),
            DynamicItem(42,
                Dynamic(SpawnLocationType.InsideLczCafe, 20),
                Dynamic(SpawnLocationType.InsideLczWc, 8),
                Dynamic(SpawnLocationType.InsideLczArmory, 5)),
            DynamicItem(43,
                Dynamic(SpawnLocationType.InsideLczWc, 18),
                Dynamic(SpawnLocationType.InsideLczCafe, 8),
                Dynamic(SpawnLocationType.Inside173Armory, 5)),
            DynamicItem(46,
                Dynamic(SpawnLocationType.InsideHczArmory, 12),
                Dynamic(SpawnLocationType.Inside049Armory, 8),
                Dynamic(SpawnLocationType.Inside079Armory, 8)),
            DynamicItem(47,
                Dynamic(SpawnLocationType.InsideIntercom, 15),
                Dynamic(SpawnLocationType.InsideEscapePrimary, 5),
                Dynamic(SpawnLocationType.InsideEscapeSecondary, 5)),
            DynamicItem(50,
                Dynamic(SpawnLocationType.Inside079Armory, 8),
                Dynamic(SpawnLocationType.Inside079First, 5),
                Dynamic(SpawnLocationType.Inside079Secondary, 5)),
            DynamicItem(51,
                Dynamic(SpawnLocationType.Inside330, 18),
                Dynamic(SpawnLocationType.Inside330Chamber, 8),
                Dynamic(SpawnLocationType.Inside173Armory, 5)),
            DynamicItem(54,
                1,
                Dynamic(SpawnLocationType.InsideLczArmory, 5),
                Dynamic(SpawnLocationType.InsideHczArmory, 8),
                Dynamic(SpawnLocationType.Inside049Armory, 5),
                Dynamic(SpawnLocationType.Inside079Armory, 5),
                Dynamic(SpawnLocationType.InsideSurfaceNuke, 10)),
            DynamicItem(55,
                Dynamic(SpawnLocationType.InsideLczArmory, 8),
                Dynamic(SpawnLocationType.InsideHczArmory, 12),
                Dynamic(SpawnLocationType.InsideLczCafe, 5)),
            RoomItem(53, Room(RoomType.EzChef, 20, new Vector3(0f, 1f, 0f))),
            RoomItem(56,
                Room(RoomType.HczServerRoom, 15, new Vector3(0f, 1f, 0f)),
                Room(RoomType.HczTestRoom, 10, new Vector3(0f, 1f, 0f))),
            RoomItem(57, Room(RoomType.LczPlants, 20, new Vector3(0f, 1f, 0f))),
            RoomItem(58, Room(RoomType.HczLoadingBay, 15, new Vector3(0f, 1f, 0f))),
            LockerItem(59,
                Locker(ZoneType.Unspecified, LockerType.Scp018Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp207Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp244Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp268Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp500Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp1576Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp1344Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp1853Pedestal, true, Vector3.zero, 10),
                Locker(ZoneType.Unspecified, LockerType.Scp2176Pedestal, true, Vector3.zero, 10)),
            RoomItem(61, Room(RoomType.HczNuke, 15, new Vector3(0f, 1f, 0f))),
            DisabledItem(40),
        };
    }

    private static ItemSpawnOverride DynamicItem(uint itemId, uint limit, params DynamicSpawnPoint[] points)
    {
        ItemSpawnOverride item = DynamicItem(itemId, points);
        item.SpawnLimit = limit;
        return item;
    }

    private static ItemSpawnOverride DynamicItem(uint itemId, params DynamicSpawnPoint[] points)
    {
        return new ItemSpawnOverride
        {
            AssemblyName = "VVUP.CustomItems",
            ItemId = itemId,
            Enabled = true,
            SpawnLimit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(points),
        };
    }

    private static ItemSpawnOverride DynamicWithStaticItem(
        uint itemId,
        DynamicSpawnPoint[] dynamicPoints,
        StaticCustomItemSpawnPoint[] staticPoints)
    {
        return new ItemSpawnOverride
        {
            AssemblyName = "VVUP.CustomItems",
            ItemId = itemId,
            Enabled = true,
            SpawnLimit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(dynamicPoints),
            StaticSpawnPoints = new List<StaticCustomItemSpawnPoint>(staticPoints),
        };
    }

    private static ItemSpawnOverride DynamicAndRoomItem(
        uint itemId,
        uint limit,
        DynamicSpawnPoint[] dynamicPoints,
        RoleSpawnPoint[] rolePoints,
        RoomSpawnPoint[] roomPoints,
        LockerCustomItemSpawnPoint[]? lockerPoints = null)
    {
        return new ItemSpawnOverride
        {
            AssemblyName = "VVUP.CustomItems",
            ItemId = itemId,
            Enabled = true,
            SpawnLimit = limit,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>(dynamicPoints),
            RoleSpawnPoints = new List<RoleSpawnPoint>(rolePoints),
            RoomSpawnPoints = new List<RoomSpawnPoint>(roomPoints),
            LockerSpawnPoints = lockerPoints is null
                ? new List<LockerCustomItemSpawnPoint>()
                : new List<LockerCustomItemSpawnPoint>(lockerPoints),
        };
    }

    private static ItemSpawnOverride LockerItem(uint itemId, params LockerCustomItemSpawnPoint[] points)
    {
        return new ItemSpawnOverride
        {
            AssemblyName = "VVUP.CustomItems",
            ItemId = itemId,
            Enabled = true,
            SpawnLimit = 1,
            LockerSpawnPoints = new List<LockerCustomItemSpawnPoint>(points),
        };
    }

    private static ItemSpawnOverride DisabledItem(uint itemId)
    {
        return new ItemSpawnOverride
        {
            AssemblyName = "VVUP.CustomItems",
            ItemId = itemId,
            Enabled = false,
            SpawnLimit = 1,
        };
    }

    private static DynamicSpawnPoint Dynamic(SpawnLocationType location, float chance)
    {
        return new DynamicSpawnPoint
        {
            Location = location,
            Chance = chance,
        };
    }

    private static ItemSpawnOverride RoomItem(uint itemId, params RoomSpawnPoint[] points)
    {
        return new ItemSpawnOverride
        {
            AssemblyName = "VVUP.CustomItems",
            ItemId = itemId,
            Enabled = true,
            SpawnLimit = 1,
            RoomSpawnPoints = new List<RoomSpawnPoint>(points),
        };
    }

    private static RoomSpawnPoint Room(RoomType room, float chance, Vector3 offset)
    {
        return new RoomSpawnPoint
        {
            Room = room,
            Chance = chance,
            Offset = offset,
        };
    }

    private static RoleSpawnPoint Role(RoleTypeId role, float chance)
    {
        return new RoleSpawnPoint
        {
            Role = role,
            Chance = chance,
        };
    }

    private static StaticCustomItemSpawnPoint Static(string name, float chance, Vector3 position)
    {
        return new StaticCustomItemSpawnPoint
        {
            Name = name,
            Chance = chance,
            Position = position,
        };
    }

    private static LockerCustomItemSpawnPoint Locker(
        ZoneType zone,
        LockerType type,
        bool useChamber,
        Vector3 offset,
        float chance)
    {
        return new LockerCustomItemSpawnPoint
        {
            Zone = zone,
            Type = type,
            UseChamber = useChamber,
            Offset = offset,
            Chance = chance,
        };
    }
}

public sealed class ItemSpawnOverride
{
    public string AssemblyName { get; set; } = "VVUP.CustomItems";

    public uint ItemId { get; set; }

    public bool Enabled { get; set; } = true;

    [Description("Maximum number of this item that EXILED can spawn this round. Zero means unlimited.")]
    public uint SpawnLimit { get; set; } = 1;

    [Description("Built-in EXILED dynamic locations. Locations that do not resolve to a room are skipped.")]
    public List<DynamicSpawnPoint> DynamicSpawnPoints { get; set; } = new();

    [Description("Specific room type, room-relative offset, and chance.")]
    public List<RoomSpawnPoint> RoomSpawnPoints { get; set; } = new();

    [Description("Role spawn locations are supported only for SCP roles with a fixed containment room.")]
    public List<RoleSpawnPoint> RoleSpawnPoints { get; set; } = new();

    [Description("Fixed world positions and chance.")]
    public List<StaticCustomItemSpawnPoint> StaticSpawnPoints { get; set; } = new();

    [Description("Locker types, zone, chamber preference, offset, and chance.")]
    public List<LockerCustomItemSpawnPoint> LockerSpawnPoints { get; set; } = new();
}

public sealed class StaticCustomItemSpawnPoint
{
    public string Name { get; set; } = string.Empty;

    public float Chance { get; set; }

    public Vector3 Position { get; set; }
}

public sealed class LockerCustomItemSpawnPoint
{
    public ZoneType Zone { get; set; }

    public bool UseChamber { get; set; }

    public Vector3 Offset { get; set; }

    public LockerType Type { get; set; }

    public float Chance { get; set; }
}

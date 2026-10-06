using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using HarmonyLib;
using MEC;
using UnityEngine;

namespace VVUP.CustomItemSpawnManager;

internal static class NaturalSpawnTracker
{
    private static readonly Dictionary<ushort, TrackedSpawn> TrackedPickups = new();
    private static HashSet<string> managedAssemblies = new(StringComparer.OrdinalIgnoreCase);
    private static bool isRoundActive;

    public static void Initialize(IEnumerable<string>? assemblies)
    {
        managedAssemblies = new HashSet<string>(
            (assemblies ?? Enumerable.Empty<string>()).Where(name => !string.IsNullOrWhiteSpace(name)),
            StringComparer.OrdinalIgnoreCase);
        Reset();
    }

    public static void OnRoundStarted()
    {
        TrackedPickups.Clear();
        isRoundActive = true;
    }

    public static void OnRoundEnded(RoundEndedEventArgs ev)
    {
        Reset();
    }

    public static void OnPickingUpItem(PickingUpItemEventArgs ev)
    {
        if (!ev.IsAllowed)
            return;

        ushort serial = ev.Pickup.Serial;
        Timing.CallDelayed(0.1f, () => RemoveIfDespawned(serial));
    }

    public static void Reset()
    {
        TrackedPickups.Clear();
        isRoundActive = false;
    }

    public static void Record(CustomItem item, Pickup pickup)
    {
        if (!isRoundActive || item is null || pickup is null || !pickup.IsSpawned ||
            !managedAssemblies.Contains(item.GetType().Assembly.GetName().Name ?? string.Empty))
        {
            return;
        }

        TrackedPickups[pickup.Serial] = new TrackedSpawn(pickup, item.Id, item.Name);
    }

    public static void RemoveIfDespawned(ushort serial)
    {
        Pickup? pickup = Pickup.Get(serial);
        if (pickup is null || !pickup.IsSpawned)
            TrackedPickups.Remove(serial);
    }

    public static List<string> GetOverviewLines()
    {
        var lines = new List<string>();

        foreach (KeyValuePair<ushort, TrackedSpawn> entry in TrackedPickups.ToList())
        {
            Pickup? pickup = Pickup.Get(entry.Key);
            if (pickup is null || !ReferenceEquals(pickup, entry.Value.Pickup) ||
                !pickup.IsSpawned ||
                !CustomItem.TryGet(pickup, out CustomItem? item) ||
                item is null || item.Id != entry.Value.ItemId ||
                !managedAssemblies.Contains(item.GetType().Assembly.GetName().Name ?? string.Empty))
            {
                TrackedPickups.Remove(entry.Key);
                continue;
            }

            Room? room = Room.Get(pickup.Position);
            string roomName = room is null || room.Type == RoomType.Unknown
                ? "Unknown room"
                : $"{room.Type} ({room.Name})";
            Vector3 position = pickup.Position;
            lines.Add(
                $"{StripRichText(entry.Value.ItemName)} — {roomName} " +
                $"(X {position.x.ToString("R", CultureInfo.InvariantCulture)}, " +
                $"Y {position.y.ToString("R", CultureInfo.InvariantCulture)}, " +
                $"Z {position.z.ToString("R", CultureInfo.InvariantCulture)})");
        }

        return lines;
    }

    private static string StripRichText(string value)
    {
        return Regex.Replace(value ?? string.Empty, "<[^>]*>", string.Empty);
    }

    private sealed class TrackedSpawn
    {
        public TrackedSpawn(Pickup pickup, uint itemId, string itemName)
        {
            Pickup = pickup;
            ItemId = itemId;
            ItemName = itemName;
        }

        public Pickup Pickup { get; }

        public uint ItemId { get; }

        public string ItemName { get; }
    }
}

[HarmonyPatch(typeof(CustomItem), nameof(CustomItem.SpawnAll))]
internal static class NaturalSpawnPatch
{
    [HarmonyPrefix]
    private static void CaptureExistingPickups(out HashSet<ushort> __state)
    {
        __state = new HashSet<ushort>(Pickup.List.Select(pickup => pickup.Serial));
    }

    [HarmonyPostfix]
    private static void TrackSpawnedPickups(CustomItem __instance, HashSet<ushort> __state)
    {
        foreach (Pickup pickup in Pickup.List.ToList())
        {
            if (__state.Contains(pickup.Serial) ||
                !CustomItem.TryGet(pickup, out CustomItem? item) ||
                item is null ||
                item.GetType() != __instance.GetType() ||
                item.Id != __instance.Id)
            {
                continue;
            }

            NaturalSpawnTracker.Record(item, pickup);
        }
    }
}

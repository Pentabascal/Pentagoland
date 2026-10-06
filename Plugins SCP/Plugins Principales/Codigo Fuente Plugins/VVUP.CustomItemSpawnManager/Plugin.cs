using System;
using Exiled.API.Features;
using HarmonyLib;
using MEC;

namespace VVUP.CustomItemSpawnManager;

public sealed class Plugin : Plugin<Config>
{
    private CoroutineHandle applyHandle;
    private Harmony? harmony;

    public override string Name => "VVUP Custom Item Spawn Manager";

    public override string Prefix => "vvup_custom_item_spawn_manager";

    public override string Author => "Copilot";

    public override Version Version => new(1, 0, 0);

    public override Version RequiredExiledVersion => new(9, 14, 2);

    public override void OnEnabled()
    {
        NaturalSpawnTracker.Initialize(Config.ManagedAssemblies);
        harmony = new Harmony("vvup.customitemspawnmanager");
        harmony.PatchAll();
        Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        Exiled.Events.Handlers.Server.RoundStarted += NaturalSpawnTracker.OnRoundStarted;
        Exiled.Events.Handlers.Server.RoundEnded += NaturalSpawnTracker.OnRoundEnded;
        Exiled.Events.Handlers.Player.PickingUpItem += NaturalSpawnTracker.OnPickingUpItem;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
        Exiled.Events.Handlers.Server.RoundStarted -= NaturalSpawnTracker.OnRoundStarted;
        Exiled.Events.Handlers.Server.RoundEnded -= NaturalSpawnTracker.OnRoundEnded;
        Exiled.Events.Handlers.Player.PickingUpItem -= NaturalSpawnTracker.OnPickingUpItem;
        Timing.KillCoroutines(applyHandle);
        harmony?.UnpatchAll(harmony.Id);
        harmony = null;
        NaturalSpawnTracker.Reset();
        base.OnDisabled();
    }

    private void OnWaitingForPlayers()
    {
        NaturalSpawnTracker.Reset();
        Timing.KillCoroutines(applyHandle);
        applyHandle = Timing.CallDelayed(0.5f, () => SpawnService.Apply(Config));
    }
}

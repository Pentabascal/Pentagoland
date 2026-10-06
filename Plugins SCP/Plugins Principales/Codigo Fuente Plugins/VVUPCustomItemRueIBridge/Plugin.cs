using System;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using MEC;

namespace VVUPCustomItemRueIBridge
{
    public sealed class Plugin : Plugin<Config>
    {
        private const string HarmonyId = "vvup.customitems.ruei.bridge";
        private Harmony _harmony;

        public override string Name => "VVUP Custom Item RueI Bridge";
        public override string Author => "Copilot";
        public override string Prefix => "vvup_custom_item_ruei_bridge";
        public override Version Version => new Version(1, 6, 4);
        public override Version RequiredExiledVersion => new Version(9, 14, 2);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            _harmony = new Harmony(HarmonyId);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Info("EXILED and LabAPI plugin hints use edge-aligned RueI overlays. RueI's own render calls are untouched.");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            HintMessageBridge.ClearAll();
            _harmony?.UnpatchAll(HarmonyId);
            _harmony = null;
            base.OnDisabled();
        }

        private void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Player player = ev.Player;
            RueIAuthenticationGuardPatch.MarkVerified(player.ReferenceHub);
            Timing.CallDelayed(2f, () =>
            {
                if (player != null && player.IsConnected)
                {
                    RueI.API.RueDisplay.Get(player).Update();
                }
            });
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            RueIAuthenticationGuardPatch.Remove(ev.Player.ReferenceHub);
            HintMessageBridge.Clear(ev.Player);
        }
    }
}

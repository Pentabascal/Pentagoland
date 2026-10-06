using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;

namespace VVUP.CustomItems.NotificationPatch
{
    public sealed class Config : Exiled.API.Interfaces.IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; }
    }

    public sealed class Plugin : Plugin<Config>
    {
        private Harmony _harmony;

        public override string Name { get; } = "VVUP Custom Items Notification Patch";
        public override string Author { get; } = "Vicious Vikki";
        public override string Prefix { get; } = "VVUP.CI.Notifications";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(9, 14, 2);

        public override void OnEnabled()
        {
            _harmony = new Harmony("vvup.customitems.notification-patch");
            _harmony.PatchAll(typeof(Plugin).Assembly);
            InfiltradoSpawnHintPatch.Apply(_harmony);
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            _harmony?.UnpatchAll("vvup.customitems.notification-patch");
            _harmony = null;
            base.OnDisabled();
        }
    }
}

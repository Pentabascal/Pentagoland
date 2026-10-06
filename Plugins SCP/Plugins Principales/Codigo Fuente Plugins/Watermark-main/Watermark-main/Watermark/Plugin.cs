using Exiled.API.Features;
using System;
using Exiled.Events.EventArgs.Player;
using MEC;
using RueI.API;
using RueI.API.Elements;
using RueI.API.Elements.Enums;

// Copyright (c) 2026 polyshed

namespace Watermark
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; private set; }

        public override string Name { get; } = "ServerWatermark";
        public override string Author { get; } = "@polyshed.";
        public override Version Version { get; } = new Version(0, 0, 7);
        public override Version RequiredExiledVersion { get; } = new Version(9, 14, 2);

        private static readonly Tag WatermarkTag = new();
        private BasicElement watermark;

        public override void OnEnabled()
        {
            Instance = this;

            if (string.IsNullOrWhiteSpace(Config.Text))
            {
                Log.Error("Watermark is not displayed because its configured text is empty.");
                base.OnEnabled();
                return;
            }

            if (float.IsNaN(Config.Position) || float.IsInfinity(Config.Position) || Config.Position < 0f || Config.Position > 1000f)
            {
                Log.Error("Watermark is not displayed because its configured position must be between 0 and 1000.");
                base.OnEnabled();
                return;
            }

            watermark = new BasicElement(Config.Position, Config.Text)
            {
                ZIndex = 100,
                ResolutionBasedAlign = true,
                VerticalAlign = VerticalAlign.Up
            };

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            foreach (Player player in Player.List)
            {
                ShowWatermark(player);
            }

            base.OnEnabled();
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            Player player = ev.Player;
            Timing.CallDelayed(2f, () =>
            {
                if (player.IsConnected)
                    ShowWatermark(player);
            });
        }

        private void ShowWatermark(Player player)
        {
            if (watermark != null)
            {
                RueDisplay.Get(player).Show(WatermarkTag, watermark);
            }
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            foreach (Player player in Player.List)
            {
                RueDisplay.Get(player).Remove(WatermarkTag);
            }

            watermark = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}

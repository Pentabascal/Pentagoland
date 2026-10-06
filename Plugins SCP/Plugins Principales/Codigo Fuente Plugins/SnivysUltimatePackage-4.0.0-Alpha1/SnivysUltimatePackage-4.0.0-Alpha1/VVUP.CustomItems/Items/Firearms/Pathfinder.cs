using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunCOM18)]
    public class Pathfinder : VVUP.CustomItems.API.ValidatedCustomWeapon, ICustomItemGlow
    {
        [YamlIgnore] public override ItemType Type { get; set; } = ItemType.GunCOM18;
        public override uint Id { get; set; } = 40;
        public override string Name { get; set; } = "<color=#0096FF>Pathfinder</color>";

        public override string Description { get; set; } =
            "Al alcanzar a un enemigo, lo marca durante un tiempo y hace que reciba más daño.";

        public override float Weight { get; set; } = 1.5f;
        public override SpawnProperties SpawnProperties { get; set; }
        public override float Damage { get; set; } = 0;
        public override byte ClipSize { get; set; } = 1;

        [Description("Duración de la marca aplicada al objetivo.")]
        public float MarkedDuration { get; set; } = 10f;
        [Description("Multiplicador del daño que reciben los objetivos marcados (1,10 = un 10 % más; 1,50 = un 50 % más, etc.).")]
        public float DamageMultiplier { get; set; } = 1.10f;
        [Description("Si está activado, Pathfinder podrá marcar a los compañeros en servidores con fuego amigo. No tiene efecto si el fuego amigo está desactivado.")]
        public bool MarkTeammates { get; set; } = false;
        
        private List<PlayerAPI> _activeMarkedPlayers = new();
        private List<PlayerAPI> _playersReloading = new();
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 255, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;
            base.UnsubscribeEvents();
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            if (ev.Target == null || !ShouldMarkTarget(ev.Player, ev.Target))
                return;

            if (_activeMarkedPlayers.Contains(ev.Target))
                return;

            _activeMarkedPlayers.Add(ev.Target);
            Timing.CallDelayed(MarkedDuration, () =>
            {
                _activeMarkedPlayers.Remove(ev.Target);
            });
        }

        private bool ShouldMarkTarget(PlayerAPI shooter, PlayerAPI target)
        {
            Side shooterSide = shooter.Role.Side;
            if (shooterSide is not (Side.Mtf or Side.ChaosInsurgency or Side.Tutorial or Side.Scp))
                return false;

            return MarkTeammates || shooterSide != target.Role.Side;
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (IsMatchingWeapon(ev))
                base.OnHurting(ev);

            if (!_activeMarkedPlayers.Contains(ev.Player))
                return;
            ev.Amount *= DamageMultiplier;
        }

        private void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            Timing.CallDelayed(2f, () =>
            {
                Log.Debug($"VVUP Custom Items: Pathfinder, {ev.Player.Nickname} has started reloading, setting correct ammo");
                ev.Firearm.MagazineAmmo = ClipSize;
            });
        }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using RueI.API;
using RueI.API.Elements;
using UnityEngine;
using VVUP.Base.API;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunFSP9)]
    public class MediGun : VVUP.CustomItems.API.ValidatedCustomWeapon, ICustomItemGlow
    {
        public override ItemType Type { get; set; } = ItemType.GunFSP9;
        
        public override uint Id { get; set; } = 27;
        public override string Name { get; set; } = "<color=#0096FF>Phantom Pulse</color>";

        public override string Description { get; set; } =
            "Cura 20 HP al jugador alcanzado y elimina efectos negativos concretos. No cura zombis.";

        public override float Weight { get; set; } = 1.5f;
        
        public override float Damage { get; set; } = 0;

        public override byte ClipSize { get; set; } = 10;

        [Description("¿El medigun puede dañar a los aliados? (Solo se aplica en servidores con fuego amigo activado).")]
        public override bool FriendlyFire { get; set; } = false;
        
        private const float HealingAmount = 20f;
        private const float ShotCooldown = 5f;
        private const float HealedMessageDuration = 2f;
        private const float HealedMessagePosition = 250f;
        private const string HealedMessageTagId = "VVUP-CustomItems-MediGun-Healed";
        private static readonly Tag HealedMessageTag = new(HealedMessageTagId);
        private static readonly EffectType[] EffectsToClear =
        {
            EffectType.Bleeding,
            EffectType.Blurred,
            EffectType.Burned,
            EffectType.Deafened,
            EffectType.Disabled,
        };

        private readonly Dictionary<int, float> _nextShotByPlayer = new();
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGr18,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGateA,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideGateB,
                },
            }
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 255, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            _nextShotByPlayer.Clear();
            base.UnsubscribeEvents();
        }

        protected override void OnShooting(Exiled.Events.EventArgs.Player.ShootingEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
            {
                float now = Time.time;
                if (_nextShotByPlayer.TryGetValue(ev.Player.Id, out float nextShotTime) && now < nextShotTime)
                {
                    ev.IsAllowed = false;
                    return;
                }

                base.OnShooting(ev);
                if (ev.IsAllowed)
                    _nextShotByPlayer[ev.Player.Id] = now + ShotCooldown;
                return;
            }

            base.OnShooting(ev);
        }

        private void OnPlayerLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev) =>
            _nextShotByPlayer.Remove(ev.Player.Id);

        protected override void OnWaitingForPlayers()
        {
            _nextShotByPlayer.Clear();
            base.OnWaitingForPlayers();
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (!IsMatchingWeapon(ev))
                return;

            if (ev.Attacker == ev.Player)
            {
                base.OnHurting(ev);
                return;
            }

            ev.IsAllowed = false;
            if (ev.Player.IsAlive && ev.Player.Role != RoleTypeId.Scp0492)
            {
                ev.Player.Heal(HealingAmount);
                foreach (EffectType effect in EffectsToClear)
                    ev.Player.DisableEffect(effect);

                string healerName = ev.Attacker.Nickname
                    .Replace("<", "[")
                    .Replace(">", "]")
                    .Replace('\r', ' ')
                    .Replace('\n', ' ');
                string message = $"<align=center><size=22>Fuiste curado por {healerName}</size></align>";

                RueDisplay.Get(ev.Player).Show(
                    HealedMessageTag,
                    new BasicElement(HealedMessagePosition, message),
                    HealedMessageDuration);
            }
        }
    }
}
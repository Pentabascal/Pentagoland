using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using System.ComponentModel;
using UnityEngine;
using VVUP.Base;
using ApplyEffects = VVUP.CustomItems.ApplyEffects;
using VVUP.Base.API;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : VVUP.CustomItems.API.ValidatedCustomWeapon, ICustomItemGlow
    {
        public override ItemType Type { get; set; } = ItemType.GunE11SR;
        public override uint Id { get; set; } = 58;
        public override string Name { get; set; } = "<color=#009F64>Rifle de cerrojo EL-95</color>";
        public override string Description { get; set; } = "Rifle de francotirador de cerrojo.";
        public override float Weight { get; set; } = 1.5f;
        public override AttachmentName[] Attachments { get; set; } = {AttachmentName.LowcapMagAP, AttachmentName.ExtendedStock, AttachmentName.RifleBody, AttachmentName.Laser, AttachmentName.Foregrip, AttachmentName.ScopeSight, AttachmentName.SoundSuppressor};
        public override byte ClipSize { get; set; } = 1;
        public override float Damage { get; set; } = 150f;

        private readonly Dictionary<ushort, float> _lastShotTimes = new();

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new ()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside079Armory,
                },
            }
        };

        [Description("Si está activado, se podrán cambiar los accesorios. Desactivado de forma predeterminada.")]
        public bool AllowChangingAttachments { get; set; } = false;
        public string RestrictedAttachmentChangingMessage { get; set; } =
            "No puedes cambiar los accesorios del rifle EL-95.";
        public bool UseHints { get; set; } = false;
        public float RestrictedAttachmentChangeMessageTimeDuration { get; set; } = 5f;

        [Description("Si no está vacío, los efectos se aplicarán al apuntar con la mira.")]
        public List<ApplyEffects> ApplyEffectsOnAim { get; set; } = new()
        {
            new ApplyEffects()
            {
                EffectType = EffectType.Slowness,
                Intensity = 255,
                Duration = 0,
            },
        };

        [Description(
            "If its a positive, non-zero number, it will determine how long it would take before being allowed to reload again after firing")]
        public float TimeBeforeReloadAllowed { get; set; } = 0f;
        [Description("Usa %time% para mostrar cuánto falta para poder volver a recargar.")]
        public string RestrictedReloadMessage { get; set; } = "Debes esperar %time% segundos antes de volver a recargar.";

        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(0, 150, 100, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.AimingDownSight += OnAim;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.AimingDownSight -= OnAim;
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            _lastShotTimes.Clear();
            base.OnWaitingForPlayers();
        }

        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem))
                _lastShotTimes[ev.Player.CurrentItem.Serial] = Time.time;
            base.OnShooting(ev);
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem) && TimeBeforeReloadAllowed > 0)
            {
                if (_lastShotTimes.TryGetValue(ev.Player.CurrentItem.Serial, out float lastShotTime))
                {
                    float remainingTime = TimeBeforeReloadAllowed - (Time.time - lastShotTime);
                    if (remainingTime > 0)
                    {
                        Log.Debug($"VVUP Custom Items, Sniper: Player {ev.Player.Nickname} tried to reload too soon, remaining time: {remainingTime:F1} seconds");
                        ev.IsAllowed = false;
                        if (!string.IsNullOrWhiteSpace(RestrictedReloadMessage))
                        {
                            string message = RestrictedReloadMessage.Replace("%time%", remainingTime.ToString("F1"));
                            if (UseHints)
                                ev.Player.ShowHint(message, RestrictedAttachmentChangeMessageTimeDuration);
                            else
                                ev.Player.Broadcast((ushort)RestrictedAttachmentChangeMessageTimeDuration, message);
                        }
                    }
                }
            }
            base.OnReloading(ev);
        }

        protected override void OnChangingAttachment(ChangingAttachmentsEventArgs ev)
        {
            if (!Check(ev.Item) || AllowChangingAttachments)
                return;
            if (!string.IsNullOrWhiteSpace(RestrictedAttachmentChangingMessage)) 
                if (UseHints)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Sniper, showing Restricted Attachment Changing Message Hint to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.ShowHint(RestrictedAttachmentChangingMessage,
                        RestrictedAttachmentChangeMessageTimeDuration);
                }
                else
                {
                    Log.Debug(
                        $"VVUP Custom Items: Sniper, showing Restricted Attachment Changing Message Broadcast to {ev.Player.Nickname} for {RestrictedAttachmentChangeMessageTimeDuration} seconds");
                    ev.Player.Broadcast((ushort)RestrictedAttachmentChangeMessageTimeDuration,
                        RestrictedAttachmentChangingMessage);
                }
            ev.IsAllowed = false;
            base.OnChangingAttachment(ev);
        }

        public void OnAim(AimingDownSightEventArgs ev)
        {
            if (ApplyEffectsOnAim.IsEmpty() || !Check(ev.Item))
                return;

            if (ev.AdsIn)
            {
                foreach(var effect in ApplyEffectsOnAim)
                {
                    ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                    Log.Debug($"VVUP Custom Items: Sniper, applied effect {effect.EffectType} with intensity {effect.Intensity} for {effect.Duration} seconds to {ev.Player.Nickname}");
                }
            }
            else
            {
                foreach(var effect in ApplyEffectsOnAim)
                {
                    ev.Player.DisableEffect(effect.EffectType);
                    Log.Debug($"VVUP Custom Items: Sniper, removed effect {effect.EffectType} from {ev.Player.Nickname}");
                }
            }
        }
    }
}

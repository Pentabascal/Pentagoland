using System.Collections.Generic;
using System.ComponentModel;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.AntiSCP207)]
    public class AdditionalHealth207 : CustomItem, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.AntiSCP207;
        public override uint Id { get; set; } = 37;
        public override string Name { get; set; } = "<color=#6600CC>Salud adicional SCP-207</color>";
        public override string Description { get; set; } = "Aumenta la salud al consumirlo.";
        public override float Weight { get; set; } = 1;
        public float HealthToBeAdded { get; set; } = 15;
        public bool AllowWhen207OrAnti207IsActive { get; set; } = false;

        public string TextToShowToPlayerOnFailCola { get; set; } =
            "No creo que mezclar esto con el efecto de una bebida de cola sea buena idea.";
        [Description("Si UseHints está activado, se mostrará una sugerencia al jugador; de lo contrario, se mostrará un aviso.")]
        public bool UseHints { get; set; } = false;
        [Description("Si se desactiva, los jugadores podrán consumir muchas unidades de Salud adicional SCP-207.")]
        public bool CapMaxHealth { get; set; } = true;
        public float MaxUserHpLimit { get; set; } = 130;
        public string TextToShowToPlayerOnFailHpLimit { get; set; } = "Ya he tomado suficiente; no necesito más.";
        public ushort TextDisplayDuration { get; set; } = 5;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new DynamicSpawnPoint()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Chance = 25,
                    Role = RoleTypeId.Scp106
                }
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        
        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            base.UnsubscribeEvents();
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            
            if (ev.Player.GetEffectIntensity<Scp207>() != 0 && !AllowWhen207OrAnti207IsActive || ev.Player.GetEffectIntensity<AntiScp207>() != 0 && !AllowWhen207OrAnti207IsActive)
            {
                Log.Debug(
                    $"VVUP Custom Items: Additional Health 207, {ev.Player.Nickname} tried using Additional Health 207, but has Anti-207 or 207 active, but allowing to use this is disabled with either effect active");
                if (!string.IsNullOrWhiteSpace(TextToShowToPlayerOnFailCola))
                    if (UseHints)
                    {
                        Log.Debug($"VVUP Custom Items: Additional Health 207, displaying use fail hint to {ev.Player.Nickname}");
                        ev.Player.ShowHint(TextToShowToPlayerOnFailCola, TextDisplayDuration);
                    }
                    else
                    {
                        Log.Debug($"VVUP Custom Items: Additional Health 207, displaying use fail broadcast to {ev.Player.Nickname}");
                        ev.Player.Broadcast(TextDisplayDuration, TextToShowToPlayerOnFailCola);
                    }
                ev.IsAllowed = false;
                return;
            }

            if (CapMaxHealth && ev.Player.MaxHealth >= MaxUserHpLimit)
            {
                Log.Debug(
                    $"VVUP Custom Items: Additional Health 207, {ev.Player.Nickname} tried using Additional Health 207, " +
                    $"but has the max health cap, {ev.Player.Nickname} has {ev.Player.MaxHealth} max health, " +
                    $"and the limit is set to {MaxUserHpLimit}");
                if (!string.IsNullOrWhiteSpace(TextToShowToPlayerOnFailHpLimit))
                    if (UseHints)
                    {
                        Log.Debug($"VVUP Custom Items: Additional Health 207, displaying use fail hint to {ev.Player.Nickname}");
                        ev.Player.ShowHint(TextToShowToPlayerOnFailHpLimit, TextDisplayDuration);
                    }
                    else
                    {
                        Log.Debug($"VVUP Custom Items: Additional Health 207, displaying use fail broadcast to {ev.Player.Nickname}");
                        ev.Player.Broadcast(TextDisplayDuration, TextToShowToPlayerOnFailHpLimit);
                    }
                ev.IsAllowed = false;
                return;
            }

            ev.IsAllowed = false;
            ev.Player.RemoveHeldItem();
            Log.Debug(
                $"VVUP Custom Items: Additional Health 207, {ev.Player.Nickname} is consuming Addition Health 207, locking inventory for a moment");
            
            ev.Player.MaxHealth += HealthToBeAdded;
            ev.Player.Heal(HealthToBeAdded);
        }
    }
}
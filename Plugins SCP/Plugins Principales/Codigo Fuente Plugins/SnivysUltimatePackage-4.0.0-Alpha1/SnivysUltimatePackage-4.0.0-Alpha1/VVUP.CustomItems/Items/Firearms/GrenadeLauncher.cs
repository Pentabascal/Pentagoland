using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Modules;
using JetBrains.Annotations;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using Firearm = Exiled.API.Features.Items.Firearm;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunLogicer)]
    public class GrenadeLauncher : VVUP.CustomItems.API.ValidatedCustomWeapon, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GunLogicer;
        public override uint Id { get; set; } = 46;
        public override string Name { get; set; } = "<color=#FF0000>ADATS</color>";
        public override string Description { get; set; } = "Un lanzagranadas.";
        public override float Weight { get; set; } = 3;

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
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new ()
                {
                    Chance = 40,
                    Location = SpawnLocationType.Inside079Armory,
                },
            }
        };
        public override byte ClipSize { get; set; } = 1;
        [Description("Desactívalo para incluir granadas personalizadas (como las granadas de racimo) en el lanzagranadas. NOTA: esto no hará que el lanzagranadas dispare granadas de racimo.")]
        public bool IgnoreCustomGrenades { get; set; } = true;

        //public float GrenadeFuseTime { get; set; } = 1.5f;
        public bool UseGrenadesToReload { get; set; } = true;

        [Description("Optional message shown when the ADATS cannot be reloaded. Leave empty to disable it.")]
        public string ReloadMessageDryfire { get; set; } = string.Empty;
        public string FullForceSetMessage { get; set; } = "El ADATS está en modo de fuerza completa.";
        public string HalfForceSetMessage { get; set; } = "El ADATS está en modo de media fuerza.";
        public string LaunchTypeImpactSetMessage { get; set; } = "El ADATS está en modo de detonación por impacto.";
        public string LaunchTypeRollerSetMessage { get; set; } = "El ADATS está en modo de detonación por rodadura.";
        [Description("Si está activado, se mostrará una sugerencia al jugador; de lo contrario, se mostrará un aviso.")]
        public bool UseHints { get; set; } = true;
        [Description("Duración de la sugerencia o el aviso.")]
        public float MessageDuration { get; set; } = 3f;
        [Description("Al recargar, a veces se supera el valor de ClipSize. Si está activado, se comprobará y corregirá la cantidad de munición.")]
        public bool FixOverClipSizeBug { get; set; } = true;
        private ProjectileType GrenadeType { get; set; } = ProjectileType.FragGrenade;
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        [CanBeNull] 
        private CustomGrenade _loadedCustomGrenade;
        [YamlIgnore]
        private Dictionary<Player, bool> FullForceMode { get; set; } = new();
        [YamlIgnore]
        private Dictionary<Player, bool> LaunchTypeMode { get; set; } = new();
        [YamlIgnore]
        private readonly HashSet<ushort> _reloadsInProgress = new();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.DryfiringWeapon += OnDryfiringWeapon;
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.DryfiringWeapon -= OnDryfiringWeapon;
            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            FullForceMode.Clear();
            LaunchTypeMode.Clear();
            _reloadsInProgress.Clear();
            base.OnWaitingForPlayers();
        }

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if (!FullForceMode.ContainsKey(player))
                FullForceMode.Add(player, true);
            if (!LaunchTypeMode.ContainsKey(player))
                LaunchTypeMode.Add(player, true);
            base.OnAcquired(player, item, displayMessage);
        }

        public void ToggleForceMode(Player player)
        {
            if (!FullForceMode.ContainsKey(player))
                FullForceMode.Add(player, true);

            FullForceMode[player] = !FullForceMode[player];
            Log.Debug(
                $"VVUP Custom Items: Grenade Launcher: {player.Nickname} toggled force mode to {(FullForceMode[player] ? "Full Force" : "Half Force")}");
            if (!string.IsNullOrWhiteSpace(FullForceSetMessage) || !string.IsNullOrWhiteSpace(HalfForceSetMessage))
                if (UseHints)
                    player.ShowHint(FullForceMode[player] ? FullForceSetMessage : HalfForceSetMessage, MessageDuration);
                else
                    player.Broadcast((ushort)MessageDuration,
                        FullForceMode[player] ? FullForceSetMessage : HalfForceSetMessage);
        }

        public void ToggleLaunchTypeMode(Player player)
        {
            if (!LaunchTypeMode.ContainsKey(player))
                LaunchTypeMode.Add(player, true);
            
            LaunchTypeMode[player] = !LaunchTypeMode[player];
            Log.Debug($"VVUP Custom Items: Grenade Launcher: {player.Nickname} toggled launch type mode to {(LaunchTypeMode[player]? "Impact" : "Roller")}");
            if (!string.IsNullOrWhiteSpace(LaunchTypeImpactSetMessage) || !string.IsNullOrWhiteSpace(LaunchTypeRollerSetMessage))
                if (UseHints)
                    player.ShowHint(LaunchTypeMode[player] ? LaunchTypeImpactSetMessage : LaunchTypeRollerSetMessage,
                        MessageDuration);
                else
                    player.Broadcast((ushort)MessageDuration,
                        LaunchTypeMode[player] ? LaunchTypeImpactSetMessage : LaunchTypeRollerSetMessage);
        }
        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Player.CurrentItem is Firearm firearm)
            {
                if (firearm.MagazineAmmo > ClipSize && FixOverClipSizeBug)
                {
                    Log.Debug("VVUP Custom Items: Grenade Launcher: Fixing ammo count due to over clip size bug");
                    firearm.MagazineAmmo = ClipSize;
                }
                firearm.MagazineAmmo -= 1;
            }
            Log.Debug($"VVUP Custom Items: Grenade Launcher: {ev.Player.Nickname} fired, firing a {GrenadeType}, Is Full Force: {FullForceMode[ev.Player]}, Is Impact: {LaunchTypeMode[ev.Player]}");
            switch (GrenadeType)
            {
                case ProjectileType.FragGrenade or ProjectileType.Flashbang or ProjectileType.Scp2176:
                    Projectile projectile = ev.Player.ThrowGrenade(GrenadeType, FullForceMode[ev.Player]).Projectile;
                    if (LaunchTypeMode[ev.Player])
                        projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, projectile.Base);
                    break;
                
                case ProjectileType.Scp018 or ProjectileType.Snowball or ProjectileType.Coal or ProjectileType.SpecialCoal:
                    Throwable throwable = (Throwable)Item.Create(GrenadeType.GetItemType());
                    ev.Player.ThrowItem(throwable, FullForceMode[ev.Player]);
                    break;
            }
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (UseGrenadesToReload)
            {
                ev.IsAllowed = false;
                if (!TryReloadWithGrenade(ev.Player, ev.Player.CurrentItem as Firearm) &&
                    !string.IsNullOrWhiteSpace(ReloadMessageDryfire))
                {
                    if (UseHints)
                        ev.Player.ShowHint(ReloadMessageDryfire, MessageDuration);
                    else
                        ev.Player.Broadcast((ushort)MessageDuration, ReloadMessageDryfire);
                }
                return;
            }

            Log.Debug($"VVUP Custom Items: Grenade Launcher: {ev.Player.Nickname} reloaded the Grenade Launcher with regular ammo.");
        }

        private void OnDryfiringWeapon(DryfiringWeaponEventArgs ev)
        {
            if (Check(ev.Player.CurrentItem) && ev.Player.CurrentItem is Firearm { MagazineAmmo: 0 } firearm && UseGrenadesToReload)
            {
                Log.Debug(
                    $"VVUP Custom Items: Grenade Launcher: {ev.Player.Nickname} is reloading the Grenade Launcher with grenades.");
                TryReloadWithGrenade(ev.Player, firearm);
            }
        }

        private bool TryReloadWithGrenade(Player player, Firearm firearm)
        {
            if (!UseGrenadesToReload || firearm == null || firearm.MagazineAmmo != 0 ||
                !Check(firearm) || _reloadsInProgress.Contains(firearm.Serial))
                return false;

            foreach (Item item in player.Items.ToList())
            {
                Log.Debug($"VVUP Custom Items: Grenade Launcher: {player.Nickname} has {item.Type}");
                if (item.Type != ItemType.GrenadeHE && item.Type != ItemType.GrenadeFlash &&
                    item.Type != ItemType.SCP018 && item.Type != ItemType.SCP2176
                    && item.Type != ItemType.Coal && item.Type != ItemType.SpecialCoal && item.Type != ItemType.Snowball)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Grenade Launcher: {player.Nickname} has a {item.Type}, not a grenade, skipping.");
                    continue;
                }

                if (TryGet(item, out CustomItem? customItem))
                {
                    if (IgnoreCustomGrenades)
                    {
                        Log.Debug(
                            $"VVUP Custom Items: Grenade Launcher: {player.Nickname} has a {item.Type}, but it's a custom grenade, skipping.");
                        continue;
                    }

                    if (customItem is CustomGrenade customGrenade)
                    {
                        _loadedCustomGrenade = customGrenade;
                        Log.Debug(
                            $"VVUP Custom Items: Grenade Launcher: {player.Nickname} has a {item.Type}, it's a custom grenade, setting it to {_loadedCustomGrenade.Name}");
                    }
                }

                _reloadsInProgress.Add(firearm.Serial);
                player.DisableEffect(EffectType.Invisible);
                GrenadeType = item.Type switch
                {
                    ItemType.GrenadeHE => ProjectileType.FragGrenade,
                    ItemType.GrenadeFlash => ProjectileType.Flashbang,
                    ItemType.SCP018 => ProjectileType.Scp018,
                    ItemType.SCP2176 => ProjectileType.Scp2176,
                    ItemType.Coal => ProjectileType.Coal,
                    ItemType.SpecialCoal => ProjectileType.SpecialCoal,
                    ItemType.Snowball => ProjectileType.Snowball
                };
                player.RemoveItem(item);
                ushort ammo762Amount = player.GetAmmo(AmmoType.Nato762);
                player.AddAmmo(AmmoType.Nato762, 1);
                Timing.CallDelayed(0.5f, () =>
                {
                    if (firearm.Base.TryGetModule(out AnimatorReloaderModuleBase reloaderModule))
                    {
                        reloaderModule.ClientTryReload();
                        reloaderModule.ServerTryReload();
                    }
                    Log.Debug(
                        $"VVUP Custom Items: Grenade Launcher: Server-side reload triggered for {player.Nickname}");
                });
                Timing.CallDelayed(4f, () =>
                {
                    try
                    {
                        firearm.MagazineAmmo = ClipSize;
                        player.SetAmmo(AmmoType.Nato762, ammo762Amount);
                    }
                    finally
                    {
                        _reloadsInProgress.Remove(firearm.Serial);
                    }
                });
                Log.Debug(
                    $"VVUP Custom Items: Grenade Launcher: {player.Nickname} reloaded the Grenade Launcher with a {GrenadeType} grenade.");
                return true;
            }

            return false;
        }
        protected override void OnReloaded(ReloadedWeaponEventArgs ev)
        {
            Log.Debug($"VVUP Custom Items: Grenade Launcher: {ev.Player.Nickname} reloaded the Grenade Launcher setting Magazine Ammo to {ClipSize}.");
            ev.Firearm.MagazineAmmo = ClipSize;
        }
    }
}
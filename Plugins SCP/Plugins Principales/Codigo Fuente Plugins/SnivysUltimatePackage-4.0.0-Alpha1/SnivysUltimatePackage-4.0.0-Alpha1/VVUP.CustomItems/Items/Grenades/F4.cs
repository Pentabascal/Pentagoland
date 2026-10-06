using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Items.ThrowableProjectiles;
using UnityEngine;
using VVUP.Base.API;
using VVUP.CustomItems.API;
using YamlDotNet.Serialization;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class F4 : CustomGrenade, ICustomItemGlow
    {
        public enum F4RemoveMethod
        {
            Remove = 0,
            Detonate = 1,
            Drop = 2,
        }
        public static F4 Instance { get; private set; } = null!;
        public static Dictionary<Pickup, Player> PlacedCharges { get; } = new();
        public override uint Id { get; set; } = 54;
        public override string Name { get; set; } = "<color=#6600CC>F4</color>";
        public override float Weight { get; set; } = 0.75f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideLczArmory,
                },

                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory,
                },

                new()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside049Armory,
                },
                
                new ()
                {
                    Chance = 50,
                    Location = SpawnLocationType.Inside079Armory,
                },
                
                new()
                {
                    Chance = 100,
                    Location = SpawnLocationType.InsideSurfaceNuke,
                },
            },
        };
        public override string Description { get; set; } = "Granada cegadora que puede detonarse a distancia.";
        
        [Description("¿Es necesario un objeto específico para detonar el F4?")]
        public bool RequireDetonator { get; set; } = true;
        
        [Description("Objeto detonador que se usará para detonar las cargas F4.")]
        public ItemType DetonatorItem { get; set; } = ItemType.Radio;
        
        [Description(
            "What happens with F4 charges placed by player, when he dies/leaves the game. (Remove / Detonate / Drop)")]
        public F4RemoveMethod MethodOnDeath { get; set; } = F4RemoveMethod.Drop;
        [Description("¿Disparar a las cargas F4 debe tener algún efecto?")]
        public bool AllowShoot { get; set; } = true;
        
        [Description("Qué ocurre con las cargas F4 al dispararles: Remove (eliminar), Detonate (detonar) o Drop (soltar).")]
        public F4RemoveMethod ShotMethod { get; set; } = F4RemoveMethod.Remove;
        
        [Description("Distancia máxima entre la carga F4 y el jugador para detonarla.")]
        public float MaxDistance { get; set; } = 100f;
        
        [Description("Tiempo tras el cual la carga F4 detonará automáticamente.")]
        public override float FuseTime { get; set; } = 9999f;
        
        [Description("¿La explosión del F4 se atribuye al jugador que lo colocó o al servidor?")]
        public bool AssociateF4WithServer { get; set; } = false;
        [Description("Determina si el F4 se adhiere a las superficies o rueda por el suelo.")]
        public bool Sticky { get; set; } = true;
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(102, 0, 204, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        
        [YamlIgnore]
        public override bool ExplodeOnCollision { get; set; } = false;
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public void F4Handler(Pickup? charge, F4RemoveMethod removeMethod = F4RemoveMethod.Detonate, Player? player = null)
        {
            if (charge?.Base?.gameObject == null)
                return;

            if (player == null && PlacedCharges.TryGetValue(charge, out var placedPlayer))
            {
                player = placedPlayer;
            }

            switch (removeMethod)
            {
                case F4RemoveMethod.Remove:
                {
                    break;
                }

                case F4RemoveMethod.Detonate:
                {
                    FlashGrenade grenade = (FlashGrenade)Item.Create(Type);
                    grenade.FuseTime = 0.1f;
                    grenade.SpawnActive(charge.Position, owner: AssociateF4WithServer ? Server.Host : player);
                    break;
                }

                case F4RemoveMethod.Drop:
                {
                    TrySpawn(Id, charge.Position, out _);
                    break;
                }
            }

            PlacedCharges.Remove(charge);
            charge.Destroy();
        }
        
        protected override void SubscribeEvents()
        {
            Instance = this;

            StickyGrenadeApi.RegisterStickyGrenade("F4", pickup => Sticky && PlacedCharges.ContainsKey(pickup));
            
            PlayerEvent.Destroying += OnDestroying;
            PlayerEvent.Died += OnDied;
            PlayerEvent.Shooting += OnShooting;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            base.SubscribeEvents();
        }
        
        protected override void UnsubscribeEvents()
        {
            StickyGrenadeApi.UnregisterStickyGrenade("F4");
            PlayerEvent.Destroying -= OnDestroying;
            PlayerEvent.Died -= OnDied;
            PlayerEvent.Shooting -= OnShooting;

            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            PlacedCharges.Clear();

            base.OnWaitingForPlayers();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (!PlacedCharges.ContainsKey(ev.Projectile))
                PlacedCharges.Add(ev.Projectile, ev.Player);
            
            base.OnThrownProjectile(ev);
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            if (!AssociateF4WithServer)
                ev.Projectile.PreviousOwner = ev.Player;
            Pickup pickup = Pickup.Get(ev.Projectile.Base);
            PlacedCharges.Remove(pickup);
        }

        private void OnDestroying(DestroyingEventArgs ev)
        {
            foreach (var charge in PlacedCharges.ToList().Where(charge => charge.Value == ev.Player))
            {
                try
                {
                    if (charge.Key == null || charge.Key.Base == null || charge.Key.Base.gameObject == null)
                    {
                        PlacedCharges.Remove(charge.Key!);
                        continue;
                    }
                    F4Handler(charge.Key, MethodOnDeath, ev.Player);
                }
                catch (System.NullReferenceException)
                {
                    PlacedCharges.Remove(charge.Key!);
                }
            }
        }

        private void OnDied(DiedEventArgs ev)
        {
            if (ev.Player == null)
                return;

            foreach (var charge in PlacedCharges.ToList().Where(charge => charge.Value == ev.Player))
            {
                try
                {
                    if (charge.Key == null || charge.Key.Base == null || charge.Key.Base.gameObject == null)
                    {
                        PlacedCharges.Remove(charge.Key!);
                        continue;
                    }
                    F4Handler(charge.Key, MethodOnDeath, ev.Player);
                }
                catch (System.NullReferenceException)
                {
                    PlacedCharges.Remove(charge.Key!);
                }
            }
        }

        private void OnShooting(ShootingEventArgs ev)
        {
            if (!AllowShoot)
                return;

            Vector3 forward = ev.Player.CameraTransform.forward;
            if (Physics.Raycast(ev.Player.CameraTransform.position + forward, forward, out var hit, 500))
            {
                EffectGrenade grenade = hit.collider.gameObject.GetComponentInParent<EffectGrenade>();
                if (grenade == null)
                {
                    return;
                }

                if (PlacedCharges.ContainsKey(Pickup.Get(grenade)))
                {
                    F4Handler(Pickup.Get(grenade), ShotMethod, ev.Player);
                }
            }
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            PlacedCharges.Clear();
            StickyGrenadeApi.ClearTrackedGrenades();
        }
    }
}
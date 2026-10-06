using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using Item = Exiled.API.Features.Items.Item;
using Server = Exiled.API.Features.Server;

namespace VVUP.CustomItems.Items.Grenades
{
    [CustomItem(ItemType.GrenadeHE)]
    public class GuardianGrenade : CustomGrenade, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeHE;

        public override uint Id { get; set; } = 55;
        public override string Name { get; set; } = "<color=#FF0000>Granada guardiana</color>";
        public override string Description { get; set; } = "Granada que detona cuando alguien se acerca.";
        public override float Weight { get; set; } = 0.75f;
        public override bool ExplodeOnCollision { get; set; } = false;

        [Description("Tiempo hasta que la granada desaparece si nadie se acerca.")]
        public override float FuseTime { get; set; } = 9999;
        [Description("Radio en el que detonará la granada al entrar un jugador.")]
        public float TriggerRadius { get; set; } = 3.5f;
        [Description("Tiempo que tarda la granada en activarse y poder detonar.")]
        public float ActivationDelay { get; set; } = 2.0f;

        [Description("Demora desde que un jugador entra en el radio.")]
        public float DetonationDelay { get; set; } = 2.0f;
        [Description("Tiempo de mecha de la granada desde que un jugador entra en el radio.")]
        public float DetonationTime { get; set; } = 2.0f;

        [Description("¿La granada debe ignorar a los compañeros de quien la lanzó? (No activa ni desactiva el fuego amigo).")]
        public bool IgnoreTeammates { get; set; } = false;

        [Description("¿La granada se atribuye al servidor o al jugador?")]
        public bool AssociateAsServer { get; set; } = false;
        [Description("Frecuencia con la que la granada comprobará si hay jugadores cerca (en segundos).")]
        public float CheckInterval { get; set; } = 0.25f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideLczArmory
                },
                new()
                {
                    Chance = 25,
                    Location = SpawnLocationType.InsideHczArmory
                }
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        
        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            Timing.CallDelayed(ActivationDelay, () =>
            {
                Timing.RunCoroutine(MonitorProximity(ev));
            });
            base.OnThrownProjectile(ev);
        }
        
        private IEnumerator<float> MonitorProximity(ThrownProjectileEventArgs ev)
        {
            while (ev.Projectile != null)
            {
                foreach (var player in Exiled.API.Features.Player.List)
                {
                    if (player == null || (IgnoreTeammates && player.Role.Team == ev.Player.Role.Team))
                        continue;
                
                    if (Vector3.Distance(player.Position, ev.Projectile.Position) <= TriggerRadius)
                    {
                        yield return Timing.WaitForSeconds(DetonationDelay);

                        if (ev.Projectile != null)
                        {
                            ev.Projectile.Destroy();
                            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(Type);
                            grenade.FuseTime = DetonationTime;
                            grenade.SpawnActive(ev.Projectile.Position, owner: AssociateAsServer ? Server.Host : ev.Player);
                        }

                        yield break;
                    }
                }
        
                yield return Timing.WaitForSeconds(CheckInterval);
            }
        }
    }
}
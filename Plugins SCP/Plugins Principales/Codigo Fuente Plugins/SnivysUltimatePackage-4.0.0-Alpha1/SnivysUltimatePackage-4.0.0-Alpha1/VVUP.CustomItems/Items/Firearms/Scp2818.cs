using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class Scp2818 : VVUP.CustomItems.API.ValidatedCustomWeapon, ICustomItemGlow
    {
        public override uint Id { get; set; } = 33;
        public override string Name { get; set; } = "<color=#FF0000>SCP-2818</color>";

        public override string Description { get; set; } =
            "Al dispararla, utiliza al tirador como proyectil. Creando una explosion al impactar.";

        public override float Weight { get; set; } = 4;
        public override float Damage { get; set; } = 1;
        public override byte ClipSize { get; set; } = 1;

        [Description("Determina si el arma desaparece tras disparar y acertar.")]
        public bool DespawnAfterUse { get; set; } = true;
        [Description("Determina si el arma desaparece tras disparar y fallar.")]
        public bool DespawnAfterMiss { get; set; } = true;
        [Description("¿Debe morir el jugador al instante si falla un disparo con esta arma?")]
        public bool KillAfterMiss { get; set; } = true;
        [Description("¿Se debe comprobar si el jugador ha disparado al suelo? Nota: desactivarlo permitirá acceder a lugares que deberían ser inaccesibles.")]
        public bool UseRaycastPosition { get; set; } = true;

        public string DeathReasonUser { get; set; } = "Vaporizado por ser un Kamikaze";
        public string DeathReasonTarget { get; set; } = "Vaporizado por un Kamikaze";

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new ()
                {
                    Chance = 16,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void OnShot(ShotEventArgs ev)
        {
            bool miss = false;
            if (ev.Target == null)
            {
                miss = true;
            }
            else
            {
                Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Player.Nickname} shot and hit {ev.Target.Nickname}, running hit code");
                ev.CanHurt = false;
                Vector3 impactPosition = ev.Position;
                Vector3 targetPosition = ev.Target.Position;
                Log.Debug($"VVUP Custom Items: SCP-2818, dealing {Damage} base damage to {ev.Target.Nickname}");
                ev.Target.Hurt(Damage);
                SpawnImpactGrenade(impactPosition, ev.Player);
                ev.Player.Position = targetPosition;
            }



            Timing.CallDelayed(0.1f, () =>
            {
                if (miss)
                {
                    Log.Debug($"VVUP Custom Items: SCP-2818, {ev.Player.Nickname} fired and missed a target, teleporting them to bullet impact location ({ev.RaycastHit.point}");

                    if (DespawnAfterMiss)
                    {
                        ev.Player.RemoveHeldItem();
                    } else
                    {
                        ev.Player.DropHeldItem();
                    }

                    ev.CanSpawnImpactEffects = false;

                    if (UseRaycastPosition)
                    {
                        if (ev.RaycastHit.normal == new UnityEngine.Vector3(0, 1, 0))
                        {
                            ev.Player.Position = ev.RaycastHit.point + new UnityEngine.Vector3(0, 1, 0);
                        }
                    } 
                    else
                    {
                        ev.Player.Position = ev.Position;
                    }

                    Timing.CallDelayed(0.1f, () =>
                    {
                        if (KillAfterMiss)
                        {
                            ev.Player.Kill(DeathReasonUser);
                        } 
                        else
                        {
                            ev.Player.Hurt(ev.Distance, "Body is badly mutilated.");
                        }
                    });
                }
                else
                {
                    if (DespawnAfterUse)
                    {
                        Log.Debug($"VVUP Custom Items: SCP-2818, Despawn After Use is true, removing SCP-2818 from {ev.Player.Nickname}'s inventory");
                        ev.Player.RemoveItem(ev.Item);
                    }

                    Log.Debug($"VVUP Custom Items: SCP-2818, Killing {ev.Player.Nickname}");
                    ev.Player.Kill(DeathReasonUser);
                }
            });
        }

        private static void SpawnImpactGrenade(Vector3 position, Player owner)
        {
            try
            {
                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                grenade.FuseTime = 0.3f;
                grenade.SpawnActive(position, owner: owner);
            }
            catch (System.Exception exception)
            {
                Log.Error($"VVUP Custom Items: SCP-2818 failed to spawn its vanilla impact grenade: {exception}");
            }
        }
    }
}
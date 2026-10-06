using CustomPlayerEffects;
using Footprinting;
using LabApi.Features.Audio;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public class FireArrow : SpellBase
    {
        public override string Name => "Flecha de fuego";

        public override Color BaseColor => Color.red;

        public override int RankIndex => 0;

        public override float CastTime => 0.5f;

        CoroutineHandle coroutine;

        public override void Cast()
        {
            new Projectile(this, Caster.Player.Camera.position, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 20f, 10f, Caster.Player);
            PlaySound("cast");

            coroutine = Timing.CallPeriodically(0.42f, 0.1f, () =>
            {
                if (!Caster.Player.IsAlive)
                {
                    Timing.KillCoroutines(coroutine);
                    return;
                }

                new Projectile(this, Caster.Player.Camera.position + Caster.Player.Camera.forward * 0.4f, Caster.Player.Camera.rotation, Caster.Player.Camera.forward * 20f, 10f, Caster.Player);
                PlaySound("cast");
            });
        }

        public class Projectile(SpellBase spell, Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null) : CasterBase.MagicProjectileBase(spell, initialPosition, initialRotation, initialVelocity, lifetime, owner)
        {
            const float homingRangeSqr = 25f;
            float initialSpeed;
            Player homing;

            List<Player> targets;

            public override string SchematicName => "FireArrow";

            public override bool UseGravity => false;

            public override float CollisionRadius => 0.05f;

            public override void Init()
            {
                base.Init();
                initialSpeed = InitialVelocity.magnitude;
                targets = [.. Player.List.Where(p => p != Owner && p.IsAlive && (Owner == null || p.Faction != Owner.Faction))];
            }

            public override void Tick()
            {
                base.Tick();

                if (homing == null)
                {
                    Player targetHoming = null;
                    float dist = float.MaxValue;
                    foreach (Player p in targets)
                    {
                        float distSqr = (p.Position - Rigidbody.position).sqrMagnitude;

                        if (distSqr > homingRangeSqr)
                            continue;

                        if (dist > distSqr)
                        {
                            targetHoming = p;
                            dist = distSqr;
                        }
                    }
                    homing = targetHoming;
                }
                else
                {
                    Vector3 dir = (homing.Position - Rigidbody.position).normalized;
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    Rigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, lookRot, 320f * Time.fixedDeltaTime));
                    Rigidbody.linearVelocity = Rigidbody.transform.forward * initialSpeed;

                    if (!homing.IsAlive)
                    {
                        targets.Remove(homing);
                        homing = null;
                    }
                }
            }

            public override void Hit(Collision col, ReferenceHub player)
            {
                if (player != null)
                {
                    player.playerStats.DealDamage(new ExplosionDamageHandler(new Footprint(Owner.ReferenceHub), InitialVelocity, 40f * (player.IsSCP() ? 3f : 1f), 100, ExplosionType.Disruptor));
                    player.playerEffectsController.EnableEffect<Burned>(3f, true);

                    if (player.roleManager.CurrentRole is IFpcRole role)
                    {
                        role.FpcModule.Motor.JumpController.ForceJump(1f);
                    }

                    Owner?.SendHitMarker();
                }

                LightSourceToy toy = LightSourceToy.Create(Rigidbody.position, null, false);
                toy.Color = Color.red;
                toy.Intensity = 10f;
                LightExplosion.Create(toy, 40f);
                Destroy();
            }
        }
    }
}

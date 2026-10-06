using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using ProjectMER.Features;
using ProjectMER.Features.Objects;
using SwiftArcadeMode.Utils.Extensions;
using SwiftArcadeMode.Utils.Structures;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace SwiftArcadeMode.Utils.Projectiles
{
    public abstract class ProjectileBase
    {
        public static readonly LayerMask CollisionLayers = LayerMask.GetMask("Default", "Door", "Glass");
        public static readonly LayerMask IgnoreRaycastLayer = LayerMask.GetMask("Ignore Raycast");
        public Player Owner { get; private set; }
        public PrimitiveObjectToy Parent { get; set; }
        public SchematicObject Schematic { get; private set; }
        public Vector3 InitialPosition { get; private set; }
        public Quaternion InitialRotation { get; private set; }
        public Vector3 InitialVelocity { get; private set; }
        public abstract float CollisionRadius { get; }
        public Rigidbody Rigidbody { get; private set; }
        public SphereCollider Collider { get; private set; }
        public readonly Timer Lifetime = new();

        /// <summary>
        /// Leave empty for invisible projectile.
        /// </summary>
        public virtual string SchematicName => "";

        public ProjectileBase(Vector3 initialPosition, Quaternion initialRotation, Vector3 initialVelocity, float lifetime = 10f, Player owner = null)
        {
            InitialPosition = initialPosition;
            InitialRotation = initialRotation;
            InitialVelocity = initialVelocity;
            Lifetime.Reset(lifetime);
            Owner = owner;
            ProjectileManager.All.Add(this);

            Init();
        }

        public virtual void Init()
        {
            Parent = PrimitiveObjectToy.Create(InitialPosition, InitialRotation, networkSpawn: false);
            Parent.Flags = AdminToys.PrimitiveFlags.None;
            Parent.MovementSmoothing = 1;
            Parent.SyncInterval = 0f;
            Parent.Type = PrimitiveType.Cube;
            Parent.IsStatic = false;

            Rigidbody = Parent.GameObject.AddComponent<Rigidbody>();
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.linearVelocity = InitialVelocity;

            Parent.Spawn();

            if (!string.IsNullOrWhiteSpace(SchematicName))
            {
                Schematic = ObjectSpawner.SpawnSchematic(SchematicName.ApplySchematicPrefix(), default, Quaternion.identity);
                Schematic?.transform.SetParent(Parent.Transform, false);
            }

            Construct();

            Collider = Parent.GameObject.AddComponent<SphereCollider>();
            if (CollisionRadius > 0f)
                Collider.radius = CollisionRadius;
            else
                Logger.Error("Collision radius is non-positive for projectile " + GetType().FullName);
            Collider.excludeLayers = LayerMask.GetMask("Hitbox");

            if (Owner != null && Owner.RoleBase is IFpcRole role)
                Physics.IgnoreCollision(Collider, role.FpcModule.CharController, true);

            Parent.GameObject.AddComponent<ProjectileComponent>().projectile = this;
        }
        public virtual void Construct() { }
        public virtual void Tick()
        {
            Lifetime.Tick(Time.fixedDeltaTime);

            if (Lifetime.Ended)
                EndLife();
        }

        public virtual void EndLife() => Destroy();

        public virtual void Destroy()
        {
            ProjectileManager.All.Remove(this);
            Parent.Destroy();
        }

        public void OnCollide(Collision cols)
        {
            cols.collider.TryGetComponent(out ReferenceHub hub);
            Hit(cols, hub);
        }

        public abstract void Hit(Collision col, ReferenceHub hit);
    }

    public class LightExplosion : MonoBehaviour
    {
        public LightSourceToy Toy;
        public float FadeSpeed = 20f;

        public void FixedUpdate()
        {
            Toy.Intensity = Mathf.MoveTowards(Toy.Intensity, 0f, Time.fixedDeltaTime * FadeSpeed);
            if (Toy.Intensity <= 0f)
            {
                Destroy(this);
                Toy.Destroy();
            }
        }

        public static LightExplosion Create(LightSourceToy unspawned, float fadeSpeed = 20f)
        {
            unspawned.SyncInterval = 0;
            LightExplosion exp = unspawned.GameObject.AddComponent<LightExplosion>();
            exp.Toy = unspawned;
            exp.FadeSpeed = fadeSpeed;
            unspawned.Spawn();
            return exp;
        }
    }
}

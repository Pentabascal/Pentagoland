using ProjectMER.Features;
using ProjectMER.Features.Objects;
using SwiftArcadeMode.Utils.Structures;
using UnityEngine;

namespace SwiftArcadeMode.Utils.Visuals
{
    public class SchematicEffect : MonoBehaviour
    {
        public static SchematicEffect Create(string schematicName, Vector3 position, Quaternion rotation, Vector3 scale, float lifetime)
        {
            SchematicObject obj = ObjectSpawner.SpawnSchematic(schematicName, position, rotation, scale);

            if (obj == null)
                return null;

            SchematicEffect effect = obj.gameObject.AddComponent<SchematicEffect>();
            effect.Schematic = obj;
            effect.lifetime.Reset(lifetime);
            return effect;
        }

        public SchematicObject Schematic;

        private readonly Timer lifetime = new();

        private void FixedUpdate()
        {
            lifetime.Tick(Time.fixedDeltaTime);
            if (lifetime.Ended)
            {
                Destroy(this);
                Schematic.Destroy();
            }
        }
    }
}

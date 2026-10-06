using SwiftArcadeMode.Utils.Deployable;
using System.Collections.Generic;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    public abstract class SummonSpell : SpellBase
    {
        public virtual int Limit => 1;

        protected readonly List<DeployableBase> spawnedDeployables = [];

        public override void Cast()
        {
            if (Physics.Raycast(Caster.Player.Camera.position, Vector3.down, out RaycastHit hit, 4f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
                Spawn(hit.point);
        }

        public virtual void Spawn(Vector3 point)
        {
            if (Limit <= 0)
                return;

            Vector3 loc = point + Vector3.up;

            spawnedDeployables.RemoveAll(d => d.Destroyed);

            if (spawnedDeployables.Count < Limit)
                spawnedDeployables.Add(Create(loc));
            else
            {
                DeployableBase dep = spawnedDeployables[0];
                dep.Position = loc;
                if (Limit > 1)
                {
                    spawnedDeployables.Remove(dep);
                    spawnedDeployables.Add(dep);
                }
            }
        }

        public abstract DeployableBase Create(Vector3 loc);
    }
}

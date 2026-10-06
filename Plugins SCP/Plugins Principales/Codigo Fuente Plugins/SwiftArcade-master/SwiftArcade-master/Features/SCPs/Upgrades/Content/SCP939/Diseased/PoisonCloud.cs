using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    public class PoisonCloud(UpgradePathPerkBase parent) : UpgradeBase<Diseased>(parent)
    {
        public override string Name => "Nube tóxica";

        public override string Description => $"Infliges {Damage} HP/s a todos los afectados por amnesia.";

        public float Damage => 3f;

        public override void Tick()
        {
            base.Tick();
            foreach (Player p in Player.List)
                if (p.HasEffect<AmnesiaVision>())
                    p.Damage(Damage * Time.fixedDeltaTime, Player, armorPenetration: 100);
        }
    }
}

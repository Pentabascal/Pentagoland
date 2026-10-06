using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerBall : GamblerEffectBase
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Me gustan mucho estas pelotas...";

        public override void Effect(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                Pickup pick = Pickup.Create(ItemType.SCP018, player.Position);
                pick.Spawn();
                pick.Rigidbody.AddForce(Random.insideUnitSphere * Random.Range(5f, 40f), ForceMode.Impulse);
            }
        }
    }
}

using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerGravityPositive : GamblerEffectBase
    {
        public override bool Positive => true;

        public override int Weight => 1;

        public override string Explanation => "Tu gravedad se ha reducido a la mitad.";

        public override void Effect(Player player) => player.Gravity /= 2f;
    }
}

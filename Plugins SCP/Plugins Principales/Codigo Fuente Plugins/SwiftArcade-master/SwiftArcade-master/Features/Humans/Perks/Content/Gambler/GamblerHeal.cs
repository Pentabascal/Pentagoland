using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerHeal : GamblerEffectBase
    {
        public override bool Positive => true;

        public override int Weight => 3;

        public override string Explanation => "Te has curado.";

        public override void Effect(Player player) => player.Heal(15f);
    }
}

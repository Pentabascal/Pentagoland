using CustomPlayerEffects;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerHands : GamblerStatusEffectBase<SeveredHands>
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Oye, colega, ¿necesitas una mano?";
    }
}

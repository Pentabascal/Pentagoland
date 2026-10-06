using CustomPlayerEffects;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerEyes : GamblerStatusEffectBase<SeveredEyes>
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Gracias por esos ojos.";
    }
}

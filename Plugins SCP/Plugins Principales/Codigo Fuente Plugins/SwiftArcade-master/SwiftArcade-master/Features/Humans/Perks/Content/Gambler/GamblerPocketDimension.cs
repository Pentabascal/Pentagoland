using CustomPlayerEffects;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerPocketDimension : GamblerStatusEffectBase<PocketCorroding>
    {
        public override bool Positive => false;

        public override int Weight => 1;

        public override string Explanation => "Volvamos a probar suerte.";
    }
}

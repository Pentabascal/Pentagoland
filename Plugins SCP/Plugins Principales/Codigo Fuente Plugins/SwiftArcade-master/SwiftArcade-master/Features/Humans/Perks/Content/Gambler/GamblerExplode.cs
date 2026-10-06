using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerExplode : GamblerEffectBase
    {
        public override bool Positive => false;
        public override int Weight => 1;
        public override string Explanation => "¡Bum!";

        public override void Effect(Player player) => TimedGrenadeProjectile.SpawnActive(player.Position, ItemType.GrenadeHE, player, 0.1f);
    }
}

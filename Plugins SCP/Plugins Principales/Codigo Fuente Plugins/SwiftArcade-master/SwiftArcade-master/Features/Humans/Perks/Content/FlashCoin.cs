using CustomPlayerEffects;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("FlashCoin", Rarity.Legendary)]
    public class FlashCoin(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Modo Dios";

        public override string Description => "La mejor ventaja.";

        public override void Init()
        {
            base.Init();
            Player.EnableEffect<Flashed>(1, 10, true);
        }
    }
}

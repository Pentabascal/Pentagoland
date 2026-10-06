using SwiftArcadeMode.Features.Humans.Perks.Content;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    public class SlotUpgrade(UpgradePathPerkBase parent) : UpgradeBase<RandomPerks>(parent)
    {
        public override string Name => "Mejora de espacio";

        public override string Description => "Obtienes una mejora de espacio.";

        public override void Init()
        {
            base.Init();
            if (!Player.HasPerk(typeof(PerkSlotUpgrade)))
                Player.GivePerk(typeof(PerkSlotUpgrade));
        }
    }
}

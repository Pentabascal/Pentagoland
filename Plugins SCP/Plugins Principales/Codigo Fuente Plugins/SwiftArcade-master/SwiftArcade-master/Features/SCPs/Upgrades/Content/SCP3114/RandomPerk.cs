using SwiftArcadeMode.Utils.Extensions;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    public class RandomPerk(UpgradePathPerkBase parent) : UpgradeBase<RandomPerks>(parent)
    {
        public override string Name => $"Ventaja aleatoria ({Rarity switch
        {
            SwiftArcadeMode.Features.Rarity.Common => "Común",
            SwiftArcadeMode.Features.Rarity.Uncommon => "Poco común",
            SwiftArcadeMode.Features.Rarity.Rare => "Rara",
            SwiftArcadeMode.Features.Rarity.Epic => "Épica",
            SwiftArcadeMode.Features.Rarity.Legendary => "Legendaria",
            SwiftArcadeMode.Features.Rarity.Mythic => "Mítica",
            SwiftArcadeMode.Features.Rarity.Secret => "Secreta",
            _ => Rarity.ToString()
        }})";

        public override string Description => "Obtienes una ventaja aleatoria.";

        public Rarity Rarity { get; set; } = Enum.GetValues(typeof(Rarity)).ToArray<Rarity>().GetRandom();

        public override void Init()
        {
            base.Init();
            PerkAttribute att = PerkManager.GetRandomPerk(p => p.Rarity == Rarity);
            if (!Player.HasPerk(att.Perk))
                Player.GivePerk(att);
        }
    }
}

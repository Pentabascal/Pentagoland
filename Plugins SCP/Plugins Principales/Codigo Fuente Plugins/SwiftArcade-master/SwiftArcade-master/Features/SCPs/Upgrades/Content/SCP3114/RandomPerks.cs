using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP3114
{
    [UpgradePath(RoleTypeId.Scp3114)]
    [Perk("3114.RandomPerks", Rarity.Legendary, PerkRestriction.SCP)]
    public class RandomPerks(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override int SlotUsage => 0;

        public override string PathName => "Ventajas aleatorias";

        public override string PathDescription => "Obtienes una ventaja aleatoria con cada mejora.\nEsta rama de mejoras ocupa 0 espacios.";

        public override Type[] AllUpgrades => [
            typeof(SlotUpgrade),
            typeof(RandomPerk),
            typeof(RandomPerk),
            typeof(RandomPerk),
            typeof(RandomPerk),
            ];
    }
}

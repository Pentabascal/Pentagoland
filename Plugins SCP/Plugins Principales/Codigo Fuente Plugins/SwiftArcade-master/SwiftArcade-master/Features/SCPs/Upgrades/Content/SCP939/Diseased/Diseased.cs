using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Diseased
{
    [UpgradePath(PlayerRoles.RoleTypeId.Scp939)]
    [Perk("939.Diseased", Rarity.Epic, PerkRestriction.SCP)]
    public class Diseased(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Infeccioso";

        public override string PathDescription => "Propaga la rabia.";

        public override Type[] AllUpgrades => [
            typeof(PoisonCloud),
            typeof(Bleeding)
            ];
    }
}

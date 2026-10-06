using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    [UpgradePath(RoleTypeId.Scp173)]
    [Perk("173.Scouter", Rarity.Rare, PerkRestriction.SCP)]
    public class Scouter(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Explorador";

        public override string PathDescription => "Se centra en la movilidad y la obtención de información.";

        public override Type[] AllUpgrades => [
            typeof(TacticalRetreat),
            typeof(Phantom),
            ];
    }
}

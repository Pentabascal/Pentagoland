using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    [UpgradePath(RoleTypeId.Scp106)]
    [Perk("106.EndlessDecay", Rarity.Epic, PerkRestriction.SCP)]
    public class EndlessDecay(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Descomposición interminable";

        public override string PathDescription => "Descompone todo tipo de cosas.";

        public override Type[] AllUpgrades => [
            typeof(FleshDecay),
            typeof(LimbBreak),
            typeof(GunMelter)
            ];
    }
}

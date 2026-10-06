using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Tank
{
    [UpgradePath(RoleTypeId.Scp173)]
    [Perk("173.Tank", Rarity.Rare, PerkRestriction.SCP)]
    public class Tank(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Tanque";

        public override string PathDescription => "Se centra en reducir el daño recibido.";

        public override Type[] AllUpgrades => [
            typeof(DefensiveSpeed),
            typeof(BlastResistant)
            ];
    }
}

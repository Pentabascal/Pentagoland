using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.Overlord
{
    [UpgradePath(RoleTypeId.Scp049)]
    [Perk("049.Overlord", Rarity.Epic, PerkRestriction.SCP)]
    public class Overlord(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Señor supremo";

        public override string PathDescription => "¡Sácales partido a tus zombis!";

        public override Type[] AllUpgrades => [
            typeof(Siphon),
            typeof(Soulbound)
            ];
    }
}

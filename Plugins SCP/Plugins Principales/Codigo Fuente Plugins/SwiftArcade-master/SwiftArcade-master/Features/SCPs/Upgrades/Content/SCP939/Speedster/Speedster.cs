using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    [UpgradePath(PlayerRoles.RoleTypeId.Scp939)]
    [Perk("939.Speedster", Rarity.Uncommon, PerkRestriction.SCP)]
    public class Speedster(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Velocista";

        public override string PathDescription => "Se centra en la velocidad.";

        public override Type[] AllUpgrades => [
            typeof(Endurance),
            typeof(ReadyUp)
            ];
    }
}

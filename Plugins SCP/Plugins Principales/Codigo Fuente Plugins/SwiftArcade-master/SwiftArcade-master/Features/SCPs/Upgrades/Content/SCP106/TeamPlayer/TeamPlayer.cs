using PlayerRoles;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    [UpgradePath(RoleTypeId.Scp106)]
    [Perk("106.TeamPlayer", Rarity.Legendary, PerkRestriction.SCP)]
    public class TeamPlayer(PerkInventory inv) : UpgradePathPerkBase(inv)
    {
        public override string PathName => "Compañero de equipo";

        public override string PathDescription => "Proporciona bonificaciones para el equipo.";

        public override Type[] AllUpgrades => [
            typeof(LifeConsumption),
            typeof(SharedConsumption)
            ];
    }
}

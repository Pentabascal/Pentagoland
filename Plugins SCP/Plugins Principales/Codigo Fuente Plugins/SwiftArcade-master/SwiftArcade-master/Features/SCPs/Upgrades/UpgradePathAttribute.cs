using PlayerRoles;
using SwiftArcadeMode.Utils.Interfaces;
using System;

namespace SwiftArcadeMode.Features.SCPs.Upgrades
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UpgradePathAttribute(params RoleTypeId[] roles) : Attribute, IWeight
    {
        public readonly RoleTypeId[] Roles = roles;

        public PerkAttribute Perk { get; set; }

        public int Weight => Perk == null ? 0 : Perk.Weight;
    }
}

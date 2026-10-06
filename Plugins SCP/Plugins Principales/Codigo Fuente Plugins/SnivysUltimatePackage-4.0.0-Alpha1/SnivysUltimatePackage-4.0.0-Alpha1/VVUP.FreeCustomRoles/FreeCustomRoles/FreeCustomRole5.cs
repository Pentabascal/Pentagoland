using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.FreeCustomRoles.FreeCustomRoles
{
    [CustomRole(RoleTypeId.None)]
    public class FreeCustomRole5 : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;
        public StartTeam StartTeam { get; set; } = StartTeam.Other;
        public override uint Id { get; set; } = 105;
        public override RoleTypeId Role { get; set; } = RoleTypeId.None;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "Rol personalizado libre 5";
        public override string Description { get; set; } = "Rol personalizado libre";
        public override string CustomInfo { get; set; } = "Rol personalizado libre";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
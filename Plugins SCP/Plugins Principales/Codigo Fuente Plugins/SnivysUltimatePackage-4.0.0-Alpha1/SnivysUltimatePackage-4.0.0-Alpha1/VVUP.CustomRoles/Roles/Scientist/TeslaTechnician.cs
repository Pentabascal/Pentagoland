using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scientist
{
    public class TeslaTechnician : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        public override uint Id { get; set; } = 64;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#FFFF7C>Técnico de Tesla</color>";
        public override string Description { get; set; } = "Científico que trabaja habitualmente con las puertas Tesla.";
        public override string CustomInfo { get; set; } = "Técnico de Tesla";
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardScientist.ToString(),
            ItemType.Medkit.ToString(),
        };
        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new IgnoreTeslas()
            {
                Name = "Ignorar puertas Tesla [Pasiva]",
            },
        };
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
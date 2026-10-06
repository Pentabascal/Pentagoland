using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class ClassDDruggy : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 63;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#FF8E00>Drogodependiente de Clase-D</color>";

        public override string Description { get; set; } =
            "Un Clase-D con una fuerte adicción a las drogas.";

        public override string CustomInfo { get; set; } = "Drogodependiente de Clase-D";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Painkillers.ToString(),
            ItemType.Painkillers.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new Addicted
            {
                Name = "Adicción [Pasiva]",
            },
        };
        
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
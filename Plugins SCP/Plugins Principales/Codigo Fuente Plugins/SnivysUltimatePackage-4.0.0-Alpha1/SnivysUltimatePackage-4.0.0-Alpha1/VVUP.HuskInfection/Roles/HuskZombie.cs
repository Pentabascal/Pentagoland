using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;
using VVUP.HuskInfection.Abilities;
using YamlDotNet.Serialization;

namespace VVUP.HuskInfection.Roles
{
    public class HuskZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;
        public override uint Id { get; set; } = 56;
        public override int MaxHealth { get; set; } = 350;
        public override string Name { get; set; } = "<color=#FF0000>Husk</color>";
        public override string Description { get; set; } = "A diferencia de otros zombis, el husk nace cuando una infección convierte a un huésped vivo en su presa y acaba apoderándose de él.";
        public override string CustomInfo { get; set; } = "Husk";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Other;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
           new ApplyHuskInfection()
           {
               Name = "Infectar al golpear [Pasiva]",
           },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 0,
        };
        
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class ExplosiveZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 25;
        public override uint Id { get; set; } = 42;
        public override int MaxHealth { get; set; } = 500;
        public override string Name { get; set; } = "<color=#FF0000>SCP-049-2 balístico</color>";
        public override string Description { get; set; } = "Un zombi que explota al morir.";
        public override string CustomInfo { get; set; } = "SCP-049-2 balístico";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new Martyrdom
            {
                Name = "Explosión al morir [Pasiva]",
                Description = "Hace que explotes al morir.",
                ExplosiveFuse = 0.5f,
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
        };
        
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
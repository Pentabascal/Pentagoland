using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class InfectedZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 20;
        public override uint Id { get; set; } = 53;
        public override int MaxHealth { get; set; } = 350;
        public override string Name { get; set; } = "<color=#FF0000>Zombi infectado</color>";
        public override string Description { get; set; } = "Un zombi que convierte en zombi a cada jugador al que mata.";
        public override string CustomInfo { get; set; } = "Zombi infectado";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new TeamConvertOnKill()
            {
                Name = "Convertir al matar [Pasiva]",
                Description = "Al matar a un jugador, lo convertirás en zombi.",
                ConvertToRole = RoleTypeId.Scp0492,
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
using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class SoundBreaker173 : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;
        public override uint Id { get; set; } = 58;
        public override int MaxHealth { get; set; } = 3800;
        public override string Name { get; set; } = "<color=#FF0000>SCP-173 rompesonidos</color>";
        public override string Description { get; set; } = "Reinicia la recarga del parpadeo y reduce el intervalo y el alcance del siguiente.";
        public override string CustomInfo { get; set; } = "SCP-173 rompesonidos";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp173;

        public StartTeam StartTeam { get; set; } = StartTeam.Scp173;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new SoundBreaker()
            {
                Name = "Rompesonidos [Activa]",
                Description = "Reinicia la recarga del parpadeo y reduce el intervalo y el alcance del siguiente.",
            }
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.Scp173,
                    Chance = 100,
                },
            },
        };

        public override string AbilityUsage { get; set; } = "Usa la habilidad Rompesonidos con la tecla asignada en el servidor (configúrala en ESC > Ajustes > Ajustes del servidor).";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
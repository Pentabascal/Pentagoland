using System.Collections.Generic;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Other
{
    public class PaperMan : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 0;
        public StartTeam StartTeam { get; set; } = StartTeam.Other;
        public override uint Id { get; set; } = 61;
        public override int MaxHealth { get; set; } = 50;
        public override string Name { get; set; } = "Hombre de papel";
        public override string Description { get; set; } = "Eres tan plano como una hoja de papel.";
        public override string CustomInfo { get; set; } = "Hombre de papel";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Tutorial;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility
            {
                Name = "Escala [Pasiva]",
                Description = "Te aplana como una hoja de papel.",
                ScaleForPlayers = new UnityEngine.Vector3(1, 1, 0.1f),
            },
        };
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class DwarfZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 20;
        public override uint Id { get; set; } = 43;
        public override int MaxHealth { get; set; } = 200;
        public override string Name { get; set; } = "<color=#FF0000>SCP-049-2 enano</color>";
        public override string Description { get; set; } = "Un zombi más pequeño.";
        public override string CustomInfo { get; set; } = "SCP-049-2 enano";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility()
            {
                Name = "Enano [Pasiva]",
                Description = "Te hace pequeño como un zombi enano.",
                ScaleForPlayers = new Vector3(0.75f, 0.75f, 0.75f),
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
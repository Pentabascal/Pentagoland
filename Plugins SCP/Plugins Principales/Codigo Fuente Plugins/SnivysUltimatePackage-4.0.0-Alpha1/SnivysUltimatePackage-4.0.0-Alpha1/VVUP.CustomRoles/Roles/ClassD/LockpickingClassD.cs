using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class LockpickingClassD : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 46;
        public override int MaxHealth { get; set; } = 90;
        public override string Name { get; set; } = "<color=#FF8E00>Forzapuertas de Clase-D</color>";
        public override string Description { get; set; } = "Puede forzar cerraduras de puertas.";
        public override string CustomInfo { get; set; } = "Forzapuertas de Clase-D";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new CustomRoleEscape
            {
                Name = "Escape con rol personalizado [Pasiva]",
                Description = "Si escapas como forzapuertas, conservarás tu inventario.",
                EscapeToRegularRole = true,
                RegularRole = RoleTypeId.ChaosConscript,
                AllowCuffedCustomRoleChange = false,
                AllowUncuffedCustomRoleChange = false,
                SaveInventory = true
            },
            new DoorPicking
            {
                Name = "Forzar cerraduras [Activa]",
                Description = "Te permite forzar cualquier puerta durante un breve periodo, aunque está sujeto a ciertos factores externos.",
            }
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        
        public override string AbilityUsage { get; set; } = "Usa el botón de noclip [Alt izquierdo] para cambiar de habilidad y activarla. Pulsa dos veces para cambiar y una vez para activar.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
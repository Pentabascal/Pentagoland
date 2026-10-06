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
    public class TheoreticalPhysicistScientist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public override uint Id { get; set; } = 49;
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;
        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;
        
        public override int MaxHealth { get; set; } = 100;
        
        public override string Name { get; set; } = "<color=#FFFF7C>Científico físico teórico</color>";

        public override string Description { get; set; } =
            "Científico convencido de que la dimensión de bolsillo es mucho más de lo que parece.";
        
        public override string CustomInfo { get; set; } = "Científico físico teórico";
        
        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.KeycardScientist.ToString(),
            ItemType.Painkillers.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new PocketDimensionEscapeChance()
            {
                Name = "Escape de la dimensión de bolsillo [Pasiva]",
                Description = "Te permite escapar con seguridad de la dimensión de bolsillo dos veces.",
                EscapeChance = 100,
                AmountOfAllowedEscapes = 2,
                CustomDeathReason = "Has alcanzado el límite garantizado de escapes de la dimensión de bolsillo.",
                RemoveTraumatizedOnEscape = true,
            },
            new CustomRoleEscape
            {
                Name = "Escape con rol personalizado [Pasiva]",
                Description = "Si escapas con este rol personalizado, conservarás tu inventario.",
                EscapeToRegularRole = true,
                RegularRole = RoleTypeId.NtfSpecialist,
                AllowCuffedCustomRoleChange = false,
                AllowUncuffedCustomRoleChange = false,
                SaveInventory = true
            },
        };
        
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
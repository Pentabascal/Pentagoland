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
    public class ContainmentScientist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;

        public override uint Id { get; set; } = 30;

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#FFFF7C>Científico ingeniero de contención</color>";

        public override string Description { get; set; } =
            @"\n\n\n\nCientífico que aparece en la zona de entrada con un equipamiento distinto.\nNo puedes escapar para convertirte en miembro de los MTF.\nPuedes ser detenido y escapar para unirte a la Insurgencia del Caos.";

        public override string CustomInfo { get; set; } = "Científico ingeniero de contención";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>()
            {
                new()
                {
                    Role = RoleTypeId.FacilityGuard,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Medkit.ToString(),
            ItemType.KeycardContainmentEngineer.ToString(),
            ItemType.Adrenaline.ToString(),
            ItemType.Radio.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new RestrictedEscape
            {
                Name = "Escape restringido [Pasiva]",
                Description = "Restringe las condiciones de escape del científico ingeniero de contención.",
                EscapeTextUncuffed = "Como científico ingeniero de contención, no puedes escapar si no estás detenido.",
                EscapeTextTime = 5,
                UseHints = true,
                AllowedCuffedEscape = true,
            },
        };
        
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
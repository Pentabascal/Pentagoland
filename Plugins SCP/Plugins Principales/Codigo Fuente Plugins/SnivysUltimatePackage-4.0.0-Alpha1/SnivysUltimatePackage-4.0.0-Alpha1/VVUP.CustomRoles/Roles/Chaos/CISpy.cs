using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class CISpy : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;

        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;

        public override uint Id { get; set; } = 40;

        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfPrivate;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "Espía de la Insurgencia del Caos";
        public override string Description { get; set; } = "Insurgente del Caos disfrazado de miembro de los MTF.";

        public override string CustomInfo { get; set; } = "Soldado de los MTF";

        public override bool KeepInventoryOnSpawn { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.NtfPrivate,
                    Chance = 100,
                },
            },
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new Disguised
            {
                Name = "Disfraz [Pasiva]",
                Description = "Gestiona todo lo relacionado con el disfraz.",
                DisguisedFriendlyFireText = "Ese miembro de los MTF en realidad está del lado de la Insurgencia del Caos.",
                DisguisedTrueTeam = Disguised.TrueTeamEnum.Ci,
                DisguisedHintDisplay = false,
                DisguisedTextDisplayTime = 5,
            },
            new RemoveDisguise
            {
                Name = "Quitar disfraz [Activa]",
                Description =
                    "Al activarla, te quitarás el disfraz y no podrás volver a ponértelo. Úsala con cuidado.",
                Duration = 0,
                Cooldown = 5,
                RestorePreviousInventory = false,
                SavePosition = true,
            },
        };
        
        public override string AbilityUsage { get; set; } = "Usa el botón de noclip [Alt izquierdo] para cambiar de habilidad y activarla. Pulsa dos veces para cambiar y una vez para activar.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
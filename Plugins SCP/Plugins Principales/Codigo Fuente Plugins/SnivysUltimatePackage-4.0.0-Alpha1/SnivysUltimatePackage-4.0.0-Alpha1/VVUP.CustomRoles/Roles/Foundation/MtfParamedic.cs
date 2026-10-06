using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class MtfParamedic : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;
        public override uint Id { get; set; } = 50;
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfSpecialist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#0096FF>Sanitario de los MTF</color>";

        public override string Description { get; set; } =
            "Sanitario con equipo médico adicional.";
        public override string CustomInfo { get; set; } = "Sanitario de los MTF";
        
        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#0096FF>Phantom Pulse</color>",
            ItemType.Medkit.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorCombat.ToString()
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 80
            },
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.NtfSpecialist,
                    Chance = 100,
                },
            },
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            /*new RevivingMist
            {
                Name = "Niebla de reanimación [Activa]",
                Description = "Te permite reanimar a tus compañeros.",
                Duration = 1,
                Cooldown = 180,
                ReviveRadius = 5,
                ReviveTimeWindow = 30f,
                ReviveHealthPercent = 30f,
                ReviveMessage = "¡Un sanitario te ha reanimado!",
                ReviveMessageTime = 5,
                ReviveTeammatesOnly = true,
            },*/
            new HealingMist
            {
                Name = "Niebla curativa [Activa]",
                Description =
                    "Libera sustancias químicas que curan y protegen brevemente a los aliados.",
                Duration = 15,
                Cooldown = 180,
                HealAmount = 6,
                ProtectionAmount = 45,
            },
        };
        
        public override string AbilityUsage { get; set; } = "Usa el botón de noclip [Alt izquierdo] para cambiar de habilidad y activarla. Pulsa dos veces para cambiar y una vez para activar.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class TelepathicChaos : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;

        public StartTeam StartTeam { get; set; } = StartTeam.Chaos;

        public override uint Id { get; set; } = 38;

        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosRifleman;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#008f1e>Caótico telepático</color>";

        public override string Description { get; set; } =
            "Puedes SENTIR la presencia de enemigos de la Insurgencia del Caos que estén cerca.";

        public override string CustomInfo { get; set; } = "Caótico telepático";

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.ChaosConscript,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.GunA7}",
            $"{ItemType.KeycardChaosInsurgency}",
            $"{ItemType.Medkit}",
            $"{ItemType.ArmorCombat}",
            $"{ItemType.Painkillers}",
            "<color=#6600CC>Obscurus Veil-5</color>",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato762, 120
            },
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new Detect
            {
                Name = "Detectar [Activa]",
                Description = "Detecta a los objetivos de la Insurgencia del Caos.",
                Duration = 0,
                Cooldown = 120,
                DetectRange = 30,
                ShowMissingRoles = true,
                RoleNames = new Dictionary<RoleTypeId, string>()
                {
                    {RoleTypeId.Scientist, "Científico"},
                    {RoleTypeId.NtfCaptain, "Capitán de los MTF"},
                    {RoleTypeId.NtfPrivate, "Soldado de los MTF"},
                    {RoleTypeId.NtfSergeant, "Sargento de los MTF"},
                    {RoleTypeId.NtfSpecialist, "Especialista de los MTF"},
                    {RoleTypeId.FacilityGuard, "Guardia de la instalación"},
                    {RoleTypeId.Scp049, "SCP-049"},
                    {RoleTypeId.Scp0492, "SCP-049-2"},
                    {RoleTypeId.Scp079, "SCP-079"},
                    {RoleTypeId.Scp096, "SCP-096"},
                    {RoleTypeId.Scp106, "SCP-106"},
                    {RoleTypeId.Scp173, "SCP-173"},
                    {RoleTypeId.Scp939, "SCP-939"},
                    {RoleTypeId.Scp3114, "SCP-3114"}
                },
            },
        };
        
        public override string AbilityUsage { get; set; } = "Usa el botón de noclip [Alt izquierdo] para cambiar de habilidad y activarla. Pulsa dos veces para cambiar y una vez para activar.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
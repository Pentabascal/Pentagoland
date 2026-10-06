using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using InventorySystem.Items.Usables.Scp330;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class MtfJuggernaut : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;

        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;

        public override uint Id { get; set; } = 65;

        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfPrivate;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#0096FF>Tanque de asalto de los MTF</color>";
        public override string Description { get; set; } = "Miembro de los MTF especializado en explosivos.";

        public override string CustomInfo { get; set; } = "Tanque de asalto de los MTF";

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

        public override List<string> Inventory { get; set; } = new()
        {
            $"{ItemType.KeycardMTFPrivate}",
            $"{ItemType.GrenadeHE}",
            $"{ItemType.GrenadeHE}",
            $"{ItemType.GrenadeFlash}",
            $"{ItemType.ArmorHeavy}",
            "<color=#FF0000>ADATS</color>",
            "<color=#6600CC>Obscurus Veil-5</color>",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Ammo44Cal, 8
            },
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new GivingCandyAbility
            {
                Name = "Dulce inicial [Pasiva]",
                Description = "Te da un caramelo rosa al aparecer.",
                StartingCandy =
                {
                    CandyKindID.Pink,
                },
            },
            new CustomRoleEscape
            {
                Name = "Escape con rol personalizado [Pasiva]",
                Description =
                    "Si te detienen como miembro de los MTF y te escoltan fuera, tienes garantizado convertirte en especialista en demoliciones de los MTF.",
                UncuffedEscapeCustomRole = "",
                CuffedEscapeCustomRole = "<color=#0096FF>Especialista en demoliciones de los MTF</color>",
                AllowUncuffedCustomRoleChange = false,
                AllowCuffedCustomRoleChange = true,
                SaveInventory = true,
            },
        };

        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore]
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore]
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}

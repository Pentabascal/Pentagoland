using System.Collections.Generic;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scientist
{
    public class Biochemist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Scientist;

        public override uint Id { get; set; } = 32;

        public override RoleTypeId Role { get; set; } = RoleTypeId.Scientist;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#FFFF7C>Bioquímico</color>";

        public override string Description { get; set; } =
            "Científico especializado en alteraciones biológicas. Puede curar pasivamente a otras personas y explotar al morir.";

        public override string CustomInfo { get; set; } = "Bioquímico";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Medkit.ToString(),
            ItemType.Adrenaline.ToString(),
            ItemType.Radio.ToString(),
        };

        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new CustomRoleEscape
            {
                Name = "Escape con rol personalizado [Pasiva]",
                Description = "Si escapas como bioquímico, se te asignará un rol personalizado.",
                UncuffedEscapeCustomRole = "<color=#0096FF>Especialista en demoliciones de los MTF</color>",
                CuffedEscapeCustomRole = "<color=#008f1e>Caótico explosivo</color>",
                AllowUncuffedCustomRoleChange = true,
                AllowCuffedCustomRoleChange = true,
                SaveInventory = true
            },
            new Martyrdom
            {
                Name = "Explosión al morir [Pasiva]",
                Description = "Explotas al morir.",
                ExplosiveFuse = 0.1f
            },
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
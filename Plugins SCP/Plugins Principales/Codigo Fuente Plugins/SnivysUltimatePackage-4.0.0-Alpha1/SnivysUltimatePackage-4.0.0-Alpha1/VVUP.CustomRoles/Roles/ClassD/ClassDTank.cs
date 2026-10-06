using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using VVUP.Base;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class ClassDTank : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 5;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 52;
        public override int MaxHealth { get; set; } = 300;
        public override string Name { get; set; } = "<color=#FF8E00>Tanque de Clase-D</color>";
        public override string Description { get; set; } = "Eres un Clase-D con mayor resistencia, capaz de soportar más daño que los demás.";
        public override string CustomInfo { get; set; } = "Tanque de Clase-D";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
        public override List<CustomAbility>? CustomAbilities { get; set; } = new()
        {
            new EffectEnabler
            {
                Name = "Velocidad reducida [Pasiva]",
                Description = "Reduce tu velocidad.",
                EffectsToApply = new List<ApplyEffects>
                {
                    new ApplyEffects()
                    {
                        EffectType = EffectType.Slowness,
                        Intensity = 30,
                    }
                },
            },
            new ScaleAbility
            {
                Name = "Aumento de tamaño [Pasiva]",
                Description = "Aumenta tu tamaño.",
                ScaleForPlayers = new Vector3(1.1f, 1.1f, 1.1f)
            },
            new CustomRoleEscape
            {
                Name = "Escape con rol personalizado [Pasiva]",
                Description = "Si escapas como tanque de Clase-D, conservarás tu inventario.",
                EscapeToRegularRole = true,
                RegularRole = RoleTypeId.ChaosConscript,
                AllowCuffedCustomRoleChange = false,
                AllowUncuffedCustomRoleChange = false,
                SaveInventory = true
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };
        
        public override string AbilityUsage { get; set; } = "Tienes habilidades pasivas que no requieren activación mediante botones.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
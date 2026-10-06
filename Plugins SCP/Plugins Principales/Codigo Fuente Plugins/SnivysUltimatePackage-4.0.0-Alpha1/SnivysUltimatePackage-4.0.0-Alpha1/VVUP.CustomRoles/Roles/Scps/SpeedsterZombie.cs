using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.Base;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class SpeedsterZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 20;
        public override uint Id { get; set; } = 55;
        public override int MaxHealth { get; set; } = 300;
        public override string Name { get; set; } = "<color=#FF0000>Zombi veloz</color>";
        public override string Description { get; set; } = "Un zombi muy rápido.";
        public override string CustomInfo { get; set; } = "Zombi veloz";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new EffectEnabler()
            {
                Name = "Aumento de velocidad [Pasiva]",
                Description = "Aumenta tu velocidad de movimiento.",
                EffectsToApply = new List<ApplyEffects>()
                {
                    new()
                    {
                        EffectType = EffectType.MovementBoost,
                        Intensity = 20,
                    }
                },
            }
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
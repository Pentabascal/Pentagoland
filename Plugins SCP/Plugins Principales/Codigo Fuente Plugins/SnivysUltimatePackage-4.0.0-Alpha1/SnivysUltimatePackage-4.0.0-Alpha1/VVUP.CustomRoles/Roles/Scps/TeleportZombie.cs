using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.Base;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Scps
{
    public class TeleportZombie : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 10;
        public override uint Id { get; set; } = 57;
        public override int MaxHealth { get; set; } = 300;
        public override string Name { get; set; } = "<color=#FF0000>Zombi teletransportador</color>";
        public override string Description { get; set; } = "Un zombi capaz de teletransportarse.";
        public override string CustomInfo { get; set; } = "Zombi teletransportador";
        public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
        
        public StartTeam StartTeam { get; set; } = StartTeam.Scp | StartTeam.Revived;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new EffectEnabler()
            {
                Name = "Lentitud [Pasiva]",
                Description = "Reduce tu velocidad de movimiento.",
                EffectsToApply = new List<ApplyEffects>()
                {
                    new()
                    {
                        EffectType = EffectType.Slowness,
                        Intensity = 20,
                    }
                },
            },
            new Teleport
            {
                Name = "Teletransporte [Activa]",
                Description = "Te teletransporta a una ubicación aleatoria dentro de cierto alcance.",
            },
        };
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
        };
        
        public override string AbilityUsage { get; set; } = "Usa el botón de noclip [Alt izquierdo] para cambiar de habilidad y activarla. Pulsa dos veces para cambiar y una vez para activar.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
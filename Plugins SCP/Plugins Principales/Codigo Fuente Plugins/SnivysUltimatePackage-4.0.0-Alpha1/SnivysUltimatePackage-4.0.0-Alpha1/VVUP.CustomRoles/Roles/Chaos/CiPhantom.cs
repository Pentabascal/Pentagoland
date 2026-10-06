using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using VVUP.CustomRoles.Abilities.Active;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Chaos
{
    public class CiPhantom : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public override uint Id { get; set; } = 44;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#008f1e>Fantasma del Caos</color>";
        public override string Description { get; set; } = "Insurgente del Caos especializado en espionaje.";
        public override string CustomInfo { get; set; } = "Fantasma del Caos";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ChaosConscript;

        public StartTeam StartTeam { get; set; } = StartTeam.Guard;

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new RoleSpawnPoint
                {
                    Role = RoleTypeId.FacilityGuard,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new List<string>
        {
            "<color=#6600CC>PB-42</color>",
            "<color=#6600CC>Obscurus Veil-5</color>",
            $"{ItemType.GunCrossvec}",
            $"{ItemType.Medkit}",
            $"{ItemType.KeycardChaosInsurgency}",
            $"{ItemType.Adrenaline}",
            $"{ItemType.ArmorCombat}",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 80
            },
        };
        
        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new RestrictedItems
            {
                Name = "Objetos restringidos [Pasiva]",
                Description = "No puedes usar SCP-268.",
                RestrictedItemList = new List<ItemType>()
                {
                    ItemType.SCP268,
                },
                RestrictUsingItems = true,
                RestrictDroppingItems = true,
                RestrictPickingUpItems = true,
            },
            new ActiveCamo
            {
                Name = "Camuflaje activo [Activa]",
                Description = "Activa el camuflaje, que se volverá a aplicar al realizar la mayoría de acciones.",
            },
        };
        
        public override string AbilityUsage { get; set; } = "Usa el botón de noclip [Alt izquierdo] para cambiar de habilidad y activarla. Pulsa dos veces para cambiar y una vez para activar.";
        
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
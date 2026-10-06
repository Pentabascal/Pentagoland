using System.Collections.Generic;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using UnityEngine;
using VVUP.CustomRoles.Abilities.Passive;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.ClassD
{
    public class Dwarf : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.ClassD;
        public override uint Id { get; set; } = 60;
        public override int MaxHealth { get; set; } = 60;
        public override string Name { get; set; } = "<color=#FF8E00>Clase-D enano</color>";
        public override string Description { get; set; } = "Un Clase-D más pequeño, con menos salud pero más difícil de alcanzar. Tiene resistencia infinita, pero acceso limitado a objetos.";
        public override string CustomInfo { get; set; } = "Clase-D enano";
        public override RoleTypeId Role { get; set; } = RoleTypeId.ClassD;

        public override List<CustomAbility> CustomAbilities { get; set; } = new List<CustomAbility>
        {
            new ScaleAbility
            {
                Name = "Enano [Pasiva]",
                Description = "Te hace tan pequeño como un enano.",
                ScaleForPlayers = new Vector3(0.75f, 0.75f, 0.75f),
            },
            new InfiniteStamina
            {
                Name = "Resistencia infinita [Pasiva]",
                Description = "Te proporciona resistencia infinita.",
            },
            new RestrictedItems
            {
                Name = "Objetos restringidos [Pasiva]",
                Description = "Impide usar ciertos objetos.",
                RestrictedItemList = new List<ItemType>
                {
                    ItemType.ArmorCombat,
                    ItemType.ArmorHeavy,
                    ItemType.GunAK,
                    ItemType.GunE11SR,
                    ItemType.GunFRMG0,
                    ItemType.GunLogicer,
                    ItemType.GunShotgun,
                    ItemType.GunSCP127,
                    ItemType.MicroHID,
                    ItemType.ParticleDisruptor,
                    ItemType.SCP1344,
                    ItemType.SCP268,
                    ItemType.AntiSCP207,
                },
                RestrictUsingItems = true,
                RestrictPickingUpItems = true,
                RestrictDroppingItems = false,
            },
            new CustomRoleEscape
            {
                Name = "Escape con rol personalizado [Pasiva]",
                Description = "Si escapas como Clase-D enano, se te asignará un rol personalizado.",
                EscapeToRegularRole = false,
                CuffedEscapeCustomRole = "<color=#0096FF>Espectro de los MTF</color>",
                UncuffedEscapeCustomRole = "<color=#008f1e>Caótico telepático</color>",
                AllowCuffedCustomRoleChange = true,
                AllowUncuffedCustomRoleChange = true,
                SaveInventory = false,
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
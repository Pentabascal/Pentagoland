using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class Demolitionist : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;
        public StartTeam StartTeam { get; set; } = StartTeam.Ntf;
        public override uint Id { get; set; } = 47;
        public override RoleTypeId Role { get; set; } = RoleTypeId.NtfSpecialist;
        public override int MaxHealth { get; set; } = 100;
        public override string Name { get; set; } = "<color=#0096FF>Especialista en demoliciones de los MTF</color>";
        public override string Description { get; set; } = "Miembro de los MTF especializado en explosivos.";
        public override string CustomInfo { get; set; } = "Especialista en demoliciones de los MTF";
        
        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#FF0000>ADATS</color>",
            "<color=#FF0000>Explosive Resistant Armor</color>",
            "<color=#FF0000>C4</color>",
            "<color=#FF0000>C4</color>",
            "<color=#FF0000>C4</color>",
            "<color=#FF0000>Cluster Grenade</color>",
            ItemType.GrenadeHE.ToString(),
            ItemType.Radio.ToString(),
        };
        
        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Ammo44Cal, 8
            }
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
        
        public override string AbilityUsage { get; set; } = "No tienes habilidades especiales.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using VVUP.CustomRoles.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomRoles.Roles.Foundation
{
    public class LightGuard : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Guard;

        public override uint Id { get; set; } = 31;

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#C0C0C0>Agente de cumplimiento del protocolo</color>";

        public override string Description { get; set; } =
            "Guardia encargado de mantener a raya a los Clase-D. Aparece en la zona de contención ligera, pero sin armamento potente.";

        public override string CustomInfo { get; set; } = "Agente de cumplimiento del protocolo";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Role = RoleTypeId.Scientist,
                    Chance = 100,
                },
            },
        };

        public override List<string> Inventory { get; set; } = new()
        {
            "<color=#0096FF>Silent Serenade</color>",
            ItemType.Medkit.ToString(),
            ItemType.KeycardZoneManager.ToString(),
            ItemType.Painkillers.ToString(),
            ItemType.Radio.ToString(),
            ItemType.ArmorLight.ToString(),
            "<color=#6600CC>Obscurus Veil-5</color>",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 10
            },
        };
        
        public override string AbilityUsage { get; set; } = "No tienes habilidades especiales.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
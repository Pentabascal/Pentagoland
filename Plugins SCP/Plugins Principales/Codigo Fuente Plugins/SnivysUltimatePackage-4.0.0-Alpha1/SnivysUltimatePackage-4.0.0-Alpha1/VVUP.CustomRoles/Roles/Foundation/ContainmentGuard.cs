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
    public class ContainmentGuard : CustomRole, ICustomRole
    {
        public int Chance { get; set; } = 15;

        public StartTeam StartTeam { get; set; } = StartTeam.Guard;

        public override uint Id { get; set; } = 33;

        public override RoleTypeId Role { get; set; } = RoleTypeId.FacilityGuard;

        public override int MaxHealth { get; set; } = 100;

        public override string Name { get; set; } = "<color=#C0C0C0>Guardia de contención</color>";

        public override string Description { get; set; } =
            "Guardia mejor equipado para volver a contener a los SCP.";

        public override string CustomInfo { get; set; } = "Guardia de contención";

        public override bool KeepInventoryOnSpawn { get; set; } = false;

        public override bool KeepRoleOnDeath { get; set; } = false;

        public override bool RemovalKillsPlayer { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
        };

        public override List<string> Inventory { get; set; } = new()
        {
            ItemType.Painkillers.ToString(),
            ItemType.ArmorCombat.ToString(),
            ItemType.Radio.ToString(),
            ItemType.GunCrossvec.ToString(),
            ItemType.KeycardResearchCoordinator.ToString(),
            "<color=#6600CC>Obscurus Veil-5</color>",
        };

        public override Dictionary<AmmoType, ushort> Ammo { get; set; } = new()
        {
            {
                AmmoType.Nato9, 80
            },
        };
        
        public override string AbilityUsage { get; set; } = "No tienes habilidades especiales.";
        [YamlIgnore] 
        public override float SpawnChance { get; set; } = 0;
        [YamlIgnore] 
        public override bool IgnoreSpawnSystem { get; set; } = true;
    }
}
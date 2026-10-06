using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using PlayerRoles;
using UnityEngine;
using VVUP.Base.API;
using VVUP.HuskInfection.EventHandlers;
using YamlDotNet.Serialization;
using PlayerAPI = Exiled.API.Features.Player;

namespace VVUP.HuskInfection.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class HuskGrenade : CustomGrenade, ICustomItemGlow
    {
        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.GrenadeFlash;
        public override uint Id { get; set; } = 48;
        public override string Name { get; set; } = "<color=#FF0000>Granada de infección husk</color>";
        public override string Description { get; set; } = "Granada arrojadiza que contiene el virus de la infección husk. Al explotar, infecta a todos los jugadores cercanos.";
        public override float Weight { get; set; } = 0.75f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 5,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 100,
                    Offset = new Vector3(-3.25f, 1, -0.78f),
                    Room = RoomType.Hcz096,
                },
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 4;
        public float Range { get; set; } = 5f;
        public bool IgnoreTutorials { get; set; } = false;
        [Description("Tiempo, en segundos, que tarda la infección en alcanzar la fase 1.")]
        public float InfectionStageOneDelay { get; set; } = 30f;
        [Description("Tiempo, en segundos, que tarda la infección en alcanzar la fase 2 **DESPUÉS** de llegar a la fase 1.")]
        public float InfectionStageTwoDelay { get; set; } = 90f;
        [Description("Texto que se mostrará al alcanzar la fase 2.")]
        public string InfectionText { get; set; } = "<color=red><size=30>Sientes algo en la garganta. Intentas gritar, pero no sale ningún sonido.</size></color>";
        public bool UseHints { get; set; } = false;
        public float TextDisplayTime { get; set; } = 10f;
        public uint HuskZombieCustomRoleId { get; set; } = 56;
        public string HuskTakeOverDeathReason { get; set; } = "You have been taken over by a Husk Infection.";
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            ev.IsAllowed = false;
            foreach (PlayerAPI player in PlayerAPI.List)
            {
                if (Vector3.Distance(ev.Position, player.Position) <= Range)
                {
                    if (player.IsScp)
                        continue;
                    if (IgnoreTutorials && player.Role.Type == RoleTypeId.Tutorial)
                        continue;
                    if (HuskInfectionEventHandlers.PlayersWithHuskInfection.ContainsKey(player))
                        continue;
                    Log.Debug($"VVUP Custom Items: HuskGrenade, Husk Grenade infecting {player.Nickname}.");
                    HuskInfectionEventHandlers huskInfection = new HuskInfectionEventHandlers(player, InfectionStageOneDelay, InfectionStageTwoDelay, 
                        InfectionText, UseHints, TextDisplayTime, HuskZombieCustomRoleId, HuskTakeOverDeathReason);
                }
            }
        }
    }
}
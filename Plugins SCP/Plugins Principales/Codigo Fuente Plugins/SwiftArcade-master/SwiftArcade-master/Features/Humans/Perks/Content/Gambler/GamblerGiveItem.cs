using LabApi.Features.Wrappers;
using SwiftArcadeMode.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Gambler
{
    public class GamblerGiveItem : GamblerEffectBase
    {
        public static readonly ItemType[] Pool = [
            ItemType.SCP500,
            ItemType.SCP330,
            ItemType.SCP268,
            ItemType.SCP244b,
            ItemType.SCP244a,
            ItemType.SCP018,
            ItemType.SCP1344,
            ItemType.SCP1853,
            ItemType.SCP1576,
            ItemType.AntiSCP207,
            ItemType.ArmorCombat,
            ItemType.ArmorLight,
            ItemType.ArmorHeavy,
            ItemType.Adrenaline,
            ItemType.Medkit,
            ItemType.Painkillers,
            ItemType.ParticleDisruptor,
            ItemType.MicroHID,
            ItemType.Radio,
            ItemType.SurfaceAccessPass,
            ItemType.Flashlight,
            ItemType.Jailbird,
            ItemType.Lantern,
            ItemType.Coin,
            ItemType.GunAK,
            ItemType.GunFSP9,
            ItemType.GunCOM18,
            ItemType.GunRevolver,
            ItemType.GunLogicer,
            ItemType.GunShotgun,
            ItemType.GunSCP127,
            ItemType.GunA7,
            ItemType.GunCOM15,
            ItemType.GunFRMG0,
            ItemType.GunE11SR,
            ItemType.GunCom45,
            ItemType.GunCrossvec,
            ItemType.KeycardChaosInsurgency,
            ItemType.KeycardContainmentEngineer,
            ItemType.KeycardFacilityManager,
            ItemType.KeycardGuard,
            ItemType.KeycardJanitor,
            ItemType.KeycardMTFCaptain,
            ItemType.KeycardMTFOperative,
            ItemType.KeycardMTFPrivate,
            ItemType.KeycardZoneManager,
            ItemType.KeycardScientist,
            ItemType.KeycardResearchCoordinator,
            ];

        public override bool Positive => true;

        public override int Weight => 2;

        public override string Explanation => "Has recibido un objeto aleatorio.";

        public override void Effect(Player player)
        {
            ItemType type = Pool.GetRandom();
            if (player.IsInventoryFull)
                Pickup.Create(type, player.Position).Spawn();
            else
                player.AddItem(type);
        }
    }
}

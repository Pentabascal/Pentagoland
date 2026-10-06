using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Utils.Extensions;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("GunGame", Rarity.Rare)]
    public class GunGame(PerkInventory inv) : PerkKillBase(inv)
    {
        public static ItemType[] Pool = [
            ItemType.GunA7,
            ItemType.GunAK,
            ItemType.GunRevolver,
            ItemType.GunFRMG0,
            ItemType.GunFSP9,
            ItemType.GunLogicer,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCom45,
            ItemType.GunE11SR,
            ItemType.GunCrossvec,
            ItemType.GunShotgun,
            ItemType.ParticleDisruptor,
            ];

        public override string Name => "Juego de armas";

        public override string Description => "Tu arma cambia al azar cada vez que matas a alguien.";

        int lastRandom;

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player || Player.CurrentItem == null || !Pool.Contains(Player.CurrentItem.Type))
                return;

            Player.RemoveItem(Player.CurrentItem);
            Player.CurrentItem = Player.AddItem(Pool.GetRandom(ref lastRandom));
        }
    }
}

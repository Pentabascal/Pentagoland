using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Telefragger", Rarity.Rare)]
    public class Telefragger(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Telefragger";

        public override string Description => "Te teletransportas hasta la persona que matas.\nAparece una granada cegadora de mecha y duración cortas.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            Player.Position = ev.Player.Position;
            if (TimedGrenadeProjectile.SpawnActive(ev.Player.Position, ItemType.GrenadeFlash, Player, 0.3f) is FlashbangProjectile proj)
                proj.BaseBlindTime = 0.6f;
        }
    }
}

using CustomPlayerEffects;
using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Detonator", Rarity.Rare)]
    public class Detonator(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Detonador";

        public override string Description => "Al matar a un jugador afectado por Quemadura,\nsoltarás una granada de mecha corta en su posición.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            base.OnPlayerDying(ev);
            if (ev.Attacker != Player || !ev.Player.TryGetEffect(out Burned burned) || !burned.IsEnabled)
                return;

            TimedGrenadeProjectile proj = TimedGrenadeProjectile.SpawnActive(ev.Player.Position, ItemType.GrenadeHE, Player, 0.8f);
            if (proj.Base is ExplosionGrenade gr)
                gr.ScpDamageMultiplier = 1.5f;
        }
    }
}

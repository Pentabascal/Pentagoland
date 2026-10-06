using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Reaper", Rarity.Uncommon)]
    public class Reaper(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Segador";

        public override string Description => $"Te curas un {Percentage * 100}% del daño que infliges a los enemigos.";

        public virtual float Percentage => 0.05f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurt += OnPlayerHurt;
        }

        private void OnPlayerHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Attacker != Player || ev.DamageHandler is not StandardDamageHandler dmg)
                return;

            Player.Heal(dmg.Damage * Percentage);
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurt -= OnPlayerHurt;
        }
    }
}

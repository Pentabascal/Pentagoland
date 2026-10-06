using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("BombDuck", Rarity.Legendary, PerkRestriction.DontSpawn)]
    public class BombDuck(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Pato bomba";

        public override string Description => "Tus granadas no te hacen daño.";

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || ev.DamageHandler is not ExplosionDamageHandler || ev.Attacker != Player)
                return;

            ev.IsAllowed = false;
        }
    }
}

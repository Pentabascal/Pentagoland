using CustomPlayerEffects;
using LabApi.Events.Handlers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Furnace", Rarity.Uncommon)]
    public class Furnace(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Horno";

        public override string Description => "Quemas a quienes dañes.\nTambién te quemas cuando recibes daño.";

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurt += OnHurt;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurt -= OnHurt;
        }

        private void OnHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Attacker == Player || ev.Player == Player)
                ev.Player.EnableEffect<Burned>(1, 1, true);
        }
    }
}

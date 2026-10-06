using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Headtapper", Rarity.Uncommon)]
    public class Headtapper(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Headtapper";

        public override string Description => "Infliges daño adicional al acertar en la cabeza de un humano.\nLos impactos en el torso y las extremidades infligen menos daño.\nEl daño a los SCP no cambia.";

        public virtual float HeadMultiplier => 2f;
        public virtual float BodyMultiplier => 0.5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnPlayerHurting;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnPlayerHurting;
        }

        private void OnPlayerHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || ev.Player.IsSCP || ev.DamageHandler is not FirearmDamageHandler handler)
                return;

            handler.Damage *= handler.Hitbox == HitboxType.Headshot ? HeadMultiplier : BodyMultiplier;
        }
    }
}

using CustomPlayerEffects;
using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    public class LimbBreak(UpgradePathPerkBase parent) : UpgradeBase<EndlessDecay>(parent)
    {
        public override string Name => "Fractura";

        public override string Description => $"Cada tres golpes, ralentizas a tu objetivo durante {Duration}s.";

        public virtual float Duration => 1f;

        int counter;

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
            if (ev.Attacker != Player || ev.DamageHandler is not ScpDamageHandler)
                return;

            counter++;

            if (counter >= 3)
            {
                counter = 0;
                ev.Player.EnableEffect<Sinkhole>(1, Duration);
            }
        }
    }
}

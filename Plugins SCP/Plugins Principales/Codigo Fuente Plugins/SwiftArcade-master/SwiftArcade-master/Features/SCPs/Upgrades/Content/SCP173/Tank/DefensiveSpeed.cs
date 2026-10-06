using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Tank
{
    public class DefensiveSpeed(UpgradePathPerkBase parent) : UpgradeBase<Tank>(parent)
    {
        public override string Name => "Velocidad defensiva";

        public override string Description => "Recibes menos daño al correr a toda velocidad.";

        public virtual float Percentage => 0.25f;

        bool enabled;

        public override void Init()
        {
            base.Init();
            Scp173Events.BreakneckSpeedChanged += OnBreakneckSpeedsChanged;
            PlayerEvents.Hurting += OnPlayerHurting;
        }

        private void OnPlayerHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || !enabled)
                return;

            if (ev.DamageHandler is StandardDamageHandler dmg)
                dmg.Damage *= 1f - Percentage;
        }

        private void OnBreakneckSpeedsChanged(LabApi.Events.Arguments.Scp173Events.Scp173BreakneckSpeedChangedEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            enabled = ev.Active;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.BreakneckSpeedChanged -= OnBreakneckSpeedsChanged;
            PlayerEvents.Hurting -= OnPlayerHurting;
        }
    }
}

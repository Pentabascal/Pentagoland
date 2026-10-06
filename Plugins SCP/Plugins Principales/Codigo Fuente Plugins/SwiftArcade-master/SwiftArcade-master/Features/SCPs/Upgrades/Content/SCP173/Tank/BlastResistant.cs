using LabApi.Events.Handlers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Tank
{
    public class BlastResistant(UpgradePathPerkBase parent) : UpgradeBase<Tank>(parent)
    {
        public override string Name => "Resistente a explosiones";

        public override string Description => $"Recibes un {Percentage * 100f}% menos de daño por explosiones.";

        public float Percentage => 0.5f;

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
            if (ev.Player != Player || ev.DamageHandler is not ExplosionDamageHandler exp)
                return;

            exp.Damage *= 1f - Percentage;
        }
    }
}

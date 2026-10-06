using CommandSystem.Commands.RemoteAdmin;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using PlayerRoles.FirstPersonControl;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    public class TacticalRetreat(UpgradePathPerkBase parent) : UpgradeCooldownBase<Scouter>(parent)
    {
        public override string Name => "Retirada táctica";

        public override string UpgradeDescription => "Te teletransporta a la ubicación de tu último ataque frenético al recibir mucho daño tras romperse el escudo de Hume.";

        public override float Cooldown => 100f;

        public virtual float RealHealthDamageThreshold => 400f;

        TantrumHazard latestTantrum;
        float recordedHealth;

        public override void Init()
        {
            base.Init();
            Scp173Events.CreatedTantrum += OnCreatedTantrum;
            WarheadEvents.Detonated += OnWarheadDetonated;
            PlayerEvents.Hurt += OnHurt;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.CreatedTantrum -= OnCreatedTantrum;
            WarheadEvents.Detonated -= OnWarheadDetonated;
            PlayerEvents.Hurt -= OnHurt;
        }

        private void OnWarheadDetonated(WarheadDetonatedEventArgs ev)
        {
            latestTantrum = null;
            SendMessage("¡Ojiva detonada! Se ha perdido el teletransporte del ataque frenético.");
        }

        private void OnHurt(PlayerHurtEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (Player.HumeShield <= 0f && recordedHealth < 0f)
                recordedHealth = Player.Health;
            else if (recordedHealth - Player.Health >= RealHealthDamageThreshold)
                Trigger();
            else if (Player.HumeShield > 0f)
                recordedHealth = -1f;
        }

        private void OnCreatedTantrum(Scp173CreatedTantrumEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            latestTantrum = ev.Tantrum;
        }

        public override void Effect()
        {
            if (latestTantrum == null || !latestTantrum.IsActive || latestTantrum.IsDestroyed)
                return;

            Timing.CallDelayed(0.2f, () => { Player.Position = latestTantrum.SyncedPosition; });
        }
    }
}

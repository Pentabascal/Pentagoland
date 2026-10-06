using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    public abstract class PerkKillBase(PerkInventory inv) : PerkBase(inv)
    {
        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
            PlayerEvents.Death += OnPlayerDeath;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
            PlayerEvents.Death -= OnPlayerDeath;
        }

        protected virtual void OnPlayerDying(PlayerDyingEventArgs ev) { }
        protected virtual void OnPlayerDeath(PlayerDeathEventArgs ev) { }
    }
}

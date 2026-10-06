using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content
{
    public abstract class UpgradeKillBase<T>(UpgradePathPerkBase parent) : UpgradeBase<T>(parent) where T : UpgradePathPerkBase
    {
        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }

        protected abstract void OnPlayerDying(PlayerDyingEventArgs ev);
    }
}

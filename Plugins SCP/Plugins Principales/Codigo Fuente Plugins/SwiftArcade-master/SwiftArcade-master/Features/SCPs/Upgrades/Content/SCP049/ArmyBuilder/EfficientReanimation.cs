using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    public class EfficientReanimation(UpgradePathPerkBase parent) : UpgradeBase<ArmyBuilder>(parent)
    {
        public override string Name => "Resurrección eficiente";

        public override string Description => $"Los zombis resucitados ya no reciben penalizaciones de salud máxima.\nTodos los zombis tienen {MaxHealth} HP máximos.\nAdemás, se curan al matar.";

        public virtual float MaxHealth => 600f;
        public virtual float Healing => 50f;

        public override void Init()
        {
            base.Init();
            Parent.OnAddedZombie += OnAddedZombie;
            PlayerEvents.Dying += OnPlayerDying;
        }

        public override void Remove()
        {
            base.Remove();
            Parent.OnAddedZombie -= OnAddedZombie;
            PlayerEvents.Dying -= OnPlayerDying;
        }

        private void OnAddedZombie(Player obj)
        {
            obj.MaxHealth = MaxHealth;
            obj.Health = MaxHealth;
        }

        private void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.Role != RoleTypeId.Scp0492 || !Parent.OwnedZombies.Contains(ev.Attacker))
                return;

            ev.Attacker.Heal(Healing);
        }
    }
}

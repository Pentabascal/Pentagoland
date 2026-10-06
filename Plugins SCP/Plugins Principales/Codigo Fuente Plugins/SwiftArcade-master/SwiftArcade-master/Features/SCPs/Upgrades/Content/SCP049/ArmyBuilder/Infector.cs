using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.ArmyBuilder
{
    public class Infector(UpgradePathPerkBase parent) : UpgradeBase<ArmyBuilder>(parent)
    {
        public override string Name => "Infectador";

        public override string Description => $"Al matar, los zombis tienen un {Chance * 100f}% de probabilidad de infectar a un humano.";

        public virtual float Chance => 0.35f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        private void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.Role != RoleTypeId.Scp0492 || !Parent.OwnedZombies.Contains(ev.Attacker) || Random.Range(0f, 1f) <= Chance)
                return;

            Timing.CallDelayed(0.2f, () => 
            { 
                ev.Player.SetRole(RoleTypeId.Scp0492); 
                Parent.AddZombie(ev.Player); 
                ev.Player.SendHint("¡Te han infectado!");
            });
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }
    }
}

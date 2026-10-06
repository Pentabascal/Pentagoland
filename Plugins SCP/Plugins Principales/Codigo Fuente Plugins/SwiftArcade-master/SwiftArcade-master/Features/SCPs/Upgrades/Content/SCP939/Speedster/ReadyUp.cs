using CustomPlayerEffects;
using PlayerRoles.PlayableScps.Scp939;
using SwiftArcadeMode.Utils.Structures;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    public class ReadyUp(UpgradePathPerkBase parent) : UpgradeBase<Speedster>(parent)
    {
        public override string Name => "Preparación";

        public override string Description => $"Si te agachas durante {Requirement}s, obtienes un aumento de velocidad durante {Duration}s al levantarte.";

        public virtual float Duration => 8f;

        public virtual float Requirement => 3f;

        public bool CrouchState => role != null && role.SubroutineModule.TryGetSubroutine(out Scp939FocusAbility focus) && focus.State > 0.5f;

        private readonly Timer timer = new();
        Scp939Role role;
        bool trigger;

        public override void Init()
        {
            base.Init();
            if (Player.RoleBase is Scp939Role r)
                role = r;

            timer.OnTimerEnd += OnTimerEnd;
        }

        private void OnTimerEnd()
        {
            SendMessage("¡Aumento de velocidad disponible!");
            trigger = true;
        }

        public override void Tick()
        {
            base.Tick();

            if (CrouchState)
            {
                float t = Mathf.Round(timer.CurrentValue);
                if (t > 1f)
                    SendMessage("Tiempo hasta el aumento: " + t + "s");
                timer.Tick(Time.fixedDeltaTime);
            }
            else
            {
                if (timer.Ended && trigger)
                {
                    trigger = false;
                    Player.EnableEffect<MovementBoost>(70, Duration);
                }

                timer.Reset(Requirement);
            }
        }

        public override void Remove()
        {
            base.Remove();
            timer.OnTimerEnd -= OnTimerEnd;
        }
    }
}

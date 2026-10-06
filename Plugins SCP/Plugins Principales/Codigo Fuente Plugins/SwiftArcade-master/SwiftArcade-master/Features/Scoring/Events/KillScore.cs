using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Scoring.Events
{
    public class KillScore : ScoreEventBase
    {
        public override void Enable() => PlayerEvents.Dying += OnDying;

        protected virtual void OnDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            ev.Attacker.AddScore(5);
        }

        public override void Tick() { }

        public override void Disable() => PlayerEvents.Dying -= OnDying;
    }
}

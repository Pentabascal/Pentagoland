using System.Linq;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Roles;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Scp173;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class SoundBreaker : ActiveAbility
    {
        public override string Name { get; set; } = "Rompesonidos";
        public override string Description { get; set; } = "Reinicia la recarga del parpadeo y reduce el intervalo y el alcance del siguiente.";
        public override float Duration { get; set; } = 5f;
        public override float Cooldown { get; set; } = 150f;
        public float BlinkCooldown { get; set; } = 0.5f;
        public float MaxBlinkDistance { get; set; } = 4f;
        public int MinimumObservedPlayers { get; set; } = 1;

        public override bool CanUseAbility(Player player, out string response, bool selectedOnly = false)
        {
            if (player.Role is Scp173Role scp173Role)
            {
                int observers = scp173Role.ObservingPlayers.Count();

                if (observers >= MinimumObservedPlayers)
                    return base.CanUseAbility(player, out response, selectedOnly);
            }

            response = "Nadie te está mirando.";
            return false;
        }

        protected override void AbilityUsed(Player player)
        {
            Exiled.Events.Handlers.Scp173.Blinking += OnBlinking;

            if (player.Role is Scp173Role role)
                role.BlinkCooldown = 0f;
        }

        protected override void AbilityEnded(Player player)
        {
            Exiled.Events.Handlers.Scp173.Blinking -= OnBlinking;

            player.ShowHint("Rompesonidos terminado.", 3f);
        }

        private void OnBlinking(BlinkingEventArgs ev)
        {
            if (ActivePlayers.Contains(ev.Player))
            {
                ev.BlinkCooldown = BlinkCooldown;

                Vector3 current = ev.Player.Position;
                Vector3 target = ev.BlinkPosition;
                Vector3 direction = (target - current);

                if (direction.magnitude > MaxBlinkDistance)
                {
                    direction = direction.normalized * MaxBlinkDistance;
                    ev.BlinkPosition = current + direction;
                }
            }
        }
    }
}
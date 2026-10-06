using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class TeamConvertOnKill : PassiveAbility
    {
        public override string Name { get; set; } = "Conversión de equipo al matar";
        public override string Description { get; set; } = "Aplica efectos a los jugadores a los que golpeas.";
        [Description("¿En qué rol se debe convertir al jugador?")]
        public RoleTypeId ConvertToRole { get; set; } = RoleTypeId.Tutorial;
        
        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: TeamConvertOnKill, Adding TeamConvertOnKill Ability to {player.Nickname}");
            Exiled.Events.Handlers.Player.Dying += OnDying;
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: TeamConvertOnKill, Removing TeamConvertOnKill Ability from {player.Nickname}");
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            base.AbilityRemoved(player);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Player == null)
                return;
            if (Check(ev.Attacker))
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Log.Debug($"VVUP Custom Abilities: TeamConvertOnKill, {ev.Attacker.Nickname} converted {ev.Player.Nickname} to {ConvertToRole}");
                    ev.Player.Role.Set(ConvertToRole);
                });
            }
        }
    }
}
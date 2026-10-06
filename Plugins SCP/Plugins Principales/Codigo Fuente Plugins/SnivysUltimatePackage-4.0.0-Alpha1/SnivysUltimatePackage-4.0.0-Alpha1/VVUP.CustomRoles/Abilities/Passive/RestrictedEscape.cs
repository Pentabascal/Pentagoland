using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class RestrictedEscape : PassiveAbility
    {
        public override string Name { get; set; } = "Escape restringido";

        public override string Description { get; set; } =
            "Impide escapar por los medios habituales (se puede escapar estando detenido).";

        [Description("Texto que se muestra si el jugador no está esposado y debe estarlo para escapar.")]
        public string EscapeTextUncuffed { get; set; } = "No puedes escapar si no estás detenido";
        [Description("Texto que se muestra si el jugador está esposado y debe liberarse para escapar.")]
        public string EscapeTextCuffed { get; set; } = "No puedes escapar si estás detenido";
        [Description("Texto que se muestra si el jugador no puede escapar bajo ninguna circunstancia.")]
        public string EscapeTextBoth { get; set; } = "No puedes escapar";
        public float EscapeTextTime { get; set; } = 5;
        public bool UseHints { get; set; } = true;
        public bool AllowedUncuffedEscape { get; set; } = false;
        public bool AllowedCuffedEscape { get; set; } = false;
        
        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            base.AbilityAdded(player);
        }
        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            base.AbilityRemoved(player);
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;
            if (!AllowedCuffedEscape && !AllowedUncuffedEscape)
            {
                ev.IsAllowed = false;
                Log.Debug($"VVUP Custom Abilities: Restricting Escape of {ev.Player.Nickname}");
                if (!string.IsNullOrWhiteSpace(EscapeTextBoth))
                    if (UseHints)
                        ev.Player.ShowHint(EscapeTextBoth, EscapeTextTime);
                    else
                        ev.Player.Broadcast((ushort)EscapeTextTime, EscapeTextBoth);
            }
            else if (!AllowedCuffedEscape && ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
                Log.Debug($"VVUP Custom Abilities: Restricting Escape of {ev.Player.Nickname} while cuffed");
                if (!string.IsNullOrWhiteSpace(EscapeTextCuffed))
                    if (UseHints)
                        ev.Player.ShowHint(EscapeTextCuffed, EscapeTextTime);
                    else
                        ev.Player.Broadcast((ushort)EscapeTextTime, EscapeTextCuffed);
            }
            else if (!AllowedUncuffedEscape && !ev.Player.IsCuffed)
            {
                ev.IsAllowed = false;
                Log.Debug($"VVUP Custom Abilities: Restricting Escape of {ev.Player.Nickname} while uncuffed");
                if (!string.IsNullOrWhiteSpace(EscapeTextUncuffed))
                    if (UseHints)
                        ev.Player.ShowHint(EscapeTextUncuffed, EscapeTextTime);
                    else
                        ev.Player.Broadcast((ushort)EscapeTextTime, EscapeTextUncuffed);
            }
        }
    }
}
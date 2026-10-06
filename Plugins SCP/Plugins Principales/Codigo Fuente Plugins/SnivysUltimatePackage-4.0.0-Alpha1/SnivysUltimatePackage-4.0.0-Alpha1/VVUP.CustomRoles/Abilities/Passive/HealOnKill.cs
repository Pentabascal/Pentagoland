using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class HealOnKill : PassiveAbility
    {
        private readonly Dictionary<Player, CoroutineHandle> activeHoTs = new ();

        public override string Name { get; set; } = "Curación al matar";

        public override string Description { get; set; } = "Cura al jugador cuando elimina a otro.";

        [Description("Cantidad de salud que recibe el jugador.")]
        public float HealAmount { get; set; } = 25f;

        [Description("Determina si la curación puede superar la salud máxima.")]
        public bool HealOverMax { get; set; } = false;

        [Description("Determina si la curación se aplica gradualmente (true) o al instante (false).")]
        public bool HealOverTime { get; set; } = true;

        [Description("Duración del efecto de curación gradual, si se utiliza.")]
        public float HealOverTimeDuration { get; set; } = 10f;

        [Description("Frecuencia, en segundos, de las pulsaciones de curación gradual, si se utiliza.")]
        public float HealOverTimeTickFrequency { get; set; } = 1.0f;

        [Description("Determina si el efecto de curación gradual termina antes de tiempo cuando el jugador recibe daño.")]
        public bool DamageInterruptsHot { get; set; } = true;

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.Dying += OnDying;
            if (HealOverTime && DamageInterruptsHot)
                Exiled.Events.Handlers.Player.Hurting += OnHurting;
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            if (HealOverTime && DamageInterruptsHot) 
                Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            base.AbilityRemoved(player);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (Check(ev.Attacker))
            {
                if (HealOverTime)
                    activeHoTs[ev.Attacker] = Timing.RunCoroutine(DoHealOverTime(ev.Attacker));
                ev.Attacker.Heal(HealAmount, HealOverMax);
            }
        }

        private void OnHurting(HurtingEventArgs ev)
        {
            if (Check(ev.Player))
            {
                if (DamageInterruptsHot && activeHoTs.ContainsKey(ev.Player))
                    Timing.KillCoroutines(activeHoTs[ev.Player]);
            }
        }

        private IEnumerator<float> DoHealOverTime(Player player)
        {
            float tickAmount = HealAmount / HealOverTimeDuration;
            int tickCount = Mathf.FloorToInt(HealOverTimeDuration / HealOverTimeTickFrequency);

            for (int i = 0; i < tickCount; i++)
            {
                player.Heal(tickAmount, HealOverMax);
                yield return Timing.WaitForSeconds(HealOverTimeTickFrequency);
            }
        }
    }
}
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.CustomRoles.API.Features;
using MEC;
using VVUP.Base;

namespace VVUP.CustomRoles.Abilities.Passive
{
    public class EffectEnabler : PassiveAbility
    {
        public override string Name { get; set; } = "Activador de efectos";
        public override string Description { get; set; } = "Aplica efectos al jugador.";
        
        public List<ApplyEffects> EffectsToApply { get; set; } = new List<ApplyEffects>
        {
            new()
            {
                EffectType = EffectType.Invigorated
            }
        };
        public float DelayBeforeApplyingEffects { get; set; } = 0f;
        
        protected override void AbilityAdded(Player player)
        {
            void ApplyEffects()
            {
                if (player == null || !player.IsConnected)
                    return;

                foreach (var effect in EffectsToApply)
                {
                    Log.Debug($"VVUP Custom Abilities: Activating {effect.EffectType} to {player.Nickname}");
                    player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration, effect.AddDurationIfActive);
                }
            }

            float delay = DelayBeforeApplyingEffects > 0f ? DelayBeforeApplyingEffects : 0f;
            Timing.CallDelayed(delay, ApplyEffects);
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            foreach (var effect in EffectsToApply)
            {
                Log.Debug($"VVUP Custom Abilities: Removing {effect.EffectType} from {player.Nickname}");
                player.DisableEffect(effect.EffectType);
            }
            base.AbilityRemoved(player);
        }
    }
}
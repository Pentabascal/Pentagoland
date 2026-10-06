using System.ComponentModel;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Player = Exiled.Events.Handlers.Player;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class SpeedOnKill : PassiveAbility
    {
        public override string Name { get; set; } = "Velocidad al matar";

        public override string Description { get; set; } = "Aumenta la velocidad del jugador cuando elimina a otro.";

        public float Duration { get; set; } = 5f;

        [Description("Nivel máximo de intensidad de velocidad de SCP-207 que puede otorgar esta habilidad.")]
        public byte IntensityLimit { get; set; } = 2;

        protected override void SubscribeEvents()
        {
            Player.Dying += OnDying;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.Dying -= OnDying;
            base.UnsubscribeEvents();
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (Check(ev.Attacker))
            {
                byte curIntensity = ev.Attacker.GetEffectIntensity<Scp207>();
                if (curIntensity < IntensityLimit)
                {
                    Log.Debug($"VVUP Custom Abilities: Applying Speed On Kill Effect to {ev.Attacker.Nickname}");
                    //ev.Attacker.ChangeEffectIntensity<Scp207>((byte)(curIntensity + 1));
                    //ev.Attacker.GetEffect(EffectType.Scp207).Duration = Duration;
                    ev.Attacker.EnableEffect(EffectType.Scp207, (byte)(curIntensity + 1), Duration);
                }
            }
        }
    }
}
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class CustomHume : PassiveAbility
    {
        public override string Name { get; set; } = "Escudo Hume personalizado";
        public override string Description { get; set; } = "Otorga al jugador un escudo Hume personalizado.";
        public float MaxShield { get; set; } = 100f;
        public float StartShield { get; set; } = 100f;
        public float RegenRate { get; set; } = 1f;

        protected override void AbilityAdded(Player player)
        {
            Log.Debug($"VVUP Custom Abilities, CustomHume: Adding custom Hume Shield to {player.Nickname}, Max: {MaxShield}, Start: {StartShield}, RegenRate: {RegenRate}");
            base.AbilityAdded(player);
            player.MaxHumeShield = MaxShield;
            player.HumeShield = StartShield;
            player.CustomHumeShieldStat.ShieldRegenerationMultiplier = RegenRate;
        }
    }
}
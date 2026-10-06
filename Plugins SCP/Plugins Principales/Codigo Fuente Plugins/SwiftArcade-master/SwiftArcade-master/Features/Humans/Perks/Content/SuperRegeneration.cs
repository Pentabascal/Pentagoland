using LabApi.Events.Handlers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("SuperRegeneration", Rarity.Epic)]
    public class SuperRegeneration(PerkInventory inv) : Regeneration(inv)
    {
        public override string Name => "Superregeneración";

        public override string Description => $"{base.Description}\nSin embargo, tu salud máxima se reduce un {DecreasePercentage * 100f}%.";

        public override float HealthThresholdPercentage => 0.35f;
        public override float Rate => 9f;
        public virtual float DecreasePercentage => 0.1f;

        float originalHealth;

        public override void Init()
        {
            base.Init();
            originalHealth = Player.MaxHealth;
            Player.MaxHealth = originalHealth - DecreasePercentage * originalHealth;

            PlayerEvents.ChangedRole += OnPlayerChangedRole;
        }

        private void OnPlayerChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            originalHealth = Player.MaxHealth;
            Player.MaxHealth = originalHealth - DecreasePercentage * originalHealth;
        }

        public override void Remove()
        {
            base.Remove();

            Player.MaxHealth = originalHealth;

            PlayerEvents.ChangedRole -= OnPlayerChangedRole;
        }
    }
}

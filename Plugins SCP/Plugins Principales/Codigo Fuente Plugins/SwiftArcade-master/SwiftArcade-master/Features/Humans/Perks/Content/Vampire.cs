using LabApi.Events.Handlers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Vampire", Rarity.Epic, PerkRestriction.Human, conflictPerks: [typeof(Reaper), typeof(Regeneration), typeof(SuperRegeneration)])]
    public class Vampire(PerkInventory inv) : Reaper(inv)
    {
        public override string Name => "Vampiro";

        public override string Description => base.Description + "\nSin embargo, ya no puedes usar objetos médicos.";

        public override float Percentage => 0.5f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.UsingItem += OnUsingItem;
        }

        private void OnUsingItem(LabApi.Events.Arguments.PlayerEvents.PlayerUsingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (ev.UsableItem.Category == ItemCategory.Medical)
            {
                SendMessage("¡No puedes usar objetos médicos!");
                ev.IsAllowed = false;
            }
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.UsingItem -= OnUsingItem;
        }
    }
}

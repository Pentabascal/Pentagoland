namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("SlotUpgrade", Rarity.Legendary)]
    public class PerkSlotUpgrade(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Mejora de espacios para ventajas";

        public override string Description => $"Te otorga {Amount} espacios para ventajas y ocupa {SlotUsage} espacios.";

        public virtual int Amount => 3;

        public override int SlotUsage => 0;

        PerkInventory.LimitAdditive additive;

        public override void Init()
        {
            base.Init();
            additive = Inventory.CreateLimitAdditive(Amount);
        }

        public override void Remove()
        {
            base.Remove();
            Inventory.RemoveLimitAdditive(additive);
        }
    }
}

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Raise", Rarity.Rare)]
    public class Raise(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Aumento de sueldo";

        public override string Description => "Recibirás un aumento de sueldo.";
    }
}

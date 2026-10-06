namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("TrickOrTreater", Rarity.Legendary)]
    public class TrickOrTreater(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Truco o trato";

        public override string PerkDescription => "Recibes un caramelo.";

        public override float Cooldown => 25f;

        public override ItemType ItemType => ItemType.SCP330;
    }
}

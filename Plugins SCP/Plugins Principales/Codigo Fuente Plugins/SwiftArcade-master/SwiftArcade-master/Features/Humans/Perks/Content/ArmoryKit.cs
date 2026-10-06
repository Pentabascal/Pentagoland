namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("ArmoryKit", Rarity.Uncommon)]
    public class ArmoryKit(PerkInventory inv) : PerkItemReceiveBase(inv)
    {
        public override string Name => "Kit de armería";

        public override string PerkDescription => "Recibes una granada.";

        public override float Cooldown => 60f;

        public override ItemType ItemType => ItemType.GrenadeHE;
    }
}

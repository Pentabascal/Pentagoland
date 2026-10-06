using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("BombHen", Rarity.Legendary)]
    public class BombHen(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Gallina bomba";

        public override string PerkDescription => "Pones un huevo explosivo (también puede matarte).";

        public override float Cooldown => 35f;

        public override void Effect() => TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeHE, Player, 5f);
    }
}

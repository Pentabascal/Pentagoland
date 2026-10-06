using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Exhib", Rarity.Secret)]
    public class Exhibitionist(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public override string Name => "Exhibicionista";

        public override string PerkDescription => "Ciega a los enemigos cercanos.";

        public override float Cooldown => 20f;

        public override void Effect() => TimedGrenadeProjectile.SpawnActive(Player.Position, ItemType.GrenadeFlash, Player, 0.1f);
    }
}

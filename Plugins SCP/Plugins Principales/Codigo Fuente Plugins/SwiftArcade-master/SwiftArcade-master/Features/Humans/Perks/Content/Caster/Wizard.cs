using System;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    [Perk("Wizard", Rarity.Epic)]
    public class Wizard(PerkInventory inv) : CasterBase(inv)
    {
        public override string Name => "Mago";

        public override float RegularCooldown => 10f;

        public override Type[] ListSpells() => [
            typeof(Fireball),
            typeof(IceBolt),
            typeof(MagicMissile),
            typeof(SummonArcaneGolem)
            ];
    }
}

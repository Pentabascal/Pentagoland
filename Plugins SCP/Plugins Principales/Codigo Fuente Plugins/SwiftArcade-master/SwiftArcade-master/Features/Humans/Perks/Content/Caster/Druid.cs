using System;

namespace SwiftArcadeMode.Features.Humans.Perks.Content.Caster
{
    [Perk("Druid", Rarity.Epic)]
    public class Druid(PerkInventory inv) : CasterBase(inv)
    {
        public override float RegularCooldown => 8f;

        public override string Name => "Druida";

        public override Type[] ListSpells() => [
            typeof(ThornShot),
            typeof(OrbOfNature),
            typeof(ThornVolley),
            typeof(SummonPylon),
            typeof(SummonThornShooter)
            ];
    }
}

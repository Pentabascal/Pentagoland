using CustomPlayerEffects;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Regeneration", Rarity.Common)]
    public class Regeneration(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Regeneración";
        public override string Description => $"Si tienes más del {HealthThresholdPercentage * 100f}% de HP, recuperas {Rate} HP/s.";

        public virtual float HealthThresholdPercentage => 0.6f;
        public virtual float Rate => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.Health / Player.MaxHealth >= HealthThresholdPercentage && Player.Health < Player.MaxHealth)
                Player.Heal((Player.IsSCP ? 0.5f : 1f) * (Rate / (Player.Room.Name == MapGeneration.RoomName.Pocket ? 3f : 1f)) * Time.fixedDeltaTime);
        }
    }
}

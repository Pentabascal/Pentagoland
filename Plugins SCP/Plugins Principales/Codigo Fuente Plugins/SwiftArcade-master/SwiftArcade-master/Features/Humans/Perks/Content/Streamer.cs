using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Streamer", Rarity.Secret)]
    public class Streamer(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Streamer";

        public override string Description => $"Cada espectador que te esté viendo te cura {RatePerPlayer} HP/s.\nNo funciona en la dimensión de bolsillo.";

        public virtual float RatePerPlayer => 3f;

        public override void Tick()
        {
            base.Tick();

            if (Player.CurrentSpectators.Count > 0 && Player.Room.Name != MapGeneration.RoomName.Pocket)
                Player.Heal(RatePerPlayer * Time.fixedDeltaTime * Player.CurrentSpectators.Count);
        }
    }
}

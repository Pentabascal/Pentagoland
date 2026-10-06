using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("DoorTechnician", Rarity.Epic)]
    public class DoorTechnician(PerkInventory inv) : PerkDoorBase(inv)
    {
        public override string Name => "Técnico de puertas";

        public override string Description => "Repara todas las puertas cercanas.";

        public virtual float Range => 5f;

        public override void Tick()
        {
            base.Tick();

            Door d = GetClosestDoor();
            if ((d.Position - Player.Position).sqrMagnitude <= Range * Range && d.Base is IDamageableDoor door && door.IsDestroyed)
            {
                door.ServerRepair();
                d.IsOpened = true;
            }
        }
    }
}

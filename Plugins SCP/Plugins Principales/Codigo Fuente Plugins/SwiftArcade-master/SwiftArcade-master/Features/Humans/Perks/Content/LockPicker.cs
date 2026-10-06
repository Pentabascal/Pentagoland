using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Enums;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    //[Perk("LockPicker", Rarity.Mythic)]
    public class LockPicker(PerkInventory inv) : PerkDoorBase(inv)
    {
        public override string Name => "Cerrajero";

        public override string Description => "Abres cualquier puerta, incluso las cerradas con llave.";

        public override void OnDoorAction(DoorVariant door, DoorAction act, ReferenceHub hub)
        {
            if (door == null || hub != Player.ReferenceHub || act == DoorAction.Destroyed || act == DoorAction.Opened || act == DoorAction.Closed || door.DoorName.Equals(DoorName.Hcz079FirstGate.ToString()) || door.DoorName.Equals(DoorName.Hcz079SecondGate.ToString()) || door)
                return;

            door.NetworkTargetState = true;
        }
    }
}

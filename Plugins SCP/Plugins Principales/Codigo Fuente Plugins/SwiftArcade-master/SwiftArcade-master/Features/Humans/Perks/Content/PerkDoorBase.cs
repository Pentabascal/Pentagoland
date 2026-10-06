using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    public abstract class PerkDoorBase(PerkInventory inv) : PerkBase(inv)
    {
        public override void Init()
        {
            base.Init();

            DoorEvents.OnDoorAction += OnDoorAction;
        }

        public override void Remove()
        {
            base.Remove();

            DoorEvents.OnDoorAction -= OnDoorAction;
        }

        public virtual void OnDoorAction(DoorVariant door, DoorAction act, ReferenceHub hub) { }

        public Door GetClosestDoor()
        {
            Door closest = null;
            foreach (Door door in Door.List)
                if (closest == null || (door.Position - Player.Position).sqrMagnitude < (closest.Position - Player.Position).sqrMagnitude)
                    closest = door;
            return closest;
        }
    }
}

using InventorySystem.Items;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    public abstract class PerkItemReceiveBase(PerkInventory inv) : PerkTriggerCooldownBase(inv)
    {
        public abstract ItemType ItemType { get; }

        public virtual int Limit => 1;

        public override void Effect()
        {
            if (GetCount() <= Limit)
                GiveItem();
        }

        public virtual int GetCount() => Player.Items.Count((i) => i.Type == ItemType && AdditionalCondition(i));

        public virtual Item GiveItem() => Player.AddItem(ItemType, ItemAddReason.PickedUp);

        public virtual bool AdditionalCondition(Item i) => true;
    }
}

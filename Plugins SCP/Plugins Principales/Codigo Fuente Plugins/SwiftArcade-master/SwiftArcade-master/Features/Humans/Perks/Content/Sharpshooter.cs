using InventorySystem.Items;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Sharpshooter", Rarity.Uncommon)]
    public class Sharpshooter(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Tirador de élite";

        public override string Description => "Cada arma de fuego que recoges se convierte en un revólver.\nLas bajas con el revólver te otorgan AHP y una bala en la recámara.\nInfliges el doble de daño a los SCP con el revólver.";

        public virtual float Amount => 20f;
        public virtual float Efficacy => 1f;

        public override void Init()
        {
            base.Init();

            PlayerEvents.PickedUpItem += OnPickedUpItem;
            PlayerEvents.Hurting += OnHurting;

            Player.AddAmmo(ItemType.Ammo44cal, 50);
        }

        public override void Remove()
        {
            base.Remove();

            PlayerEvents.PickedUpItem -= OnPickedUpItem;
            PlayerEvents.Hurting -= OnHurting;
        }

        private void OnHurting(PlayerHurtingEventArgs ev)
        {
            if (ev.Attacker != Player || Player.CurrentItem == null || Player.CurrentItem.Type != ItemType.GunRevolver || !ev.Player.IsSCP || ev.DamageHandler is not FirearmDamageHandler handler)
                return;

            handler.Damage *= 2f;
        }

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player || Player.CurrentItem == null || Player.CurrentItem.Type != ItemType.GunRevolver)
                return;

            Player.ArtificialHealth += Amount;

            if (Player.CurrentItem is FirearmItem it && it.Base.TryGetModule(out CylinderAmmoModule mod))
                mod.ServerModifyAmmo(1);
        }

        private void OnPickedUpItem(PlayerPickedUpItemEventArgs ev)
        {
            if (ev.Player != Player || ev.Item.Category != ItemCategory.Firearm || ev.Item.Type == ItemType.GunRevolver)
                return;

            Player.RemoveItem(ev.Item);
            FirearmItem it = Player.AddItem(ItemType.GunRevolver, ItemAddReason.PickedUp) as FirearmItem;
            Player.AddAmmo(ItemType.Ammo44cal, 30);
            it.Base.ApplyAttachmentsCode(AttachmentsServerHandler.PlayerPreferences[Player.ReferenceHub][ItemType.GunRevolver], true);
        }
    }
}

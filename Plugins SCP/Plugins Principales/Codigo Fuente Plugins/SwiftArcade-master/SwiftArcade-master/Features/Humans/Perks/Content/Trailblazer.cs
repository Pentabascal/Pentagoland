using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using SwiftArcadeMode.Utils.Extensions;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Trailblazer", Rarity.Rare)]
    public class Trailblazer(PerkInventory inv) : PerkCooldownBase(inv)
    {
        public override string Name => "Pionero" + (initialized ? " | Objeto registrado: " + TrackedType + (!CooldownTimer.Ended ? " | Tiempo de recarga: " + Mathf.Round(CooldownTimer.CurrentValue) + "s" : "") : "");

        public override string PerkDescription => "Al usar un objeto, estableces un punto de teletransporte.\nVuelves a ese punto al usar otro objeto del mismo tipo.\nNo se registrarán tipos de objeto mientras haya un punto de teletransporte.";

        public override float Cooldown => 80f;

        public Vector3 TeleportPoint;
        public bool TeleportExists;
        public ItemType TrackedType = ItemType.None;
        public Elevator TrackedElevator;

        bool initialized;

        public override void Effect()
        {
            if (!TeleportExists)
                return;

            Player.Position = TrackedElevator != null ? TrackedElevator.Base.transform.position + TeleportPoint : TeleportPoint;
            TrackedElevator = null;
            TrackedType = ItemType.None;
            TeleportExists = false;
            SendMessage("¡Te has teletransportado! El punto de teletransporte se ha eliminado.");
        }

        public override void Init()
        {
            base.Init();

            WarheadEvents.Detonated += OnWarheadDetonated;
            PlayerEvents.UsingItem += OnUsingItem;
            PlayerEvents.UsedItem += OnUsedItem;

            if (Player != null)
                initialized = true;
        }

        private void OnWarheadDetonated(LabApi.Events.Arguments.WarheadEvents.WarheadDetonatedEventArgs ev)
        {
            if (TeleportExists && (TrackedElevator != null || !Room.TryGetRoomAtPosition(TeleportPoint, out Room room) || room.Zone != MapGeneration.FacilityZone.Surface))
            {
                TrackedElevator = null;
                TrackedType = ItemType.None;
                TeleportExists = false;
                SendMessage("¡La ojiva ha detonado! El punto de teletransporte se ha eliminado.");
            }
        }

        public override void Remove()
        {
            base.Remove();

            WarheadEvents.Detonated -= OnWarheadDetonated;
            PlayerEvents.UsingItem -= OnUsingItem;
            PlayerEvents.UsedItem -= OnUsedItem;
        }

        protected virtual void OnUsingItem(PlayerUsingItemEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            if (TeleportExists && ev.UsableItem.Type == TrackedType)
                SendMessage($"{(CooldownTimer.Ended ? "<color=#00FF00>¡Te teletransportarás!</color>" : "<color=#FF0000>¡NO te teletransportarás mientras esté en recarga!</color>")} Cancela para abortar.");
            else if (!TeleportExists && Player.Room.Name == MapGeneration.RoomName.Pocket)
                SendMessage("No puedes crear un punto de teletransporte en esta sala.");
        }

        protected virtual void OnUsedItem(PlayerUsedItemEventArgs ev) // Teleport point follows elevator
        {
            if (ev.Player != Player)
                return;

            if (TeleportExists)
            {
                if (ev.UsableItem.Type == TrackedType)
                    Trigger();
                return;
            }

            if (Player.Room.Name == MapGeneration.RoomName.Pocket)
            {
                SendMessage("No puedes crear un punto de teletransporte en esta sala.");
                return;
            }

            TrackedElevator = Player.GetElevator();
            TeleportPoint = TrackedElevator != null ? Player.Position - TrackedElevator.Base.transform.position : Player.Position;
            TrackedType = ev.UsableItem.Type;
            TeleportExists = true;
            SendMessage($"Se ha creado el punto de teletransporte{(!CooldownTimer.Ended ? " (teletransporte en recarga)" : "")}.");
        }
    }
}

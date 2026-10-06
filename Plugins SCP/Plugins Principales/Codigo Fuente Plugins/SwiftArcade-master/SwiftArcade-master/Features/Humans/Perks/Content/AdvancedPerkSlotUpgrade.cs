using LabApi.Events.Handlers;
using MEC;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("AdvSlotUpgrade", Rarity.Mythic)]
    public class AdvancedPerkSlotUpgrade(PerkInventory inv) : PerkSlotUpgrade(inv)
    {
        public override string Name => "Mejora avanzada de espacios para ventajas";
        public override string Description => $"Te otorga {Amount} espacios para ventajas y ocupa {SlotUsage} espacios.\nAl morir, perderás <color=red><b>TODAS LAS VENTAJAS</b></color> con seguridad.\nHay un {DropChance * 100f}% de probabilidad de soltar cada ventaja eliminada; esta mejora siempre se suelta.";

        public override int Amount => 12;
        public override int SlotUsage => 0;
        public virtual float DropChance => 0.3f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Dying += OnPlayerDying;
        }

        private void OnPlayerDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player || !Player.TryGetPerkInventory(out PerkInventory inv))
                return;

            Vector3 pos = Player.Position;

            PerkBase[] perks = [.. inv.Perks];
            foreach (PerkBase type in perks)
            {
                Type removed = type.GetType();
                inv.RemovePerk(removed);
                if (Random.Range(0f, 1f) <= DropChance || type == this)
                    PerkSpawner.SpawnPerk(PerkManager.GetPerk(removed), pos);
            }
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Dying -= OnPlayerDying;
        }
    }
}

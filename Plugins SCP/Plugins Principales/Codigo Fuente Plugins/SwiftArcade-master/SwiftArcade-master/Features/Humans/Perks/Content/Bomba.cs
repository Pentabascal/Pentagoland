using InventorySystem.Items.ThrowableProjectiles;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Bomba", Rarity.Common)]
    public class Bomba(PerkInventory inv) : PerkKillBase(inv)
    {
        public override string Name => "Bomba";

        public override string Description => "Al morir, sueltas una granada de mecha corta.\nEsta granada inflige menos daño a los SCP.";

        Vector3 savedPosition;

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            savedPosition = Player.Position;
        }

        protected override void OnPlayerDeath(PlayerDeathEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            TimedGrenadeProjectile proj = TimedGrenadeProjectile.SpawnActive(savedPosition, ItemType.GrenadeHE, Player, 1.75f);
            if (proj.Base is ExplosionGrenade gr)
                gr.ScpDamageMultiplier = 1.5f;
        }
    }
}

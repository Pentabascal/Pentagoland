using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("MicroMissiles", Rarity.Secret)]
    public class MicroMissiles(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Micromisiles";

        public override string Description => $"Cada {Amount} disparos, lanzas un proyectil explosivo.";

        public virtual int Amount => 10;

        int counter = 0;

        public override void Init()
        {
            base.Init();
            PlayerEvents.ShootingWeapon += OnShootingWeapon;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.ShootingWeapon -= OnShootingWeapon;
        }

        private void OnShootingWeapon(LabApi.Events.Arguments.PlayerEvents.PlayerShootingWeaponEventArgs ev)
        {
            if (ev.Player != Player)
                return;

            counter++;

            if (counter < Amount)
                return;

            counter = 0;

            ExplosiveGrenadeProjectile pick = TimedGrenadeProjectile.SpawnActive(Player.Camera.position, ItemType.GrenadeHE, Player, 3d) as ExplosiveGrenadeProjectile;
            pick.Base.MaxRadius = 2f;

            Rocketeer.ConvertRocket(Player, pick, 45f);
            pick.Base.ScpDamageMultiplier = 1f;
        }
    }
}

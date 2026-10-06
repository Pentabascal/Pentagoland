using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System.Collections.Generic;
using System.Linq;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP049.Overlord
{
    public class Soulbound(UpgradePathPerkBase parent) : UpgradeBase<Overlord>(parent)
    {
        public override string Name => "Vínculo de almas";

        public override string Description => "Cuando recibes daño con el escudo de Hume a 0, el daño se reparte entre los zombis cercanos.\nCuando mueren zombis, obtienes escudo de Hume.";

        public float HumeGain => 150f;
        public float Range => 15f;

        public override void Init()
        {
            base.Init();
            PlayerEvents.Hurting += OnHurting;
            PlayerEvents.Dying += OnDying;
        }

        public override void Remove()
        {
            base.Remove();
            PlayerEvents.Hurting -= OnHurting;
            PlayerEvents.Dying -= OnDying;
        }

        private void OnDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev)
        {
            if (ev.Player.Role != PlayerRoles.RoleTypeId.Scp0492)
                return;

            Player.HumeShield += HumeGain;
        }

        private void OnHurting(LabApi.Events.Arguments.PlayerEvents.PlayerHurtingEventArgs ev)
        {
            if (ev.Player != Player || Player.HumeShield > 0f || ev.DamageHandler is not StandardDamageHandler stand)
                return;

            IEnumerable<Player> zombies = Player.List.Where(p => p.IsSCP && p.Role == PlayerRoles.RoleTypeId.Scp0492 && (p.Position - Player.Position).sqrMagnitude <= Range * Range);
            int count = zombies.Count();

            if (count <= 0)
                return;

            float dmg = stand.Damage / count;

            foreach (Player zombie in zombies)
                zombie.Damage(dmg, ev.Attacker, (Player.Position - zombie.Position).normalized * 100f, 100);

            ev.Attacker?.SendHitMarker();

            ev.IsAllowed = false;
        }
    }
}

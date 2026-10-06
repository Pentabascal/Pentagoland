using InventorySystem.Items.Firearms.Modules;
using LabApi.Features.Wrappers;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.EndlessDecay
{
    public class GunMelter(UpgradePathPerkBase parent) : UpgradeCooldownTriggerBase<EndlessDecay>(parent)
    {
        public static List<ItemType> BannedTypes = [
            ItemType.GunSCP127,
            ItemType.ParticleDisruptor
            ];

        public override string Name => "Fundidor de armas";

        public override string UpgradeDescription => $"Derrites {Amount} balas del arma de cada humano cercano.";

        public override string ReadyMessage => "";

        public virtual int Amount => 3;
        public virtual float Radius => 5f;

        public override float Cooldown => 1f;

        public override void Effect()
        {
            foreach (Player p in Player.List.Where((p) => p.CurrentItem != null && !BannedTypes.Contains(p.CurrentItem.Type) && (p.Position - Player.Position).sqrMagnitude <= Radius * Radius))
                if (p.CurrentItem is FirearmItem it && it.Base.TryGetModule(out IPrimaryAmmoContainerModule mod))
                    mod.ServerModifyAmmo(-Amount);
        }
    }
}

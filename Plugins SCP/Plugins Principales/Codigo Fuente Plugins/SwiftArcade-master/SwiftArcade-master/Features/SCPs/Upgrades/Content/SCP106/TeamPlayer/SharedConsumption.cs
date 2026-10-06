using LabApi.Features.Wrappers;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP106.TeamPlayer
{
    public class SharedConsumption(UpgradePathPerkBase parent) : LifeConsumption(parent)
    {
        public override string Name => "Consumo compartido";

        public override string Description => base.Description + " Tu equipo también recibe esta cantidad.";

        public override void Effect()
        {
            foreach (Player p in Player.List.Where((p) => p.IsSCP && p.Role != PlayerRoles.RoleTypeId.Scp0492))
                if (p.HumeShield < p.MaxHumeShield)
                    p.HumeShield = Mathf.Clamp(p.HumeShield + CurrentAmount, 0f, p.MaxHumeShield);
        }
    }
}

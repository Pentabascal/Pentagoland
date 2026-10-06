using LabApi.Features.Wrappers;
using PlayerRoles.FirstPersonControl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwiftArcadeMode.Features.Humans.Perks.Content
{
    [Perk("Bully", Rarity.Rare)]
    public class Bully(PerkInventory inv) : PerkBase(inv)
    {
        public override string Name => "Matón";

        public override string Description => "Empuja a quienes estén cerca de ti.";

        public virtual float Strength => 3f;
        public virtual float Distance => 0.4f;

        readonly Dictionary<Player, Vector3> pushes = [];

        public override void Tick()
        {
            if (Player.RoleBase is not IFpcRole r || !r.FpcModule.IsGrounded || !r.FpcModule.Motor.MovementDetected)
                return;

            foreach (Player player in pushes.Keys)
                if (player.RoleBase is IFpcRole role)
                    role.FpcModule.ServerOverridePosition(pushes[player]);

            pushes.Clear();

            foreach (Player p in Player.List.Where(p => p != Player && (p.Position - Player.Position).sqrMagnitude <= Distance * Distance))
            {
                Vector3 dirForce = (p.Position - Player.Position).normalized * (Strength * Time.fixedDeltaTime);
                pushes.Add(p, p.Position + dirForce);
            }
        }
    }
}

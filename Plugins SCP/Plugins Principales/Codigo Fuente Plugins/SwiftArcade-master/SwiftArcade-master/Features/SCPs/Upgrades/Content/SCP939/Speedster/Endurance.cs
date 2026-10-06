using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using PlayerStatsSystem;
using SwiftArcadeMode.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP939.Speedster
{
    public class Endurance(UpgradePathPerkBase parent) : UpgradeKillBase<Speedster>(parent)
    {
        public override string Name => "Resistencia";

        public override string Description => "Recuperas resistencia al matar.";

        protected override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            if (ev.Attacker != Player)
                return;

            Player.StaminaRemaining = Player.ReferenceHub.playerStats.GetModule<StaminaStat>().MaxValue;
        }
    }
}

using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using InventorySystem.Items.Usables.Scp330;
using MEC;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class GivingCandyAbility : PassiveAbility
    {
        public override string Name { get; set; } = "Dar caramelos";

        public override string Description { get; set; } =
            "Gestiona la entrega de caramelos al jugador al comienzo de la ronda.";

        public List<CandyKindID> StartingCandy { get; set; } = new List<CandyKindID>()
        {
            CandyKindID.Pink
        };

        protected override void AbilityAdded(Player player)
        {
            Timing.CallDelayed(1.0f, () =>
            {
                Log.Debug($"VVUP Custom Abilities: Giving Candy to {player.Nickname}");
                foreach (var candy in StartingCandy)
                {
                    player.TryAddCandy(candy);
                }
            });
            base.AbilityAdded(player);
        }
    }
}
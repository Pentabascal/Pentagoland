using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class AbilityRemover : PassiveAbility
    {
        public override string Name { get; set; } = "Eliminador de habilidades";

        public override string Description { get; set; } =
            "Elimina cualquier habilidad aleatoria que el jugador pudiera recibir por algún motivo.";
        
        protected override void AbilityAdded(Player player)
        {
            Timing.CallDelayed(11f, () =>
            {
                Log.Debug($"Removing any left over ability data from {player.Nickname}");
                player.DisableAllEffects();
                player.Scale = Vector3.one;
            });
            base.AbilityAdded(player);
        }
    }
}
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Doors;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class DebugAbility : PassiveAbility
    {
        private static CoroutineHandle _debugCoroutine;
        
        public override string Name { get; set; } = "Habilidad de depuración";

        public override string Description { get; set; } =
            "Se usa para depurar: registra la posición del jugador cada 0,5 segundos, entre otros datos.";
        
        protected override void AbilityAdded(Player player)
        {
            _debugCoroutine = Timing.RunCoroutine(DebugTracking(player));
            Exiled.Events.Handlers.Player.InteractingDoor += OnDoorInteract;
            foreach (Door door in Door.List)
            {
                if (door == null)
                    continue;
                Log.Warn($"VVUP: {door.Name} - {door.Position} - {door.Zone}");
            }
            base.AbilityAdded(player);
        }

        protected override void AbilityRemoved(Player player)
        {
            Timing.KillCoroutines(_debugCoroutine);
            Exiled.Events.Handlers.Player.InteractingDoor -= OnDoorInteract;
            base.AbilityRemoved(player);
        }

        private static IEnumerator<float> DebugTracking(Player player)
        {
            for (;;)
            {
                Log.Warn($"VVUP: {player.Position}");
                Log.Warn($"VVUP: {player.CurrentRoom.LocalPosition(player.Position)}");
                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        private void OnDoorInteract(InteractingDoorEventArgs ev)
        {
            if (ev.Door != null)
                Log.Warn($"VVUP: {ev.Door.Name}");
        }
    }
}
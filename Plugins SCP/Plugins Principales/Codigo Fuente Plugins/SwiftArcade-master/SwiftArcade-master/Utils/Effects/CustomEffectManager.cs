using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using System.Collections.Generic;

namespace SwiftArcadeMode.Utils.Effects
{
    public static class CustomEffectManager
    {
        public static readonly Dictionary<Player, CustomEffectContainer> Containers = [];

        public static void Enable()
        {
            PlayerEvents.Dying += OnDying;
            PlayerEvents.Left += OnLeft;
        }

        public static void Disable()
        {
            PlayerEvents.Dying -= OnDying;
            PlayerEvents.Left -= OnLeft;
        }

        public static void Tick()
        {
            foreach (CustomEffectContainer cont in Containers.Values)
                cont?.Tick();
        }

        private static void OnLeft(LabApi.Events.Arguments.PlayerEvents.PlayerLeftEventArgs ev)
        {
            if (Containers.ContainsKey(ev.Player))
            {
                ev.Player.ClearCustomEffects();
                Containers.Remove(ev.Player);
            }
        }

        private static void OnDying(LabApi.Events.Arguments.PlayerEvents.PlayerDyingEventArgs ev) => ev.Player.ClearCustomEffects();

        public static void Register(Player player)
        {
            if (!Containers.ContainsKey(player))
                Containers.Add(player, new CustomEffectContainer(player));
        }

        public static void AddCustomEffect(this Player player, CustomEffectBase effect)
        {
            Register(player);
            Containers[player].AddEffect(effect);
        }

        public static void RemoveCustomEffect(this Player player, CustomEffectBase effect)
        {
            if (Containers.ContainsKey(player))
                Containers[player].RemoveEffect(effect);
        }

        public static void ClearCustomEffects(this Player player)
        {
            if (Containers.ContainsKey(player))
                Containers[player].ClearEffects();
        }
    }
}

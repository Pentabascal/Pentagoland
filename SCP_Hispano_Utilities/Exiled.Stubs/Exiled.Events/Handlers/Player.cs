using System;

namespace Exiled.Events.Handlers
{
    public static class Player
    {
        public static event Action<Exiled.Events.EventArgs.Player.VerifiedEventArgs> Verified;
        public static event Action<Exiled.Events.EventArgs.Player.ChattedEventArgs> Chatted;
        public static event Action<Exiled.Events.EventArgs.Player.DyingEventArgs> Dying;

        public static void RaiseVerified(Exiled.API.Features.Player player)
        {
            Verified?.Invoke(new Exiled.Events.EventArgs.Player.VerifiedEventArgs(player));
        }

        public static void RaiseChatted(Exiled.API.Features.Player player, string message)
        {
            Chatted?.Invoke(new Exiled.Events.EventArgs.Player.ChattedEventArgs(player, message));
        }

        public static void RaiseDying(Exiled.API.Features.Player player, Exiled.API.Features.Player attacker)
        {
            Dying?.Invoke(new Exiled.Events.EventArgs.Player.DyingEventArgs(player, attacker));
        }
    }
}

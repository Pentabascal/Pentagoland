using System;

namespace Exiled.Events.Handlers
{
    public static class Server
    {
        public static event Action RoundStarted;
        public static event Action<Exiled.Events.EventArgs.Server.RoundEndedEventArgs> RoundEnded;

        public static void RaiseRoundStarted()
        {
            RoundStarted?.Invoke();
        }

        public static void RaiseRoundEnded()
        {
            RoundEnded?.Invoke(new Exiled.Events.EventArgs.Server.RoundEndedEventArgs());
        }
    }
}

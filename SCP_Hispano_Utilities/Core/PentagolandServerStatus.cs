using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;

namespace Pentagoland.Utilities
{
    public sealed class PentagolandServerStatus
    {
        public string ServerName { get; set; }
        public string ServerTagline { get; set; }
        public int OnlinePlayers { get; set; }
        public int TotalPlayersSeen { get; set; }
        public int RoundsStarted { get; set; }
        public int RoundsCompleted { get; set; }
        public DateTime GeneratedAtUtc { get; set; }
        public string StatusMessage { get; set; }

        public static PentagolandServerStatus FromSnapshot(PentagolandStatisticsSnapshot snapshot, PentagolandConfig config = null)
        {
            var effectiveConfig = config ?? new PentagolandConfig();
            return new PentagolandServerStatus
            {
                ServerName = effectiveConfig.ServerName,
                ServerTagline = effectiveConfig.ServerTagline,
                OnlinePlayers = snapshot != null ? Math.Max(0, snapshot.OnlinePlayers) : 0,
                TotalPlayersSeen = snapshot != null && snapshot.Players != null ? snapshot.Players.Count : 0,
                RoundsStarted = snapshot != null && snapshot.Server != null ? snapshot.Server.RoundsStarted : 0,
                RoundsCompleted = snapshot != null && snapshot.Server != null ? snapshot.Server.RoundsCompleted : 0,
                GeneratedAtUtc = DateTime.UtcNow,
                StatusMessage = BuildStatusText(effectiveConfig, snapshot)
            };
        }

        public static string BuildStatusText(PentagolandConfig config = null, PentagolandStatisticsSnapshot snapshot = null)
        {
            var effectiveConfig = config ?? new PentagolandConfig();
            var onlinePlayers = snapshot != null ? snapshot.OnlinePlayers : Player.List.Count();
            var roundsStarted = snapshot != null && snapshot.Server != null ? snapshot.Server.RoundsStarted : 0;
            var roundsCompleted = snapshot != null && snapshot.Server != null ? snapshot.Server.RoundsCompleted : 0;

            return string.Format(
                "[{0}] {1} | Jugadores en línea: {2} | Rondas iniciadas: {3} | Rondas finalizadas: {4}",
                effectiveConfig.ServerName,
                effectiveConfig.ServerTagline,
                onlinePlayers,
                roundsStarted,
                roundsCompleted);
        }
    }
}

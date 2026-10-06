using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Pentagoland.Utilities
{
    public class PentagolandStatsStore
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<string, PentagolandPlayerStatistics> _players = new Dictionary<string, PentagolandPlayerStatistics>(StringComparer.OrdinalIgnoreCase);
        private readonly PentagolandServerStatistics _server = new PentagolandServerStatistics();

        public void AddPoints(string userId, int amount)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return;

            lock (_syncRoot)
                GetOrCreatePlayer(userId).Points += amount;
        }

        public int GetPoints(string userId)
        {
            return GetPlayerStatistics(userId).Points;
        }

        public void AddWarning(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return;

            lock (_syncRoot)
                GetOrCreatePlayer(userId).Warnings++;
        }

        public int GetWarnings(string userId)
        {
            return GetPlayerStatistics(userId).Warnings;
        }

        public void RecordDeath(string victimId, string attackerId)
        {
            if (string.IsNullOrWhiteSpace(victimId))
                return;

            lock (_syncRoot)
            {
                GetOrCreatePlayer(victimId).Deaths++;
                if (!string.IsNullOrWhiteSpace(attackerId) && !string.Equals(victimId, attackerId, StringComparison.OrdinalIgnoreCase))
                    GetOrCreatePlayer(attackerId).Kills++;
            }
        }

        public void RecordRoundStarted(DateTime startedAtUtc)
        {
            lock (_syncRoot)
            {
                _server.RoundsStarted++;
                _server.LastRoundStartedUtc = startedAtUtc;
            }
        }

        public void RecordRoundEnded(DateTime endedAtUtc)
        {
            lock (_syncRoot)
            {
                _server.RoundsCompleted++;
                _server.LastRoundEndedUtc = endedAtUtc;
            }
        }

        public PentagolandPlayerStatistics GetPlayerStatistics(string userId)
        {
            lock (_syncRoot)
            {
                return _players.TryGetValue(userId ?? string.Empty, out var statistics)
                    ? Clone(statistics)
                    : new PentagolandPlayerStatistics();
            }
        }

        public PentagolandServerStatistics GetServerStatistics()
        {
            lock (_syncRoot)
                return Clone(_server);
        }

        public PentagolandStatisticsSnapshot CreateSnapshot(int onlinePlayers, DateTime generatedAtUtc)
        {
            lock (_syncRoot)
            {
                return new PentagolandStatisticsSnapshot
                {
                    SchemaVersion = 1,
                    ServerName = "Pentagoland",
                    GeneratedAtUtc = generatedAtUtc,
                    OnlinePlayers = Math.Max(0, onlinePlayers),
                    Server = Clone(_server),
                    Players = _players.ToDictionary(pair => pair.Key, pair => Clone(pair.Value), StringComparer.OrdinalIgnoreCase)
                };
            }
        }

        public void Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return;

            var serializer = new DataContractJsonSerializer(typeof(PentagolandStatisticsSnapshot));
            PentagolandStatisticsSnapshot snapshot;
            using (var stream = File.OpenRead(filePath))
                snapshot = serializer.ReadObject(stream) as PentagolandStatisticsSnapshot;

            if (snapshot == null || snapshot.SchemaVersion != 1)
                throw new InvalidDataException("El archivo de estadísticas tiene una versión incompatible.");

            lock (_syncRoot)
            {
                _players.Clear();
                if (snapshot.Players != null)
                {
                    foreach (var pair in snapshot.Players)
                    {
                        if (!string.IsNullOrWhiteSpace(pair.Key) && pair.Value != null)
                            _players[pair.Key] = Clone(pair.Value);
                    }
                }

                Copy(snapshot.Server, _server);
            }
        }

        public void Save(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            WriteJsonAtomically(filePath, CreateSnapshot(0, DateTime.UtcNow));
        }

        public void WriteSnapshot(string filePath, int onlinePlayers, DateTime generatedAtUtc)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            WriteJsonAtomically(filePath, CreateSnapshot(onlinePlayers, generatedAtUtc));
        }

        private static void WriteJsonAtomically<T>(string filePath, T value)
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(filePath));
            Directory.CreateDirectory(directory);
            var temporaryPath = filePath + ".tmp";
            var serializer = new DataContractJsonSerializer(typeof(T));

            try
            {
                using (var stream = new FileStream(temporaryPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    serializer.WriteObject(stream, value);

                if (File.Exists(filePath))
                    File.Replace(temporaryPath, filePath, null);
                else
                    File.Move(temporaryPath, filePath);
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }

        private PentagolandPlayerStatistics GetOrCreatePlayer(string userId)
        {
            if (!_players.TryGetValue(userId, out var statistics))
            {
                statistics = new PentagolandPlayerStatistics();
                _players[userId] = statistics;
            }

            return statistics;
        }

        private static PentagolandPlayerStatistics Clone(PentagolandPlayerStatistics source)
        {
            return new PentagolandPlayerStatistics
            {
                Points = source.Points,
                Kills = source.Kills,
                Deaths = source.Deaths,
                Warnings = source.Warnings
            };
        }

        private static PentagolandServerStatistics Clone(PentagolandServerStatistics source)
        {
            return new PentagolandServerStatistics
            {
                RoundsStarted = source.RoundsStarted,
                RoundsCompleted = source.RoundsCompleted,
                LastRoundStartedUtc = source.LastRoundStartedUtc,
                LastRoundEndedUtc = source.LastRoundEndedUtc
            };
        }

        private static void Copy(PentagolandServerStatistics source, PentagolandServerStatistics destination)
        {
            if (source == null)
                return;

            destination.RoundsStarted = source.RoundsStarted;
            destination.RoundsCompleted = source.RoundsCompleted;
            destination.LastRoundStartedUtc = source.LastRoundStartedUtc;
            destination.LastRoundEndedUtc = source.LastRoundEndedUtc;
        }
    }

    [DataContract]
    public sealed class PentagolandPlayerStatistics
    {
        [DataMember(Order = 1)] public int Points { get; set; }
        [DataMember(Order = 2)] public int Kills { get; set; }
        [DataMember(Order = 3)] public int Deaths { get; set; }
        [DataMember(Order = 4)] public int Warnings { get; set; }
    }

    [DataContract]
    public sealed class PentagolandServerStatistics
    {
        [DataMember(Order = 1)] public int RoundsStarted { get; set; }
        [DataMember(Order = 2)] public int RoundsCompleted { get; set; }
        [DataMember(Order = 3)] public DateTime LastRoundStartedUtc { get; set; }
        [DataMember(Order = 4)] public DateTime LastRoundEndedUtc { get; set; }
    }

    [DataContract]
    public sealed class PentagolandStatisticsSnapshot
    {
        [DataMember(Order = 1)] public int SchemaVersion { get; set; }
        [DataMember(Order = 2)] public string ServerName { get; set; }
        [DataMember(Order = 3)] public DateTime GeneratedAtUtc { get; set; }
        [DataMember(Order = 4)] public int OnlinePlayers { get; set; }
        [DataMember(Order = 5)] public PentagolandServerStatistics Server { get; set; }
        [DataMember(Order = 6)] public Dictionary<string, PentagolandPlayerStatistics> Players { get; set; }
    }

    public class HispanoStatsStore : PentagolandStatsStore
    {
    }
}
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Pentagoland.Utilities
{
    public class PentagolandStatsStore
    {
        private readonly object _syncRoot = new object();
        private readonly PentagolandServerStatistics _server = new PentagolandServerStatistics();

        public void RecordRoundStarted(DateTime startedAtUtc)
        {
            lock (_syncRoot)
            {
                _server.RoundsStarted++;
                _server.LastRoundStartedUtc = NormalizeUtc(startedAtUtc);
            }
        }

        public void RecordRoundEnded(DateTime endedAtUtc)
        {
            lock (_syncRoot)
            {
                _server.RoundsCompleted++;
                _server.LastRoundEndedUtc = NormalizeUtc(endedAtUtc);
            }
        }

        public PentagolandServerStatistics GetServerStatistics()
        {
            lock (_syncRoot)
                return Clone(_server);
        }

        public PentagolandServerStatus CreateStatus(
            int onlinePlayers,
            int maxPlayers,
            string serverName,
            string serverTagline)
        {
            return PentagolandServerStatus.Create(onlinePlayers, maxPlayers, serverName, serverTagline);
        }

        public void Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return;

            var serializer = new DataContractJsonSerializer(typeof(PentagolandPersistedServerStatistics));
            PentagolandPersistedServerStatistics persisted;
            using (var stream = File.OpenRead(filePath))
                persisted = serializer.ReadObject(stream) as PentagolandPersistedServerStatistics;

            if (persisted == null || persisted.SchemaVersion != 1)
                throw new InvalidDataException("El archivo de estadísticas agregadas tiene una versión incompatible.");

            lock (_syncRoot)
                Copy(persisted.Server, _server);
        }

        public void Save(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            WriteJsonAtomically(filePath, new PentagolandPersistedServerStatistics
            {
                SchemaVersion = 1,
                Server = GetServerStatistics()
            });
        }

        public void WriteSnapshot(
            string filePath,
            int onlinePlayers,
            int maxPlayers,
            string serverName,
            string serverTagline)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            WriteJsonAtomically(filePath, CreateStatus(
                onlinePlayers,
                maxPlayers,
                serverName,
                serverTagline));
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

        private static PentagolandServerStatistics Clone(PentagolandServerStatistics source)
        {
            return new PentagolandServerStatistics
            {
                RoundsStarted = source.RoundsStarted,
                RoundsCompleted = source.RoundsCompleted,
                LastRoundStartedUtc = NormalizeUtc(source.LastRoundStartedUtc),
                LastRoundEndedUtc = NormalizeUtc(source.LastRoundEndedUtc)
            };
        }

        private static void Copy(PentagolandServerStatistics source, PentagolandServerStatistics destination)
        {
            if (source == null)
                return;

            destination.RoundsStarted = source.RoundsStarted;
            destination.RoundsCompleted = source.RoundsCompleted;
            destination.LastRoundStartedUtc = NormalizeUtc(source.LastRoundStartedUtc);
            destination.LastRoundEndedUtc = NormalizeUtc(source.LastRoundEndedUtc);
        }

        private static DateTime NormalizeUtc(DateTime value)
        {
            if (value == DateTime.MinValue || value == DateTime.MaxValue)
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);

            if (value.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(value, DateTimeKind.Utc);

            return value.ToUniversalTime();
        }
    }

    [DataContract]
    public sealed class PentagolandServerStatistics
    {
        [DataMember(Order = 1)]
        public int RoundsStarted { get; set; }

        [DataMember(Order = 2)]
        public int RoundsCompleted { get; set; }

        [DataMember(Order = 3)]
        public DateTime LastRoundStartedUtc { get; set; } = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

        [DataMember(Order = 4)]
        public DateTime LastRoundEndedUtc { get; set; } = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    }

    [DataContract]
    internal sealed class PentagolandPersistedServerStatistics
    {
        [DataMember(Order = 1)]
        public int SchemaVersion { get; set; }

        [DataMember(Order = 2)]
        public PentagolandServerStatistics Server { get; set; }
    }

    public class HispanoStatsStore : PentagolandStatsStore
    {
    }
}

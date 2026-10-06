using System;
using System.IO;
using System.Threading;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Player = Exiled.API.Features.Player;

namespace Pentagoland.Utilities
{
    public class PentagolandCore : Plugin<PentagolandConfig>
    {
        public override string Name => "Pentagoland Core";
        public override string Author => "Pentagoland Development Team";
        public override Version Version => new Version(4, 3, 0);
        public override Version RequiredExiledVersion => new Version(9, 14, 2);

        public static PentagolandCore Instance { get; private set; }

        private readonly object _snapshotLock = new object();
        private readonly PentagolandStatsStore _stats = new PentagolandStatsStore();
        private string _statisticsPath;
        private string _snapshotPath;
        private bool _statisticsLoadFailed;
        private volatile int _onlinePlayerCount;
        private volatile int _maxPlayerCount;
        private Timer _snapshotHeartbeat;

        public override void OnEnabled()
        {
            if (Instance != null && Instance != this)
            {
                Log.Error("Ya hay una instancia de Pentagoland Core activa; la instancia duplicada queda inactiva.");
                return;
            }

            Instance = this;
            base.OnEnabled();
            _statisticsPath = ResolveDataPath(Config.StatisticsFileName);
            _snapshotPath = ResolveDataPath(Config.IntegrationSnapshotFileName);

            if (Config.PersistStatistics)
                TryLoadStatistics();

            if (Config.CollectRoundStatistics || Config.PersistStatistics)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            }

            if (Config.EnableJsonIntegrationSnapshot)
            {
                Exiled.Events.Handlers.Player.Verified += OnPlayerStatusChanged;
                Exiled.Events.Handlers.Player.Left += OnPlayerStatusChanged;
                RefreshPlayerCounts();
                WriteIntegrationSnapshot();
                var intervalMilliseconds = Math.Max(10, Math.Min(300, Config.SnapshotIntervalSeconds)) * 1000;
                _snapshotHeartbeat = new Timer(OnSnapshotHeartbeat, null, intervalMilliseconds, intervalMilliseconds);
            }

            PersistCurrentState();
            Log.Info($"Pentagoland Core v{Version} iniciado como proveedor auxiliar de estado del servidor.");
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Verified -= OnPlayerStatusChanged;
            Exiled.Events.Handlers.Player.Left -= OnPlayerStatusChanged;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            _snapshotHeartbeat?.Dispose();
            _snapshotHeartbeat = null;

            if (Instance == this)
            {
                Instance = null;
                PersistCurrentState();
            }

            base.OnDisabled();
        }

        public PentagolandServerStatistics GetServerStatistics()
        {
            return _stats.GetServerStatistics();
        }

        public PentagolandServerStatus GetServerStatus()
        {
            return _stats.CreateStatus(
                Server.PlayerConnectedCount,
                Server.MaxPlayerCount,
                Config.ServerName,
                Config.ServerTagline);
        }

        public void WriteIntegrationSnapshot()
        {
            WriteSnapshot(_snapshotPath);
        }

        private void OnSnapshotHeartbeat(object state)
        {
            if (Instance == this && _snapshotHeartbeat != null)
                WriteSnapshot(_snapshotPath);
        }

        private void OnPlayerStatusChanged(VerifiedEventArgs ev)
        {
            RefreshPlayerCounts();
            WriteIntegrationSnapshot();
        }

        private void OnPlayerStatusChanged(LeftEventArgs ev)
        {
            RefreshPlayerCounts();
            WriteIntegrationSnapshot();
        }

        private void OnRoundStarted()
        {
            RefreshPlayerCounts();
            if (Config.CollectRoundStatistics)
                _stats.RecordRoundStarted(DateTime.UtcNow);

            WriteIntegrationSnapshot();
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            RefreshPlayerCounts();
            if (Config.CollectRoundStatistics)
                _stats.RecordRoundEnded(DateTime.UtcNow);

            PersistCurrentState();
        }

        private void PersistCurrentState()
        {
            if (Config.PersistStatistics)
                TrySaveStatistics();
            if (Config.EnableJsonIntegrationSnapshot)
                WriteSnapshot(_snapshotPath);
        }

        private void TryLoadStatistics()
        {
            try
            {
                _stats.Load(_statisticsPath);
            }
            catch (Exception ex)
            {
                _statisticsLoadFailed = true;
                Log.Error($"No se pudieron cargar las estadísticas agregadas de Pentagoland: {ex}");
            }
        }

        private void TrySaveStatistics()
        {
            if (_statisticsLoadFailed)
            {
                Log.Error("No se sobrescribieron las estadísticas de rondas de Pentagoland porque el archivo existente no pudo cargarse.");
                return;
            }

            try
            {
                _stats.Save(_statisticsPath);
            }
            catch (Exception ex)
            {
                Log.Error($"No se pudieron guardar las estadísticas de rondas de Pentagoland: {ex}");
            }
        }

        private void WriteSnapshot(string path)
        {
            if (!Config.EnableJsonIntegrationSnapshot || string.IsNullOrWhiteSpace(path))
                return;

            lock (_snapshotLock)
            {
                try
                {
                    _stats.WriteSnapshot(
                        path,
                        _onlinePlayerCount,
                        _maxPlayerCount,
                        Config.ServerName,
                        Config.ServerTagline);
                }
                catch (Exception ex)
                {
                    Log.Error($"No se pudo actualizar el snapshot público de Pentagoland: {ex}");
                }
            }
        }

        private void RefreshPlayerCounts()
        {
            _onlinePlayerCount = Math.Max(0, Server.PlayerConnectedCount);
            _maxPlayerCount = Math.Max(1, Server.MaxPlayerCount);
        }

        private string ResolveDataPath(string fileName)
        {
            var safeName = Path.GetFileName(string.IsNullOrWhiteSpace(fileName) ? "datos.json" : fileName);
            var directory = string.IsNullOrWhiteSpace(Config.DataDirectory)
                ? "PentagolandData"
                : Config.DataDirectory;
            var basePath = Path.IsPathRooted(directory)
                ? directory
                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            return Path.Combine(basePath, safeName);
        }
    }
}

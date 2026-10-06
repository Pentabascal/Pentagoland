using System;
using System.Collections.Generic;
using System.Linq;
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
        public override Version Version => new Version(4, 1, 0);
        public override Version RequiredExiledVersion => new Version(8, 0, 0);

        public static PentagolandCore Instance { get; private set; }

        private readonly object _reportLock = new object();
        private readonly Dictionary<string, DateTime> _reportCooldowns = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private readonly PentagolandStatsStore _stats = new PentagolandStatsStore();
        private string _statisticsPath;
        private string _snapshotPath;

        public event Action<PentagolandReport> ReportRegistered;

        public override void OnEnabled()
        {
            base.OnEnabled();
            if (Instance != null && Instance != this)
            {
                Log.Error("Ya hay una instancia de Pentagoland Core activa; la instancia duplicada queda inactiva.");
                return;
            }

            Instance = this;
            _statisticsPath = ResolveDataPath(Config.StatisticsFileName);
            _snapshotPath = ResolveDataPath(Config.IntegrationSnapshotFileName);

            if (Config.PersistStatistics)
                TryLoadStatistics();

            if (Config.CollectCombatStatistics)
                Exiled.Events.Handlers.Player.Dying += OnDying;
            if (Config.CollectRoundStatistics || Config.PersistStatistics || Config.EnableJsonIntegrationSnapshot)
            {
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
                Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            }

            if (Config.EnableAutoServerSnapshot)
                WriteIntegrationSnapshot();

            PersistCurrentState();
            Log.Info($"Pentagoland Core v{Version} iniciado en modo pasivo con compatibilidad ampliada.");
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            if (Instance == this)
            {
                PersistCurrentState();
                Instance = null;
            }
            base.OnDisabled();
        }

        public void RegisterReport(Player reporter, Player target, string reason)
        {
            TryRegisterReport(reporter, target, reason);
        }

        public bool TryRegisterReport(Player reporter, Player target, string reason)
        {
            if (reporter == null || target == null || string.IsNullOrWhiteSpace(reason))
                return false;

            var reporterId = GetUserId(reporter);
            var now = DateTime.UtcNow;
            lock (_reportLock)
            {
                if (_reportCooldowns.TryGetValue(reporterId, out var lastReport)
                    && (now - lastReport).TotalSeconds < Math.Max(0, Config.ReportCooldownSeconds))
                    return false;

                _reportCooldowns[reporterId] = now;
            }

            var normalizedReason = reason.Trim();
            if (normalizedReason.Length > Config.MaxReportLengthCharacters)
                normalizedReason = normalizedReason.Substring(0, Math.Max(0, Config.MaxReportLengthCharacters));

            var report = new PentagolandReport
            {
                ReporterId = reporterId,
                ReporterName = reporter.Nickname ?? string.Empty,
                TargetId = GetUserId(target),
                TargetName = target.Nickname ?? string.Empty,
                Reason = normalizedReason,
                CreatedAtUtc = now
            };

            if (Config.LogReports)
                Log.Info($"[Reporte Pentagoland] {report.ReporterName} ({report.ReporterId}) -> {report.TargetName} ({report.TargetId}): {report.Reason}");

            NotifyReportListeners(report);
            return true;
        }

        public void AddPoints(Player player, int amount)
        {
            if (player != null && amount != 0)
                _stats.AddPoints(GetUserId(player), amount);
        }

        public int GetPoints(Player player)
        {
            return player == null ? 0 : _stats.GetPoints(GetUserId(player));
        }

        public void AddWarning(Player player)
        {
            if (player != null)
                _stats.AddWarning(GetUserId(player));
        }

        public int GetWarnings(Player player)
        {
            return player == null ? 0 : _stats.GetWarnings(GetUserId(player));
        }

        public PentagolandPlayerStatistics GetStatistics(Player player)
        {
            return player == null ? new PentagolandPlayerStatistics() : _stats.GetPlayerStatistics(GetUserId(player));
        }

        public PentagolandServerStatistics GetServerStatistics()
        {
            return _stats.GetServerStatistics();
        }

        public PentagolandStatisticsSnapshot CreateIntegrationSnapshot()
        {
            return _stats.CreateSnapshot(Player.List.Count(), DateTime.UtcNow);
        }

        public PentagolandServerStatus GetServerStatus()
        {
            var snapshot = CreateIntegrationSnapshot();
            return PentagolandServerStatus.FromSnapshot(snapshot, Config);
        }

        public bool CanUseAdminCommand(Player player, string command)
        {
            if (player == null || string.IsNullOrWhiteSpace(command))
                return false;

            var commandName = PentagolandCommandRegistry.GetCommandName(command, Config.CommandPrefix);
            if (string.IsNullOrWhiteSpace(commandName))
                return false;

            if (!PentagolandCommandRegistry.IsAdminOnlyCommand(commandName, Config.CommandPrefix))
                return true;

            return PentagolandCommandGuard.IsAdministrator(player, Config);
        }

        public void WriteIntegrationSnapshot()
        {
            WriteSnapshot(_snapshotPath);
        }

        private void OnDying(DyingEventArgs ev)
        {
            if (ev?.Player == null)
                return;

            _stats.RecordDeath(GetUserId(ev.Player), ev.Attacker == null ? null : GetUserId(ev.Attacker));
        }

        private void OnRoundStarted()
        {
            if (Config.CollectRoundStatistics)
                _stats.RecordRoundStarted(DateTime.UtcNow);
        }

        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
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
                Log.Error($"No se pudieron cargar las estadísticas de Pentagoland: {ex.Message}");
            }
        }

        private void TrySaveStatistics()
        {
            try
            {
                _stats.Save(_statisticsPath);
            }
            catch (Exception ex)
            {
                Log.Error($"No se pudieron guardar las estadísticas de Pentagoland: {ex.Message}");
            }
        }

        private void WriteSnapshot(string path)
        {
            if (!Config.EnableJsonIntegrationSnapshot || string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                _stats.WriteSnapshot(path, Player.List.Count(), DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                Log.Error($"No se pudo actualizar el snapshot de Pentagoland: {ex.Message}");
            }
        }

        private void NotifyReportListeners(PentagolandReport report)
        {
            var listeners = ReportRegistered;
            if (listeners == null)
                return;

            foreach (Action<PentagolandReport> listener in listeners.GetInvocationList())
            {
                try
                {
                    listener(report);
                }
                catch (Exception ex)
                {
                    Log.Error($"Un plugin externo falló al procesar un reporte de Pentagoland: {ex.Message}");
                }
            }
        }

        private string ResolveDataPath(string fileName)
        {
            var safeName = System.IO.Path.GetFileName(string.IsNullOrWhiteSpace(fileName) ? "datos.json" : fileName);
            var directory = string.IsNullOrWhiteSpace(Config.DataDirectory)
                ? "PentagolandData"
                : Config.DataDirectory;
            var basePath = System.IO.Path.IsPathRooted(directory)
                ? directory
                : System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);
            return System.IO.Path.Combine(basePath, safeName);
        }

        private static string GetUserId(Player player)
        {
            return !string.IsNullOrWhiteSpace(player.UserId) ? player.UserId : (player.Nickname ?? string.Empty);
        }
    }

    public sealed class PentagolandReport
    {
        public string ReporterId { get; set; }
        public string ReporterName { get; set; }
        public string TargetId { get; set; }
        public string TargetName { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
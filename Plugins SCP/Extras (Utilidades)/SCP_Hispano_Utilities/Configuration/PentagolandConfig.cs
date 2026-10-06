using System.ComponentModel;
using Exiled.API.Interfaces;

namespace Pentagoland.Utilities
{
    public class PentagolandConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Conserva contadores agregados de rondas; no guarda estadísticas de jugadores.")]
        public bool CollectRoundStatistics { get; set; } = true;
        public bool PersistStatistics { get; set; } = true;
        [Description("Publica un snapshot JSON sin nombres, IDs ni estadísticas de jugadores.")]
        public bool EnableJsonIntegrationSnapshot { get; set; } = true;

        public string ServerName { get; set; } = "Pentagoland";
        public string ServerTagline { get; set; } = "Servidor hispano de SCP: Secret Laboratory";

        public string DataDirectory { get; set; } = "PentagolandData";
        public string StatisticsFileName { get; set; } = "estadisticas.json";
        public string IntegrationSnapshotFileName { get; set; } = "estado.json";
        public int SnapshotIntervalSeconds { get; set; } = 30;
    }

    public class HispanoConfig : PentagolandConfig
    {
    }
}
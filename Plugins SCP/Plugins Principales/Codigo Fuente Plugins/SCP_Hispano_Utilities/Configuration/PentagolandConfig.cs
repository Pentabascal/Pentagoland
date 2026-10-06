using System;
using Exiled.API.Interfaces;

namespace Pentagoland.Utilities
{
    public class PentagolandConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public bool CollectCombatStatistics { get; set; } = true;
        public bool CollectRoundStatistics { get; set; } = true;
        public bool PersistStatistics { get; set; } = true;
        public bool EnableJsonIntegrationSnapshot { get; set; } = true;
        public bool LogReports { get; set; } = true;
        public bool EnableCompatibilityLayer { get; set; } = true;
        public bool EnableAutoServerSnapshot { get; set; } = true;
        public bool EnableAdminOnlyRestrictions { get; set; } = true;
        public bool AllowPublicInfoCommands { get; set; } = true;
        public bool AutoCreateDataDirectories { get; set; } = true;

        public string ServerName { get; set; } = "Pentagoland";
        public string ServerTagline { get; set; } = "Servidor hispano de SCP: Secret Laboratory";
        public string CommandPrefix { get; set; } = "!";
        public string[] AdminUserIds { get; set; } = Array.Empty<string>();
        public string[] AdminRoles { get; set; } = { "owner", "admin", "moderator", "staff" };

        public string WelcomeMessage { get; set; } = "Bienvenido a Pentagoland. Respeta a la comunidad y disfruta del servidor.";
        public string RulesMessage { get; set; } = "Reglas: respeta a todos, evita abuso de comandos y participa de forma correcta.";
        public string RoundStartMessage { get; set; } = "¡La ronda ha empezado! Buena suerte y que te vaya bien.";
        public string RoundEndMessage { get; set; } = "La ronda ha finalizado. Gracias por jugar en Pentagoland.";

        public string DataDirectory { get; set; } = "PentagolandData";
        public string StatisticsFileName { get; set; } = "estadisticas.json";
        public string IntegrationSnapshotFileName { get; set; } = "estado.json";
        public int ReportCooldownSeconds { get; set; } = 30;
        public int WelcomeDurationSeconds { get; set; } = 8;
        public int RulesBroadcastDurationSeconds { get; set; } = 10;
        public int PlayerInfoDurationSeconds { get; set; } = 6;
        public int MaxReportLengthCharacters { get; set; } = 500;
    }

    public class HispanoConfig : PentagolandConfig
    {
    }
}
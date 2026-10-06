using System.Collections.Generic;
using System.ComponentModel;

namespace StatsSystem;

internal sealed class Config
{
    [Description("Activa los mensajes de depuración detallados en la consola del servidor.")]
    public bool Debug { get; set; } = false;

    [Description("Registra el tiempo total de juego de cada jugador (se guarda como la duración 'TotalPlayTime').")]
    public bool PlaytimeTracking { get; set; } = true;

    [Description("Registra las bajas totales de cada jugador.")]
    public bool KillsTracking { get; set; } = true;

    [Description("Registra las muertes totales de cada jugador.")]
    public bool DeathsTracking { get; set; } = true;

    [Description("Registra las bajas realizadas por jugadores de Clase D.")]
    public bool KillsAsClassDTracking { get; set; } = true;

    [Description("Registra las bajas cuya víctima sea de Clase D.")]
    public bool ClassDKillsTracking { get; set; } = true;

    [Description("Registra las bajas cuya víctima sea un SCP.")]
    public bool ScpKillsTracking { get; set; } = true;

    [Description("Registra las bajas realizadas con el MicroHID.")]
    public bool MicroHidKillsTracking { get; set; } = true;

    [Description("Periodos históricos, en días, que se muestran con /getstat y /getleaderboard. Por ejemplo: [7, 30, 90].")]
    public List<int> LastDays { get; set; } = [7, 30, 90];

    [Description("Carpeta, relativa a la ruta de configuración del plugin, donde se guardan las estadísticas.")]
    public string StatsDataFolder { get; set; } = "StatsSystem";

    [Description("Cada cuánto se guardan automáticamente las estadísticas en disco, en segundos. Mínimo: 10.")]
    public int AutoSaveIntervalSeconds { get; set; } = 60;

    [Description(
        "Formato de almacenamiento: Json (predeterminado, legible y compatible con v1) o Binary (más compacto y rápido, pero no legible). Al cambiar de formato, los archivos existentes se convierten automáticamente al iniciar el servidor.")]
    public StorageProviderType StorageProvider { get; set; } = StorageProviderType.Json;
}

public enum StorageProviderType
{
    Json,
    Binary
}
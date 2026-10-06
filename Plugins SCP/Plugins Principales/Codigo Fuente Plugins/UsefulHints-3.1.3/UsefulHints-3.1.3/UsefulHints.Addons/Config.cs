using Exiled.API.Interfaces;
using System.ComponentModel;

namespace UsefulHintsAddons
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Descarga y aplica traducciones remotas.")]
        public bool EnableTranslations { get; set; } = true;

        [Description("Idiomas disponibles: pl, en, de, fr, cs, sk, es, it, pt, ru, tr, zh, ko.")]
        public string Language { get; set; } = "en";

        [Description("Comprueba si hay nuevas versiones en GitHub cuando comienza la espera de jugadores.")]
        public bool EnableAutoUpdate { get; set; } = true;

        [Description("Notifica únicamente cuando haya una versión nueva.")]
        public bool NotifyOnly { get; set; } = false;

        [Description("Crea un archivo .backup antes de sobrescribir.")]
        public bool EnableBackup { get; set; } = false;

        [Description("Muestra los registros de los complementos.")]
        public bool EnableLogging { get; set; } = true;

        [Description("Tras descargar una versión nueva, reinicia el servidor al terminar la ronda.")]
        public bool RestartNextRound { get; set; } = true;

        [Description("Activa la integración con RueI y sustituye las pantallas de UsefulHints basadas en ShowHint por pantallas basadas en RueI.")]
        public bool EnableRueIIntegration { get; set; } = false;
    }
}
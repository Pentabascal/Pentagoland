namespace SCPReplacer
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;

    /// <summary>
    /// The plugin's configuration.
    /// </summary>
    public class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Indica si el complemento está activado.")]
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        [Description("Indica si se muestran mensajes de depuración.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the number of seconds into the round an SCP can quit and still trigger a replacement.
        /// </summary>
        [Description("Tiempo máximo, en segundos desde el inicio de la ronda, durante el que la desconexión de un SCP puede abrir un sorteo de reemplazo.")]
        public int QuitCutoffSeconds { get; set; } = 60;

        /// <summary>
        /// Gets or sets the minimum health percentage (0-100) the SCP must have had to trigger a replacement.
        /// </summary>
        [Description("Porcentaje mínimo de salud (0-100) que debe conservar el SCP para que se active un sorteo de reemplazo.")]
        public int RequiredHealthPercent { get; set; } = 100;

        /// <summary>
        /// Gets or sets how many seconds players have to volunteer once the lottery opens.
        /// </summary>
        [Description("Número de segundos que tienen los jugadores para apuntarse una vez abierto el sorteo.")]
        public int LotteryPeriodSeconds { get; set; } = 15;

        /// <summary>
        /// Gets or sets a value indicating whether the .human command (an SCP voluntarily giving up their role) is enabled.
        /// </summary>
        [Description("Indica si está habilitado el comando .human/.no para renunciar al rol de SCP.")]
        public bool HumanForfeitEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to check for and automatically install updates.
        /// </summary>
        [Description("Indica si se buscan e instalan actualizaciones automáticamente.")]
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to keep a backup of the previous .dll before replacing it with an update.
        /// </summary>
        [Description("Indica si se conserva una copia de seguridad del archivo .dll anterior antes de reemplazarlo por una actualización.")]
        public bool AutoUpdateBackup { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to automatically restart the server once the current round ends, to apply a downloaded update.
        /// </summary>
        [Description("Indica si el servidor se reinicia automáticamente al terminar la ronda para aplicar una actualización descargada. Nunca se reinicia durante una ronda.")]
        public bool AutoUpdateRestart { get; set; } = true;
    }
}
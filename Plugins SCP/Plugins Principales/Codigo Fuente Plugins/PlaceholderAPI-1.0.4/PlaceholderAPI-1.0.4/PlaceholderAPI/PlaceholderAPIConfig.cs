namespace PlaceholderAPI
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Interfaces;
    using PlaceholderAPI.Configs;

    /// <summary>
    /// Main Config.
    /// </summary>
    public class PlaceholderAPIConfig : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether if to connect to the ECloud.
        /// </summary>
        [Description("Conecta con ECloud de PlaceholderAPI para descargar expansiones sin reiniciar.")]
        public bool ConnectToEcloud { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether if to download unsafe expansions from the ECloud.
        /// </summary>
        [Description("Permite descargar expansiones no verificadas de ECloud.")]
        public bool DownloadUnsafeFromEcloud { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether if Harmony config section.
        /// </summary>
        [Description("Opciones para modificar aspectos del juego.")]
        public HarmonyConfigSection Harmony { get; set; } = new ();
    }
}

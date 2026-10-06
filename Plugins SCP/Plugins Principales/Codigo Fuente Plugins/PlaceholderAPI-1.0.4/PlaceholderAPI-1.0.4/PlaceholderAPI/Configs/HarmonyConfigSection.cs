using System.Collections.Generic;
using System.ComponentModel;

namespace PlaceholderAPI.Configs
{

    /// <summary>
    /// Config section for harmony.
    /// </summary>
    public class HarmonyConfigSection
    {
        /// <summary>
        /// Gets or sets harmony support.
        /// </summary>
        [Description("Permite que el plugin modifique el juego para aplicar marcadores en todas partes.")]
        public bool isHarmonyEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets harmony support for hints.
        /// </summary>
        [Description("Permite activar o desactivar el parche de avisos.")]
        public bool Hints { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets harmony support for Broadcast.
        /// </summary>
        [Description("Permite activar o desactivar el parche de anuncios.")]
        public bool Broadcast { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets harmony support for Command Interpolation.
        /// </summary>
        [Description("Permite activar o desactivar el parche de interpolación de comandos.")]
        public bool CommandInterpolation { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets harmony support for Command.
        /// </summary>
        [Description("Permite activar o desactivar el parche de modificación de comandos.")]
        public bool Commands { get; set; } = true;
    }
}

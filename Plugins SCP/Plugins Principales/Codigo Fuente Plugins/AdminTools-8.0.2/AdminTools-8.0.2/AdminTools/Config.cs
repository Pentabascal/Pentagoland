using Exiled.API.Interfaces;
using System.ComponentModel;

namespace AdminTools
{
    public class Config : IConfig
    {
        [Description("Activa o desactiva AdminTools.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indica si se deben mostrar los registros de depuración.")]
        public bool Debug { get; set; } = false;

        [Description("Indica si el personal debe tener el modo Dios al cambiarse a Tutorial. Valor predeterminado: false.")]
        public bool GodTuts { get; set; } = false;

        [Description("Amplía el uso de comandos para seleccionar jugadores (por ejemplo, el comando de caramelos y otras funciones de AdminTools). Consulta el README en GitHub para obtener más información.")]
        public bool ExtendedCommandUsage { get; set; } = true;

        [Description("¿Liberar automáticamente a todos los jugadores encarcelados al reiniciarse la ronda? Valor predeterminado: false.")]
        public bool ClearJailsOnRestart { get; set; } = false;

        [Description("Indica si se deben desactivar RoundLock y LobbyLock mientras se espera a los jugadores.")]
        public bool DisableLockOnWaiting { get; set; } = false;

        [Description("Nombre del comando para encarcelar, útil si también usas cedmod u otro plugin que cause conflictos.")]
        public string JailCommandName { get; set; } = "jail";
    }
}
namespace XPSystem.Config.Events.Types.Custom
{
    using System.ComponentModel;

    /// <summary>
    /// XP Event Config limited dictionary file.
    /// Events that can only be achieved a limited number of times.
    /// </summary>
    /// <typeparam name="T">The type of the subkeys.</typeparam>
    public class XPECLimitedDictFile<T> : XPECDictFile<T>, IXPECLimitedFile
    {
        [Description("Indica si se permitirá obtener XP siempre. Si se activa, se ignorarán los demás límites.")]
        public bool AlwaysAllow { get; set; } = false;

        [Description("Indica si un evento con Amount establecido en 0 contará para el límite.")]
        public bool ZeroXPGainTriggers { get; set; } = false;

        [Description("Indica si el límite se aplica al archivo completo (por ejemplo, a todos los objetos recogidos) o a cada subclave (por ejemplo, a cada tipo de objeto).")]
        public bool LimitUnified { get; set; } = false;

        [Description("Número máximo de veces que se puede completar el evento por ronda. Un valor negativo indica que no hay límite.")]
        public int RoundLimit { get; set; } = -1;

        [Description("Número máximo de veces que se puede completar el evento por vida (hasta cambiar de rol). Un valor negativo indica que no hay límite.")]
        public int LifeLimit { get; set; } = -1;

        [Description("Tiempo mínimo que debe transcurrir para volver a completar el evento. Un valor negativo indica que no hay tiempo de espera.")]
        public float Cooldown { get; set; } = -1f;
    }
}
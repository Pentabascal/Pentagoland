namespace XPSystem.Config.Events.Types
{
    using System.ComponentModel;

    /// <summary>
    /// Represents a XP Event Config Item.
    /// </summary>
    public class XPECItem
    {
        [Description("Cantidad de XP que se concederá. Puede ser negativa.")]
        public int Amount { get; set; }

        [Description("Mensaje que se mostrará. Déjalo vacío para no mostrar ninguno.")]
        public string? Translation { get; set; }
    }
}
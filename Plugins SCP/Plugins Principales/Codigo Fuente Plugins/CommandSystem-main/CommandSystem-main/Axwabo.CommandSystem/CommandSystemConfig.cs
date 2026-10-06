#pragma warning disable CS1591
using System.ComponentModel;

namespace Axwabo.CommandSystem;

/// <summary>
/// Configuration for the Northwood Plugin API.
/// </summary>
[Serializable]
public sealed class CommandSystemConfig
{

    public bool Debug { get; set; }

    [Description("Indica si al copiar playerID se debe sustituir por el apodo.")]
    public bool CopyNicknameInsteadOfId { get; set; }

    [Description("Indica si se deben activar las extensiones de Remote Admin.")]
    public bool EnableRemoteAdminExtensions { get; set; } = true;

    [Description("Indica si se deben eliminar los desplazamientos IL del seguimiento de pila de las excepciones.")]
    public bool StripIntermediateLanguageOffsets { get; set; }

    [Description("Indica si los selectores personalizados pueden incluir al jugador anfitrión como objetivo.")]
    public bool AllowSelectingHost { get; set; }

}

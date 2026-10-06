namespace RemoteKeycard;

#if EXILED
using Exiled.API.Interfaces;
#endif  
using System.ComponentModel;

/// <inheritdoc />
#if EXILED
public class Config : IConfig
#else
public class Config
#endif
{
    /// <summary>
    ///    Gets or sets a value indicating whether this plugin is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether debug messages should be shown.
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    ///     Whether Amnesia affects the usage of keycards.
    /// </summary>
    [Description("Indica si Amnesia impide usar tarjetas de acceso.")]
    public bool AmnesiaMatters { get; set; } = true;

    /// <summary>
    ///     Whether this plugin works on generators.
    /// </summary>
    [Description("Indica si el plugin permite usar tarjetas de acceso en los generadores.")]
    public bool AffectGenerators { get; set; } = true;

    /// <summary>
    ///     Whether this plugin works on Warhead's panel.
    /// </summary>
    [Description("Indica si el plugin permite usar tarjetas de acceso en el panel de la Alpha Warhead.")]
    public bool AffectWarheadPanel { get; set; } = true;

    /// <summary>
    ///     Whether this plugin works on SCP lockers.
    /// </summary>
    [Description("Indica si el plugin permite usar tarjetas de acceso en las taquillas SCP.")]
    public bool AffectScpLockers { get; set; } = true;

    /// <summary>
    ///     Whether this plugin works on doors.
    /// </summary>
    [Description("Indica si el plugin permite usar tarjetas de acceso en las puertas.")]
    public bool AffectDoors { get; set; } = true;

    /// <summary>
    ///     Gets whether exceptions should be shown.
    /// </summary>
    [Description("Activa o desactiva la visualización de excepciones y errores en la consola. (Actívalo antes de informar de cualquier error).")]
    public bool ShowExceptions { get; set; } = false;
}
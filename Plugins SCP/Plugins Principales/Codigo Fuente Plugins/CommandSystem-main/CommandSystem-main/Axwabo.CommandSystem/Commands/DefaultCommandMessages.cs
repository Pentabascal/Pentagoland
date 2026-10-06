namespace Axwabo.CommandSystem.Commands;

/// <summary>Constants used by built-in command types.</summary>
public static class DefaultCommandMessages
{

    /// <summary>The message to display when only a player is allowed to use the command but the sender is not a player.</summary>
    public const string MustBePlayer = "Debes ser un jugador para usar este comando.";

    /// <summary>The message to display when a targeting command doesn't find any targets.</summary>
    public const string NoTargetsFound = "No se encontraron objetivos.";

    /// <summary>The message to display when a targeting command didn't affect any players.</summary>
    public const string NoPlayersAffected = "No se ha afectado a ningún jugador.";

}

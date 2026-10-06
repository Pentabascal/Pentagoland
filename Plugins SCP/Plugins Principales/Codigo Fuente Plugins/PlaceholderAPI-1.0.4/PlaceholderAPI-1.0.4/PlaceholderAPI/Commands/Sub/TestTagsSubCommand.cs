namespace PlaceholderAPI.Commands.Sub
{
    using System;
    using System.Linq;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlaceholderAPI.API;

    /// <summary>
    /// Parsing command for testing.
    /// </summary>
    public class TestTagsSubCommand : ICommand
    {
        /// <inheritdoc />
        public string Command { get; } = "parse";

        /// <inheritdoc />
        public string[] Aliases { get; } = ["test"];

        /// <inheritdoc />
        public string Description { get; } = "Comprueba los marcadores de PlaceholderAPI";

        /// <inheritdoc />
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("papi.ecloud.test"))
            {
                response = "[PAPI] No tienes permisos suficientes.";
                return false;
            }

            response = $"[PAPI] {PlaceholderAPI.SetPlaceholders(Player.Get(sender), string.Join(" ", arguments))}";
            return true;
        }
    }
}

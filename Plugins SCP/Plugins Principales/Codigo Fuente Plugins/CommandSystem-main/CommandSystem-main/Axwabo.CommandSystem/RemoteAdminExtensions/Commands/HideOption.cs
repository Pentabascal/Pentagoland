using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Attributes.Containers;
using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.RemoteAdminExtensions.Commands;

[CommandProperties("hide", 1, "Oculta la opción indicada en tu interfaz de Remote Admin.")]
[SubcommandOfContainer(typeof(OptionPreferencesContainer))]
[Usage("<id>")]
internal sealed class HideOption : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
        => !RemoteAdminOptionManager.TryGetOption(arguments.At(0), out var option)
            ? "!No se encontró ninguna opción con ese ID."
            : OptionPreferencesContainer.SetVisibility(sender.SenderId, option, false)
                ? "La opción ya no está visible para ti."
                : "!La opción ya está oculta para ti.";

}

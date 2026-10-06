using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Attributes.Containers;
using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.RemoteAdminExtensions.Commands;

[CommandProperties("show", 1, "Muestra la opción indicada para ti.")]
[SubcommandOfContainer(typeof(OptionPreferencesContainer))]
[Usage("<id>")]
internal sealed class ShowOption : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
        => !RemoteAdminOptionManager.TryGetOption(arguments.At(0), out var option)
            ? "!No se encontró ninguna opción con ese ID."
            : OptionPreferencesContainer.SetVisibility(sender.SenderId, option, true)
                ? "La opción ya está visible para ti."
                : "!La opción ya está visible para ti.";

}

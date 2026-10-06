using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Attributes.Containers;
using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.RemoteAdminExtensions.Commands;

[CommandProperties("hideAll", "Oculta todas las opciones de Remote Admin disponibles para ti.")]
[SubcommandOfContainer(typeof(OptionPreferencesContainer))]
internal sealed class HideAll : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        var nowHidden = 0;
        foreach (var option in RemoteAdminOptionManager.AllOptions)
            if (option.Permissions.CheckSafe(sender) && OptionPreferencesContainer.SetVisibility(sender.SenderId, option, false))
                nowHidden++;
        return nowHidden == 0
            ? "!Todas las opciones ya están ocultas."
            : $"{nowHidden} {(nowHidden == 1 ? "opción" : "opciones")} se han ocultado para ti.";
    }

}

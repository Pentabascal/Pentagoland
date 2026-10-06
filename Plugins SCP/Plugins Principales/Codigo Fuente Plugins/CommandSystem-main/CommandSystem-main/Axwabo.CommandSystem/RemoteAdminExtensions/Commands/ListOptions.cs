using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Attributes.Containers;
using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.RemoteAdminExtensions.Commands;

[CommandProperties("list", "Muestra todas las opciones de Remote Admin disponibles para ti.")]
[SubcommandOfContainer(typeof(OptionPreferencesContainer))]
internal sealed class ListOptions : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        var sb = StringBuilderPool.Shared.Rent("Opciones de Remote Admin disponibles:").AppendLine();
        if (RemoteAdminOptionManager.AppendAllOptions(sender, sb, false, false))
            return CommandResult.Succeeded(StringBuilderPool.Shared.ToStringReturn(sb).TrimEnd());
        StringBuilderPool.Shared.Return(sb);
        return "!No tienes opciones disponibles.";
    }

}

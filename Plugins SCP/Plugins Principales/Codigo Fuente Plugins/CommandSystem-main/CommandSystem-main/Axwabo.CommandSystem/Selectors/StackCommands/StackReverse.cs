using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Commands;

namespace Axwabo.CommandSystem.Selectors.StackCommands;

[CommandProperties(CommandHandlerType.RaAndServer, "stackReverse", "Invierte el orden de los jugadores en la pila de selección.")]
[Aliases("sReverse", "sRev")]
internal sealed class StackReverse : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        if (!PlayerSelectionStack.PreprocessCommand(sender, out var selection, out var result))
            return result;
        selection.Reverse();
        return "Se ha invertido el orden de la pila de selección.";
    }

}

using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Commands;
using Axwabo.CommandSystem.Extensions;

namespace Axwabo.CommandSystem.Selectors.StackCommands;

[CommandProperties(CommandHandlerType.RaAndServer, "stackClear", "Vacía la pila de selección de jugadores.")]
[Aliases("sClear", "sClr", "scl")]
internal sealed class StackClear : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        if (!PlayerSelectionStack.PreprocessCommand(sender, out var selection, out var result, true))
            return result;
        if (selection.IsEmpty)
            return "La pila de selección ya está vacía.";
        var count = selection.Count;
        selection.Clear();
        return count == 1 ? "Se ha vaciado la pila de selección (se ha eliminado 1 elemento)." : $"Se ha vaciado la pila de selección (se han eliminado {count} elementos).";
    }

}

using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Commands;
using Axwabo.CommandSystem.Extensions;

namespace Axwabo.CommandSystem.Selectors.StackCommands;

[CommandProperties(CommandHandlerType.RaAndServer, "stackPop", "Retira los jugadores de la parte superior de la pila de selección.", "spop")]
[Usage("", "<index>")]
internal sealed class StackPop : CommandBase
{

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
    {
        if (!PlayerSelectionStack.PreprocessCommand(sender, out var selection, out var result))
            return result;
        var indexSet = false;
        HubCollection popped;
        var index = 0;
        if (arguments.Count > 0 && int.TryParse(arguments.At(0), out index))
        {
            popped = selection.PopAt(index);
            indexSet = true;
        }
        else
            popped = selection.Pop();

        return $"{(popped.Count == 1 ? "Se ha retirado 1 jugador" : $"Se han retirado {popped.Count} jugadores")}{(indexSet ? $" en el índice {index}" : "")} de la pila de selección:\n{popped.CombineNicknames()}";
    }

}

using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Commands;
using Axwabo.CommandSystem.Extensions;

namespace Axwabo.CommandSystem.Selectors.StackCommands;

[CommandProperties(CommandHandlerType.RaAndServer, "stackPush", "Añade los jugadores indicados a la pila de selección.")]
[Aliases("sPush")]
internal sealed class StackPush : UnifiedTargetingCommand
{

    protected override CommandResult ExecuteOnTargets(List<ReferenceHub> targets, ArraySegment<string> arguments, CommandSender sender)
    {
        if (!PlayerSelectionStack.PreprocessCommand(sender, out var selection, out var result, true))
            return result;
        if (selection.CheckOverflow(1))
            return $"!Ya se ha alcanzado el límite de listas de la pila de selección: {PlayerSelectionStack.MaxSize}.";
        selection.Push(targets);
        return $"{(targets.Count == 1 ? "Se ha añadido 1 jugador" : $"Se han añadido {targets.Count} jugadores")} a la pila de selección:\n{targets.CombineNicknames()}";
    }

}

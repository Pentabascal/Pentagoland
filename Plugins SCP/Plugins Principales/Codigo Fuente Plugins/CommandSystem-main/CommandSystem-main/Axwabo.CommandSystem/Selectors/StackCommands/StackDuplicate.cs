using Axwabo.CommandSystem.Attributes;
using Axwabo.CommandSystem.Commands;
using Axwabo.CommandSystem.Commands.Interfaces;
using Axwabo.CommandSystem.Extensions;

namespace Axwabo.CommandSystem.Selectors.StackCommands;

[CommandProperties(CommandHandlerType.RaAndServer, "stackDuplicate", 1, "Duplica las selecciones indicadas en la pila de selección.")]
[Aliases("stackDup", "sDup")]
[Usage("", "all/*", "first/f/top/t", $"índices separados por espacios o por uno de{Separators}")]
internal sealed class StackDuplicate : CommandBase, INotEnoughArgumentsHandler
{

    private const string Separators = " .,;_+-";

    protected override CommandResult Execute(ArraySegment<string> arguments, CommandSender sender)
        => !PlayerSelectionStack.PreprocessCommand(sender, out var selection, out var result)
            ? result
            : arguments.At(0).ToLower() switch
            {
                "all" or "*" => DuplicateAll(selection),
                "first" or "f" or "top" or "t" => DuplicateFirst(selection),
                _ => DuplicateSpecific(selection, arguments)
            };

    public CommandResult? OnNotEnoughArgumentsProvided(ArraySegment<string> arguments, CommandSender sender, int required)
        => !PlayerSelectionStack.PreprocessCommand(sender, out var selection, out var result) ? result : DuplicateFirst(selection);

    private static CommandResult DuplicateFirst(PlayerSelectionStack selection)
    {
        if (selection.CheckOverflow(1))
            return $"!Ya se ha alcanzado el límite de listas de la pila de selección: {PlayerSelectionStack.MaxSize}.";
        var dup = selection.Peek();
        selection.Push(dup);
        return $"Se ha duplicado la primera selección de la pila de selección:\n{dup.CombineNicknames()}";
    }

    private static CommandResult DuplicateAll(PlayerSelectionStack selection)
    {
        if (selection.CheckOverflow(selection.Count))
            return $"!Duplicar todas las selecciones ({selection.Count}) superaría el límite máximo de {PlayerSelectionStack.MaxSize} listas de la pila de selección.";
        selection.DuplicateAll();
        return "Se han duplicado todas las selecciones de la pila.";
    }

    private static CommandResult DuplicateSpecific(PlayerSelectionStack selection, ArraySegment<string> arguments)
    {
        var split = arguments.Join().Split(Separators.ToCharArray());
        var push = ListPool<HubCollection>.Shared.Rent(split.Length);
        try
        {
            foreach (var s in split)
                if (int.TryParse(s, out var index) && index >= 0 && index < selection.Count)
                    push.Add(selection.PeekAt(index));
            if (push.Count == 0)
                return "!No se han indicado índices válidos.";
            if (selection.CheckOverflow(push.Count))
                return $"!Duplicar {push.Count} listas superaría el límite máximo de {PlayerSelectionStack.MaxSize} listas de la pila de selección.";
            foreach (var collection in push)
                selection.Push(collection);
            return push.Count == 1 ? "Se ha duplicado 1 selección en la pila de selección." : $"Se han duplicado {push.Count} selecciones en la pila de selección.";
        }
        finally
        {
            ListPool<HubCollection>.Shared.Return(push);
        }
    }

}

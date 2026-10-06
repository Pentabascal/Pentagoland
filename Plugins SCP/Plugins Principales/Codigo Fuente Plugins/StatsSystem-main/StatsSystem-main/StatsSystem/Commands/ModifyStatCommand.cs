using System;
using System.Collections.Generic;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using Utils;

namespace StatsSystem.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class AddStatCommand : ModifyStatBase
{
    public override string Command => "addstat";
    public override string[] Aliases => [];
    public override string Description => "Aumenta una estadística de un jugador. Uso: addstat <player> <statKey> <amount>";

    protected override long GetDelta(long amount)
    {
        return amount;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class RemoveStatCommand : ModifyStatBase
{
    public override string Command => "removestat";
    public override string[] Aliases => [];
    public override string Description => "Reduce una estadística de un jugador. Uso: removestat <player> <statKey> <amount>";

    protected override long GetDelta(long amount)
    {
        return -amount;
    }
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class SetStatCommand : ICommand, IUsageProvider
{
    public string Command => "setstat";
    public string[] Aliases => [];
    public string Description => "Establece el valor exacto de una estadística de un jugador. Uso: setstat <player> <statKey> <value>";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!ModifyStatHelper.CheckPermission(sender, out response)) return false;
        if (arguments.Count < 3)
        {
            response = $"Uso: {Command} {string.Join(" ", Usage)}";
            return false;
        }

        var hubs = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var remaining);
        var hasOnline = hubs is { Count: > 0 };

        var statKey = hasOnline ? remaining?[0] ?? string.Empty : arguments.At(1);
        var valueRaw = hasOnline ? remaining?[1] ?? string.Empty : arguments.At(2);

        if (string.IsNullOrEmpty(statKey) || string.IsNullOrEmpty(valueRaw))
        {
            response = $"Uso: {Command} {string.Join(" ", Usage)}";
            return false;
        }

        if (!long.TryParse(valueRaw, out var value))
        {
            response = "El valor debe ser un número entero.";
            return false;
        }

        if (hasOnline)
        {
            response = ModifyStatHelper.ApplyToOnline(hubs, p =>
            {
                if (!StatsSystemPlugin.Stats.TryGetOrCreateStats(p, out var s)) return null;
                var old = s.GetCounter(statKey);
                s.SetCounter(statKey, value);
                return $"{p.Nickname}: '{statKey}' establecido en {value} (antes: {old})";
            });
            return response != "No se ha aplicado ningún cambio a los jugadores.";
        }

        var query = arguments.At(0);
        if (!StatsSystemPlugin.Stats.TryGetOrCreateStats(query, out var stats))
        {
            response = $"No se han podido encontrar ni crear estadísticas para '{query}'.";
            return false;
        }

        var oldVal = stats.GetCounter(statKey);
        stats.SetCounter(statKey, value);
        response = $"{query}: '{statKey}' establecido en {value} (antes: {oldVal})";
        return true;
    }

    public string[] Usage => ["%player%", "<statKey>", "<value>"];
}

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class DeleteStatCommand : ICommand, IUsageProvider
{
    public string Command => "deletestat";
    public string[] Aliases => [];

    public string Description =>
        "Elimina por completo una clave de estadística de los datos de un jugador. Uso: deletestat <userId> <statKey>";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!ModifyStatHelper.CheckPermission(sender, out response)) return false;
        if (arguments.Count < 2)
        {
            response = $"Uso: {Command} {string.Join(" ", Usage)}";
            return false;
        }

        var userId = arguments.At(0);
        var statKey = arguments.At(1);

        if (!StatsSystemPlugin.Stats.DeleteStatKey(userId, statKey))
        {
            response = $"No se encontró la clave de estadística '{statKey}' para '{userId}'.";
            return false;
        }

        response = $"Se ha eliminado la estadística '{statKey}' de '{userId}'.";
        return true;
    }

    public string[] Usage => ["<userId>", "<statKey>"];
}

public abstract class ModifyStatBase : ICommand, IUsageProvider
{
    public abstract string Command { get; }
    public abstract string[] Aliases { get; }
    public abstract string Description { get; }

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!ModifyStatHelper.CheckPermission(sender, out response)) return false;
        if (arguments.Count < 3)
        {
            response = $"Uso: {Command} {string.Join(" ", Usage)}";
            return false;
        }

        var hubs = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out var remaining);
        var hasOnline = hubs is { Count: > 0 };

        var statKey = hasOnline ? remaining?[0] ?? string.Empty : arguments.At(1);
        var amountRaw = hasOnline ? remaining?[1] ?? string.Empty : arguments.At(2);

        if (string.IsNullOrEmpty(statKey) || string.IsNullOrEmpty(amountRaw))
        {
            response = $"Uso: {Command} {string.Join(" ", Usage)}";
            return false;
        }

        if (!long.TryParse(amountRaw, out var amount) || amount < 0)
        {
            response = "La cantidad debe ser un entero no negativo.";
            return false;
        }

        var delta = GetDelta(amount);
        var action = delta >= 0 ? "aumentó" : "disminuyó";

        if (hasOnline)
        {
            response = ModifyStatHelper.ApplyToOnline(hubs, p =>
            {
                if (!StatsSystemPlugin.Stats.TryGetOrCreateStats(p, out var s)) return null;
                var old = s.GetCounter(statKey);
                s.IncrementCounter(statKey, delta);
                return $"{p.Nickname}: '{statKey}' {action} de {old} a {old + delta}";
            });
            return response != "No se ha aplicado ningún cambio a los jugadores.";
        }

        var query = arguments.At(0);
        if (!StatsSystemPlugin.Stats.TryGetOrCreateStats(query, out var stats))
        {
            response = $"No se ha encontrado al jugador '{query}' y no hay estadísticas guardadas para ese identificador.";
            return false;
        }

        var oldVal = stats.GetCounter(statKey);
        stats.IncrementCounter(statKey, delta);
        response = $"{query}: '{statKey}' {action} de {oldVal} a {oldVal + delta}";
        return true;
    }

    public string[] Usage => ["%player%", "<statKey>", "<amount>"];

    protected abstract long GetDelta(long amount);
}

internal static class ModifyStatHelper
{
    internal static bool CheckPermission(ICommandSender sender, out string response)
    {
        var issuer = Player.Get(sender);
        if (issuer == null)
        {
            response = "Este comando solo puede usarse dentro del juego.";
            return false;
        }

        if (!issuer.HasPermissions("stat.manage"))
        {
            response = "Falta el permiso: stat.manage";
            return false;
        }

        response = null;
        return true;
    }

    internal static string ApplyToOnline(List<ReferenceHub> hubs, Func<Player, string> action)
    {
        var sb = StringBuilderPool.Shared.Rent();
        var affected = 0;
        foreach (var hub in hubs)
        {
            var p = Player.Get(hub);
            if (p == null) continue;
            var line = action(p);
            if (line == null) continue;
            if (affected > 0) sb.Append('\n');
            sb.Append(line);
            affected++;
        }

        var result = StringBuilderPool.Shared.ToStringReturn(sb);
        return affected > 0 ? result : "No se ha aplicado ningún cambio a los jugadores.";
    }
}
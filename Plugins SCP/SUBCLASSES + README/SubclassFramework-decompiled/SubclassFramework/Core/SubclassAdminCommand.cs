using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using PlayerRoles;

namespace SubclassFramework.Core;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public sealed class SubclassAdminCommand : ICommand
{
	private const string Permission = "subclass.spawn";

	public string Command => "subclass";

	public string[] Aliases => new string[2] { "subclasses", "scspawn" };

	public string Description => "Administra las subclases del plugin.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.CheckPermission("subclass.spawn"))
		{
			response = "No tienes permiso. Requiere: subclass.spawn";
			return false;
		}
		if (arguments.Count == 0)
		{
			response = "Uso: subclass spawn <jugador|id> <subclase-id> | subclass list";
			return false;
		}
		string text = arguments.At(0).ToLowerInvariant();
		if (text == "list")
		{
			response = string.Join(", ", SubclassManager.Instance?.Registry.Select((Subclass sc) => sc.Id) ?? Enumerable.Empty<string>());
			return true;
		}
		if (text != "spawn" || arguments.Count < 3)
		{
			response = "Uso: subclass spawn <jugador|id> <subclase-id> | subclass list";
			return false;
		}
		if (SubclassManager.Instance == null)
		{
			response = "SubclassFramework no está activo.";
			return false;
		}
		Player target = FindPlayer(arguments.At(1));
		if (target == null)
		{
			response = "No se encontro al jugador '" + arguments.At(1) + "'.";
			return false;
		}
		string requestedId = NormalizeId(arguments.At(2));
		Subclass subclass = SubclassManager.Instance.Registry.FirstOrDefault((Subclass sc) => string.Equals(sc.Id, requestedId, StringComparison.OrdinalIgnoreCase));
		if (subclass == null)
		{
			response = "No existe la subclase '" + arguments.At(2) + "'. Usa 'subclass list'.";
			return false;
		}
		if (subclass.Roles == null || subclass.Roles.Length == 0)
		{
			response = "La subclase no tiene ningún rol compatible.";
			return false;
		}
		try
		{
			RoleTypeId role = subclass.Roles[0];
			SubclassManager.Instance.SuppressNextAssign(target);
			target.Role.Set(role, SpawnReason.ForceClass, RoleSpawnFlags.None);
			Timing.CallDelayed(0.8f, () =>
			{
				if (target != null && target.IsAlive && target.Role.Type == role)
				{
					SubclassManager.Instance?.ForceAssign(target, subclass.Id);
				}
			});
			response = $"Se ha solicitado {subclass.Name} ({subclass.Id}) para {target.Nickname}. Rol: {role}.";
			return true;
		}
		catch (Exception ex)
		{
			SubclassManager.Instance.CancelSuppressNextAssign(target);
			response = "No se pudo cambiar el rol: " + ex.Message;
			return false;
		}
	}

	private static string NormalizeId(string raw)
	{
		string text = raw.Trim().ToLowerInvariant();
		if (text.StartsWith("chaos_", StringComparison.Ordinal))
		{
			text = text.Substring("chaos_".Length);
		}
		return text;
	}

	private static Player FindPlayer(string token)
	{
		if (int.TryParse(token, out var result))
		{
			return Player.Get(result);
		}
		List<Player> list = Player.List.Where((Player player) => player != null && (string.Equals(player.Nickname, token, StringComparison.OrdinalIgnoreCase) || string.Equals(player.UserId, token, StringComparison.OrdinalIgnoreCase))).ToList();
		if (list.Count == 1)
		{
			return list[0];
		}
		List<Player> list2 = Player.List.Where((Player player) => player != null && player.Nickname.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0).Take(2).ToList();
		return (list2.Count == 1) ? list2[0] : null;
	}
}

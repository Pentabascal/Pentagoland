using System;
using CommandSystem;
using Exiled.API.Features;
using UnityEngine;

namespace SubclassFramework.Core;

[CommandHandler(typeof(ClientCommandHandler))]
public sealed class SubclassInfoCommand : ICommand
{
	public string Command => "subclase";

	public string[] Aliases => new string[2] { "subclass", "miclase" };

	public string Description => "Muestra la subclase que te ha tocado y su habilidad.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player player = Player.Get(sender);
		Subclass subclass = SubclassManager.Instance?.GetSubclass(player);
		if (subclass == null)
		{
			response = "No tienes ninguna subclase esta ronda.";
			return true;
		}
		float f = SubclassManager.Instance.RemainingCooldown(player);
		string text = (subclass.HasActiveAbility ? $"\nHabilidad activa | Enfriamiento: {Mathf.CeilToInt(f)}s de {Mathf.RoundToInt(SubclassManager.Instance.CooldownOf(subclass))}s" : "\nSin habilidad activa (pasiva).");
		response = subclass.Name + "\n" + subclass.Description + text;
		return true;
	}
}

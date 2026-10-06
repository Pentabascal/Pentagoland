using System;
using CommandSystem;
using Exiled.API.Features;

namespace SubclassFramework.Core;

[CommandHandler(typeof(ClientCommandHandler))]
public sealed class AbilityCommand : ICommand
{
	public string Command => "hab";

	public string[] Aliases => new string[3] { "habilidad", "ability", "skill" };

	public string Description => "Usa la habilidad de tu subclase.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player player = Player.Get(sender);
		if (player == null)
		{
			response = "Solo se puede usar en partida.";
			return false;
		}
		if (SubclassManager.Instance == null)
		{
			response = "El plugin de subclases no está activo.";
			return false;
		}
		bool flag = SubclassManager.Instance.TryUseAbility(player);
		response = (flag ? "Habilidad activada." : "No se pudo activar la habilidad; revisa el aviso del juego.");
		return flag;
	}
}

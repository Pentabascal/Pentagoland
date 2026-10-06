using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

namespace SubclassFramework.Core;

[CommandHandler(typeof(ClientCommandHandler))]
public sealed class PlayerPreferencesCommand : ICommand
{
	public string Command => "preferencias";

	public string[] Aliases => new string[2] { "prefs", "ajustes" };

	public string Description => "Personaliza los avisos de subclases que ves.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		Player player = Player.Get(sender);
		if (player == null)
		{
			response = "Solo se puede usar en partida.";
			return false;
		}
		switch ((arguments.Count <= 0) ? string.Empty : arguments.FirstOrDefault()?.ToLowerInvariant())
		{
		case "intro":
		case "introduccion":
			response = (SfUtils.ToggleSubclassIntro(player) ? "Aviso de subclase activado." : "Aviso de subclase ocultado.");
			return true;
		case "habilidad":
		case "hud":
			response = (SfUtils.ToggleAbilityHud(player) ? "Contador de habilidad activado." : "Contador de habilidad ocultado.");
			return true;
		default:
			response = ".preferencias intro - Mostrar u ocultar el aviso al aparecer.\n.preferencias habilidad - Mostrar u ocultar el contador de habilidad.";
			return true;
		}
	}
}

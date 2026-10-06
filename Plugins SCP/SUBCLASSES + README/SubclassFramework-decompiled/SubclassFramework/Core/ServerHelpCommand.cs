using System;
using CommandSystem;

namespace SubclassFramework.Core;

[CommandHandler(typeof(ClientCommandHandler))]
public sealed class ServerHelpCommand : ICommand
{
	public string Command => "ayuda";

	public string[] Aliases => new string[2] { "comandos", "commands" };

	public string Description => "Muestra las funciones y comandos disponibles del servidor.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		response = "Funciones del servidor:\n.subclase - Ver tu subclase, habilidad y cooldown.\n.subclases - Ver las subclases registradas y roles compatibles.\n.hab - Usar la habilidad activa de tu subclase.\n.preferencias - Personalizar avisos de subclases.\n.objetosavisos - Personalizar avisos de objetos personalizados.\n.ayuda - Mostrar esta ayuda.\nUsa .help para consultar también los comandos estándar del juego.";
		return true;
	}
}

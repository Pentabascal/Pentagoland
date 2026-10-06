using System;
using System.Linq;
using CommandSystem;
using PlayerRoles;

namespace SubclassFramework.Core;

[CommandHandler(typeof(ClientCommandHandler))]
public sealed class SubclassListCommand : ICommand
{
	public string Command => "subclases";

	public string[] Aliases => new string[2] { "subclasses", "clases" };

	public string Description => "Muestra las subclases disponibles y sus roles compatibles.";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		SubclassManager instance = SubclassManager.Instance;
		if (instance == null)
		{
			response = "El sistema de subclases no está activo.";
			return false;
		}
		string[] array = (from sc in instance.Registry.Where((Subclass sc) => sc != null).OrderBy((Subclass sc) => sc.Name, StringComparer.OrdinalIgnoreCase)
			select sc.Name + " [" + string.Join("/", sc.Roles ?? new RoleTypeId[0]) + "]").ToArray();
		response = ((array.Length == 0) ? "No hay subclases registradas." : string.Format("Subclases disponibles ({0}):\n- {1}", array.Length, string.Join("\n- ", array)));
		return true;
	}
}

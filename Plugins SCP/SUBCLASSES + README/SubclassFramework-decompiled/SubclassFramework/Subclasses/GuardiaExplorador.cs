using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class GuardiaExplorador : Subclass
{
	public override string Id => "guardia_explorador";

	public override string Name => "Guardia Explorador";

	public override string Description => "Hace un barrido corto para detectar si hay un SCP cerca y mostrar su zona.";

	public override RoleTypeId[] Roles => FacilitySubclasses.Guard;

	public override float Chance => 10f;

	public override int MaxPerRound => 2;

	public override float Cooldown => 90f;

	public override string FailMessage => "No hay ningún SCP detectable en el radio del barrido.";

	public override bool UseAbility(Player player)
	{
		Player player2 = (from p in Player.List
			where p != null && p.IsAlive && p.IsScp && (p.Position - player.Position).sqrMagnitude <= 1225f
			orderby (p.Position - player.Position).sqrMagnitude
			select p).FirstOrDefault();
		if (player2 == null)
		{
			return false;
		}
		SfUtils.Hint(player, "<color=#6cf><b>Barrido completado:</b> se ha detectado presencia SCP en " + SfUtils.RoomName(player2) + ".</color>", 6f);
		return true;
	}
}

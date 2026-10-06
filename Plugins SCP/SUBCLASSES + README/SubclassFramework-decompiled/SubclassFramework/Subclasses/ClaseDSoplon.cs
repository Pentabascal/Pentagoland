using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDSoplon : Subclass
{
	public override string Id => "clased_soplon";

	public override string Name => "Soplón";

	public override string Description => "Has oído rumores en las celdas y puedes intuir en qué zona está el SCP más cercano.";

	public override RoleTypeId[] Roles => ExtraSubclasses.ClassD;

	public override float Chance => 10f;

	public override int MaxPerRound => 2;

	public override float Cooldown => 50f;

	public override string FailMessage => "No notas ninguna presencia anómala.";

	public override bool UseAbility(Player player)
	{
		if (player == null || !player.IsAlive)
		{
			return false;
		}
		Player player2 = null;
		float num = float.MaxValue;
		foreach (Player item in Player.List)
		{
			if (!(item == null) && item.IsAlive && item.IsScp)
			{
				float sqrMagnitude = (item.Position - player.Position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					player2 = item;
				}
			}
		}
		if (player2 == null)
		{
			return false;
		}
		float num2 = Mathf.Sqrt(num);
		string text;
		if (num2 < 20f)
		{
			text = "<color=#FF4040>MUY CERCA</color>";
		}
		else
		{
			text = ((num2 < 50f) ? "<color=#FF8000>cerca</color>" : "<color=#80FF80>lejos</color>");
		}
		SfUtils.Hint(player, "<size=20><color=#FF8000><b>INSTINTO</b></color></size>\n<size=18>Algo anda por <b>" + Zona(player2.Zone) + "</b>\n" + text + "</size>", 6f);
		return true;
	}

	private static string Zona(ZoneType zona)
	{
		return zona switch
		{
			ZoneType.LightContainment => "Contención Ligera", 
			ZoneType.HeavyContainment => "Contención Pesada", 
			ZoneType.Entrance => "la Zona de Entrada", 
			ZoneType.Surface => "la Superficie", 
			ZoneType.Pocket => "la Dimension de Bolsillo", 
			_ => "algun sitio que no reconoces", 
		};
	}
}

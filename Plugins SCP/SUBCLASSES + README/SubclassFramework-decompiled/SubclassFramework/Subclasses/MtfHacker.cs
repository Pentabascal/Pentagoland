using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class MtfHacker : Subclass
{
	public override string Id => "mtf_hacker";

	public override string Name => "MTF Hacker";

	public override string Description => "Adelanta 25 segundos la activación de cada generador una sola vez por ronda y gana velocidad por cada generador activo.";

	public override RoleTypeId[] Roles => MtfSubclasses.Privates;

	public override float Chance => 10f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunCrossvec,
		ItemType.KeycardFacilityManager,
		ItemType.Radio,
		ItemType.Medkit,
		ItemType.ArmorLight
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato9, 120) };

	public override void OnActivatingGenerator(Player player, ActivatingGeneratorEventArgs ev)
	{
		if (ev?.IsAllowed == true)
		{
			HackerMechanics.ScheduleGeneratorAcceleration(player, ev.Generator);
		}
	}

	public override void OnTick(Player player)
	{
		HackerMechanics.Tick(player);
	}

	public override void OnRemoved(Player player)
	{
		HackerMechanics.Remove(player);
	}
}

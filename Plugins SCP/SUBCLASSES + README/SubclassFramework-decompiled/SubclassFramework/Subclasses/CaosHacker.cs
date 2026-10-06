using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class CaosHacker : Subclass
{
	public override string Id => "caos_hacker";

	public override string Name => "Caos Hacker";

	public override string Description => "Adelanta 25 segundos la activación de cada generador una sola vez por ronda y gana velocidad por cada generador activo.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 8f;

	public override bool OverrideInventory => true;

	public override ItemType[] Items => new ItemType[5]
	{
		ItemType.GunAK,
		ItemType.KeycardChaosInsurgency,
		ItemType.Radio,
		ItemType.Medkit,
		ItemType.ArmorCombat
	};

	public override KeyValuePair<AmmoType, ushort>[] Ammo => new KeyValuePair<AmmoType, ushort>[1] { Subclass.A(AmmoType.Nato762, 90) };

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

using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Features;
using Exiled.Events.Handlers;
using MEC;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class ZombiInfeccioso : Subclass
{
	public override string Id => "zombi_infeccioso";

	public override string Name => "Zombi Infeccioso";

	public override string Description => "Tiene 280 de vida máxima. Cada muerte tiene un 50% de probabilidad de levantar a la víctima como otro zombi infeccioso.";

	public override RoleTypeId[] Roles => ScpSubclasses.Scp0492;

	public override float Chance => 6f;

	public override int MaxHealth => 280;

	public override void OnKill(Exiled.API.Features.Player killer, Exiled.API.Features.Player victim, DiedEventArgs ev)
	{
		if (victim == null || !victim.IsHuman || !SfUtils.Roll(50f))
		{
			return;
		}
		SubclassManager manager = SubclassManager.Instance;
		if (manager == null)
		{
			return;
		}
		bool spawnHandled = false;
		Vector3 where = victim.Position;
		try
		{
			Ragdoll ragdoll = Ragdoll.List.FirstOrDefault((Ragdoll r) => r != null && r.Owner == victim);
			if (ragdoll != null)
			{
				where = ragdoll.Position;
			}
		}
		catch (Exception ex)
		{
			Log.Debug("[SubclassFramework] Zombi Infeccioso (ragdoll): " + ex.Message);
		}
		Timing.CallDelayed(0.5f, () =>
		{
			try
			{
				if (!(victim == null) && victim.IsConnected && victim.Role.Type == RoleTypeId.Spectator)
				{
					Ragdoll ragdoll2 = Ragdoll.List.FirstOrDefault((Ragdoll r) => r != null && r.Owner == victim);
					if (ragdoll2 != null)
					{
						where = ragdoll2.Position;
					}
					manager.SuppressNextAssign(victim);
					Exiled.Events.Handlers.Player.Spawned += new CustomEventHandler<SpawnedEventArgs>(OnZombieSpawned);
					victim.Role.Set(RoleTypeId.Scp0492, SpawnReason.ForceClass, RoleSpawnFlags.None);
					Timing.CallDelayed(5f, () =>
					{
						Exiled.Events.Handlers.Player.Spawned -= new CustomEventHandler<SpawnedEventArgs>(OnZombieSpawned);
						if (!spawnHandled)
						{
							manager.CancelSuppressNextAssign(victim);
						}
					});
				}
			}
			catch (Exception ex2)
			{
				manager.CancelSuppressNextAssign(victim);
				Exiled.Events.Handlers.Player.Spawned -= new CustomEventHandler<SpawnedEventArgs>(OnZombieSpawned);
				Log.Debug("[SubclassFramework] Zombi Infeccioso: " + ex2.Message);
			}
		});
		SfUtils.Hint(killer, "<color=#8f8><b>Infección propagada.</b></color>", 4f);
		void OnZombieSpawned(SpawnedEventArgs spawnEv)
		{
			if (!(spawnEv?.Player != victim))
			{
				Exiled.Events.Handlers.Player.Spawned -= new CustomEventHandler<SpawnedEventArgs>(OnZombieSpawned);
				if (!(victim == null) && victim.IsAlive)
				{
					spawnHandled = true;
					victim.Position = where;
					if (manager.ForceAssign(victim, "zombi_infeccioso"))
					{
						SfUtils.Hint(victim, "<size=22><color=#8f8><b>La infección te reclama.</b></color></size>");
					}
				}
			}
		}
	}
}

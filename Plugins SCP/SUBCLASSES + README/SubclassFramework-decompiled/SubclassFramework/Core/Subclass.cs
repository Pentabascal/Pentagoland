using System;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace SubclassFramework.Core;

public abstract class Subclass
{
	private static readonly ItemType[] NoItems = new ItemType[0];

	private static readonly KeyValuePair<AmmoType, ushort>[] NoAmmo = new KeyValuePair<AmmoType, ushort>[0];

	public abstract string Id { get; }

	public abstract string Name { get; }

	public abstract string Description { get; }

	public abstract RoleTypeId[] Roles { get; }

	public virtual Tier AssignTier => Tier.Specific;

	public virtual float Chance => 10f;

	public virtual int MaxPerRound => -1;

	public virtual int MaxPerWave => -1;

	public virtual float MinRoundTime => 0f;

	public virtual int MaxHealth => -1;

	public virtual bool OverrideInventory => false;

	public virtual ItemType[] Items => NoItems;

	public virtual ItemType[] ExtraItems => NoItems;

	public virtual KeyValuePair<AmmoType, ushort>[] Ammo => NoAmmo;

	public virtual float SpeedMultiplier => 1f;

	public virtual float SpeedDelay => 0f;

	public virtual float Scale => 1f;

	public virtual float Cooldown => 0f;

	public bool HasActiveAbility => Cooldown > 0f;

	public virtual string FailMessage => "No puedes usar la habilidad ahora mismo.";

	public virtual bool UseAbility(Player player)
	{
		return false;
	}

	public virtual void OnAssigned(Player player)
	{
	}

	public virtual void OnRemoved(Player player)
	{
	}

	public virtual void OnTick(Player player)
	{
	}

	public virtual void OnDealingDamage(Player attacker, HurtingEventArgs ev)
	{
	}

	public virtual void OnTakingDamage(Player victim, HurtingEventArgs ev)
	{
	}

	public virtual void OnKill(Player killer, Player victim, DiedEventArgs ev)
	{
	}

	public virtual void OnDeath(Player player, DiedEventArgs ev)
	{
	}

	public virtual void OnShooting(Player player, ShootingEventArgs ev)
	{
	}

	public virtual void OnInteractingDoor(Player player, InteractingDoorEventArgs ev)
	{
	}

	public virtual void OnActivatingGenerator(Player player, ActivatingGeneratorEventArgs ev)
	{
	}

	public virtual void OnItemConsumed(Player player, ItemType item)
	{
	}

	public virtual void OnItemConsumed(Player player, Item item)
	{
		if (item != null)
		{
			OnItemConsumed(player, item.Type);
		}
	}

	protected static KeyValuePair<AmmoType, ushort> A(AmmoType type, ushort amount)
	{
		return new KeyValuePair<AmmoType, ushort>(type, amount);
	}

	protected bool IsCurrentAssignment(Player player, int stamp)
	{
		return SubclassManager.Instance?.IsCurrentAssignment(player, this, stamp) ?? false;
	}

	public override string ToString()
	{
		return Name + " (" + Id + ")";
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		return obj is Subclass subclass && string.Equals(Id, subclass.Id, StringComparison.OrdinalIgnoreCase);
	}
}

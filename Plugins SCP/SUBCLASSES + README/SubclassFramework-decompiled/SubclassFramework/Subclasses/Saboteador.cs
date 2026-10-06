using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Saboteador : Subclass
{
	private static readonly Dictionary<Generator, float> Sabotaged = new Dictionary<Generator, float>();

	private readonly Dictionary<Player, float> armed = new Dictionary<Player, float>();

	private static readonly HashSet<Player> JustSabotaged = new HashSet<Player>();

	public override string Id => "saboteador";

	public override string Name => "Saboteador";

	public override string Description => "Represor de apoyo capaz de sabotear un generador para retrasar a la Fundación.";

	public override RoleTypeId[] Roles => ChaosSubclasses.Repressor;

	public override float Chance => 10f;

	public override int MaxPerRound => 1;

	public override int MaxHealth => 100;

	public override float Cooldown => 200f;

	public override ItemType[] ExtraItems => new ItemType[1] { ItemType.GrenadeFlash };

	public override string FailMessage => "Acercate a un generador para sabotearlo.";

	public override bool UseAbility(Player player)
	{
		Generator generator = Generator.List.FirstOrDefault((Generator g) => g != null && (g.Position - player.Position).sqrMagnitude <= 16f);
		if (generator == null)
		{
			armed[player] = Time.time + 15f;
			SfUtils.Hint(player, "<color=#fc4>Sabotaje preparado: interactua con un generador.</color>");
			return true;
		}
		Sabotage(player, generator);
		return true;
	}

	public override void OnActivatingGenerator(Player player, ActivatingGeneratorEventArgs ev)
	{
		if (armed.TryGetValue(player, out var value) && !(Time.time > value) && ev.Generator != null)
		{
			armed.Remove(player);
			Sabotage(player, ev.Generator);
			JustSabotaged.Add(player);
		}
	}

	private void Sabotage(Player player, Generator generator)
	{
		Sabotaged.Clear();
		Sabotaged[generator] = Time.time + 45f;
		SfUtils.Hint(player, "<color=#fc4><b>Generador saboteado</b> durante 45 segundos.</color>");
	}

	public static void ClearAll()
	{
		Sabotaged.Clear();
		JustSabotaged.Clear();
	}

	public static bool IsSabotaged(Generator generator)
	{
		if (generator == null || !Sabotaged.TryGetValue(generator, out var value))
		{
			return false;
		}
		if (Time.time > value)
		{
			Sabotaged.Remove(generator);
			return false;
		}
		return true;
	}

	public static void PunishActivator(Player activator, Generator generator)
	{
		if ((!(activator != null) || !JustSabotaged.Remove(activator)) && IsSabotaged(generator) && !(activator == null))
		{
			SfUtils.Slow(activator, 20f, 3f);
			if (SfUtils.Roll(25f))
			{
				SfUtils.Damage(activator, 10f, "Generador saboteado");
				SfUtils.Hint(activator, "<color=#f66>El generador suelta chispas. Alguien lo ha manipulado.</color>", 4f);
			}
		}
	}

	public override void OnRemoved(Player player)
	{
		armed.Remove(player);
	}
}

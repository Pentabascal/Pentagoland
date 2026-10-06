using System.Collections.Generic;
using Exiled.API.Features;

namespace SubclassFramework.Subclasses;

public static class ZombieReviveTracker
{
	private static readonly Dictionary<Player, int> RevivedZombieCounts = new Dictionary<Player, int>();

	public static int Count(Player doctor)
	{
		return doctor != null && RevivedZombieCounts.TryGetValue(doctor, out int count) ? count : 0;
	}

	public static void Notify(Player doctor)
	{
		if (doctor != null)
		{
			RevivedZombieCounts.TryGetValue(doctor, out int count);
			RevivedZombieCounts[doctor] = count + 1;
		}
	}

	public static void Clear(Player doctor)
	{
		if (doctor != null)
		{
			RevivedZombieCounts.Remove(doctor);
		}
	}

	public static void ClearAll()
	{
		RevivedZombieCounts.Clear();
	}
}

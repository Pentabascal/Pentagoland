using Exiled.API.Features;
using PlayerRoles;
using SubclassFramework.Core;
using UnityEngine;

namespace SubclassFramework.Subclasses;

public sealed class Veterano : Subclass
{
	public override string Id => "veterano";

	public override string Name => "Veterano";

	public override string Description => "Cada 90 segundos ganas 10 de escudo permanente. Sobrevivir da recompensa.";

	public override RoleTypeId[] Roles => ChaosSubclasses.AllChaos;

	public override Tier AssignTier => Tier.General;

	public override float Chance => 10f;

	public override float Cooldown => 90f;

	public override bool UseAbility(Player player)
	{
		SfUtils.AddPermanentAhp(player, 10f);
		SfUtils.Hint(player, $"<color=#8f8>+10 de escudo. Total: {Mathf.RoundToInt(player.ArtificialHealth)}</color>", 4f);
		return true;
	}
}

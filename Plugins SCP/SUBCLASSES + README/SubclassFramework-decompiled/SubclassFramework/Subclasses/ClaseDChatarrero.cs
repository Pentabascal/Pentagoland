using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using PlayerRoles;
using SubclassFramework.Core;

namespace SubclassFramework.Subclasses;

public sealed class ClaseDChatarrero : Subclass
{
	private readonly HashSet<Player> spent = new HashSet<Player>();

	public override string Id => "clased_chatarrero";

	public override string Name => "Clase D Chatarrero";

	public override string Description => "Recoge monedas. Cuantas más lleves al canjearlas, mejor será el premio. Solo una vez por ronda.";

	public override RoleTypeId[] Roles => FacilitySubclasses.ClassD;

	public override float Chance => 10f;

	public override float Cooldown => 3600f;

	public override string FailMessage => "Todavía no puedes canjearlas (espera a los 3 minutos de ronda) o ya lo has hecho.";

	public override bool UseAbility(Player player)
	{
		if (spent.Contains(player))
		{
			return false;
		}
		if (Round.ElapsedTime.TotalSeconds < 180.0)
		{
			int num = 180 - (int)Round.ElapsedTime.TotalSeconds;
			SfUtils.Hint(player, $"<color=#fc6>Aun no. Faltan {num}s para poder canjear.</color>");
			return false;
		}
		int num2 = SfUtils.CountItem(player, ItemType.Coin);
		spent.Add(player);
		foreach (Item item in player.Items.Where((Item item) => item != null && item.Type == ItemType.Coin).Take(num2).ToList())
		{
			player.RemoveItem(item);
		}
		switch (num2)
		{
		case 0:
		case 1:
			SfUtils.Hint(player, "<color=#aaa>Con una sola moneda no puedes conseguir ningún premio.</color>", 6f);
			break;
		case 2:
		{
			ItemType[] list = new ItemType[3]
			{
				ItemType.Medkit,
				ItemType.Painkillers,
				ItemType.Adrenaline
			};
			ItemType itemType = SfUtils.Pick(list);
			player.AddItem(itemType);
			SfUtils.Hint(player, $"<color=#9f9>Premio: <b>{itemType}</b></color>", 6f);
			break;
		}
		case 3:
			SfUtils.SetMaxHealth(player, player.MaxHealth + 10f, healToFull: false);
			SfUtils.Heal(player, 10f);
			SfUtils.Hint(player, "<color=#9f9>+10 de vida maxima permanente.</color>", 6f);
			break;
		case 4:
			SfUtils.SpeedBoost(player, 12f, 90f);
			SfUtils.Hint(player, "<color=#9f9>Velocidad extra durante 90 segundos.</color>", 6f);
			break;
		default:
			player.AddItem(ItemType.GunCOM15);
			player.AddAmmo(AmmoType.Nato9, (ushort)((num2 >= 6) ? 40u : 12u));
			SfUtils.Hint(player, (num2 >= 6) ? "<color=#8f8><b>Premio gordo:</b> COM-15 y 40 balas.</color>" : "<color=#8f8><b>Premio gordo:</b> COM-15.</color>", 6f);
			break;
		}
		return true;
	}

	public override void OnRemoved(Player player)
	{
		spent.Remove(player);
	}
}

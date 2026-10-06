namespace EffectKeeper;

using InventorySystem.Items.Usables.Scp1344;
using CustomPlayerEffects;
using HarmonyLib;
using PlayerRoles;
using MEC;
    
#if EXILED
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
#endif

[HarmonyPatch(typeof(PlayerEffectsController), nameof(PlayerEffectsController.OnRoleChanged))]
public static class EffectPatch
{
    private static Config Config => EffectKeeper.Instance.Config;

    private static bool Prefix(
        PlayerEffectsController __instance,
        ReferenceHub targetHub,
        PlayerRoleBase oldRole,
        PlayerRoleBase newRole)
    {
        if (targetHub != __instance._hub)
            return false;

        bool isDeath = Config.DeathCancels && (oldRole.Team == Team.Dead || newRole.Team == Team.Dead);

        foreach (StatusEffectBase statusEffectBase in __instance.AllEffects)
        {
            if (isDeath)
            {
                statusEffectBase.OnDeath(oldRole);
                continue;
            }

            bool shouldCancel = false;

#if EXILED
            if (!statusEffectBase.TryGetEffectType(out EffectType effectType))
                continue;

            EffectCategory categories = effectType.GetCategories();
            if (!Config.AllowedEffects.Contains(effectType) && !Config.AllowedCategories.Any(c => categories.HasFlag(c)))
                shouldCancel = true;
#else
            shouldCancel = !Config.AllowedCategories.Contains(statusEffectBase.Classification)
                           && !Config.AllowedEffects.Contains(statusEffectBase.name);
#endif
            if (shouldCancel)
            {
                statusEffectBase.OnRoleChanged(oldRole, newRole);
                continue;
            }
            
#if EXILED
            if (!Config.ReapplyEffects.Contains(effectType))
                continue;
#else
            if (!Config.ReapplyEffects.Contains(statusEffectBase.name))
                continue;
#endif
            byte intensity = statusEffectBase.Intensity;
            if (intensity == 0)
                continue;
            
            statusEffectBase.Intensity = 0;
            Timing.CallDelayed(0, () =>
            {
                if (statusEffectBase is Scp1344)
                {
                    Scp1344Item? scp1344Item =
                        targetHub.inventory.UserInventory.Items.FirstOrDefault(kvp =>
                            kvp.Value.ItemTypeId == ItemType.SCP1344).Value as Scp1344Item;
                    
                    if (scp1344Item)
                        scp1344Item.Status = Scp1344Status.Active;
                }
                
                statusEffectBase.Intensity = intensity;
            });
        }

        return false;
    }
}
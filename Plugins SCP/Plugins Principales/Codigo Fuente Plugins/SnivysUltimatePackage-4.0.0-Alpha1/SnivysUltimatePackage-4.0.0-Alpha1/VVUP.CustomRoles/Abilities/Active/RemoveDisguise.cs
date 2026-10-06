using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomRoles.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class RemoveDisguise : ActiveAbility
    {
        public override string Name { get; set; } = "Quitar disfraz";

        public override string Description { get; set; } =
            "Al activarla, te quitarás el disfraz y no podrás volver a ponértelo. Úsala con cuidado.";

        public override float Duration { get; set; } = 0f;
        public override float Cooldown { get; set; } = 5f;
        
        [Description("A veces se suelta el inventario del jugador. Actívalo para devolvérselo automáticamente.")]
        public bool RestorePreviousInventory { get; set; } = false;

        [Description("Si está activado, el jugador podrá quitarse el disfraz y adoptar un rol personalizado.")]
        public bool UseCustomRoles { get; set; } = false;

        [Description(
            "Si UseCustomRoles está desactivado, determina con qué rol normal dejará de estar disfrazado el jugador.")]
        public RoleTypeId UndisguiseRole { get; set; } = RoleTypeId.Tutorial;

        [Description(
            "Si UseCustomRoles está activado, determina con qué rol personalizado dejará de estar disfrazado el jugador, mediante su ID de rol personalizado.")]
        public uint UndisguiseCustomRole { get; set; } = 0;

        [Description("Si está activado, se guardará y restaurará la posición del jugador al usar la habilidad. Es útil para los roles personalizados, que normalmente aparecen en su punto de aparición asignado.")]
        public bool SavePosition { get; set; } = true;
        

        protected override void AbilityUsed(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Removing {player.Nickname} disguise");
            List<Item> storedInventory = player.Items.ToList();

            var ammoCount = player.Ammo.ToDictionary(ammo => ammo.Key, ammo => ammo.Value);

            Vector3 savedPosition = player.Position;
            if (UseCustomRoles)
            {
                CustomRole.Get(UndisguiseCustomRole)?.AddRole(player);
                if (SavePosition)
                    Timing.CallDelayed(0.25f, () => player.Position = savedPosition);

                Log.Debug($"VVUP Custom Abilities: {player.Nickname} undisguised into custom role into {CustomRole.Get(UndisguiseCustomRole)?.Name})");
            }
            else
            {
                RoleSpawnFlags spawnFlags;

                if (SavePosition && RestorePreviousInventory)
                    spawnFlags = RoleSpawnFlags.All;
                else if (SavePosition)
                    spawnFlags = RoleSpawnFlags.AssignInventory;
                else if (RestorePreviousInventory)
                    spawnFlags = RoleSpawnFlags.UseSpawnpoint;
                else
                    spawnFlags = RoleSpawnFlags.None;
                player.Role.Set(UndisguiseRole, spawnFlags);
                Log.Debug($"VVUP Custom Abilities: {player.Nickname} undisguised into role {UndisguiseRole}");
            }
            
            if (RestorePreviousInventory)
            {
                Timing.CallDelayed(1f, () =>
                {
                    player.ClearInventory();
                    
                    foreach (Item item in storedInventory)
                    {
                        player.AddItem(item);
                    }

                    foreach (KeyValuePair<ItemType, ushort> ammo in ammoCount)
                    {
                        player.AddAmmo((AmmoType)ammo.Key, ammo.Value);
                    }
                });
            }
        }
    }
}
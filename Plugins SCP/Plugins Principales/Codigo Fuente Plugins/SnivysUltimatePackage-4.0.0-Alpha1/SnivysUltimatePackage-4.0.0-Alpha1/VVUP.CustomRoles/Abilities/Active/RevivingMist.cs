/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class RevivingMist : ActiveAbility
    {
        private readonly List<CoroutineHandle> coroutines = new();
        private readonly Dictionary<Player, DeathInfo> recentlyDead = new();
        
        public override string Name { get; set; } = "Niebla de reanimación";
        public override string Description { get; set; } = "Crea una niebla que cura a tus aliados y reanima a los compañeros que hayan caído recientemente.";
        public override float Duration { get; set; } = 1f;
        public override float Cooldown { get; set; } = 180f;
        public float ReviveRadius { get; set; } = 5f;
        public float ReviveTimeWindow { get; set; } = 30f;
        public float ReviveHealthPercent { get; set; } = 30f;
        public string ReviveMessage { get; set; } = "¡Un sanitario te ha reanimado!";
        public ushort ReviveMessageTime { get; set; } = 5;
        public bool ReviveTeammatesOnly { get; set; } = true;
        public bool GrantLoadoutOnRevive { get; set; } = false;
        [Description("Si está activado, el jugador revivirá con un rol determinado; si no, conservará su rol original.")]
        public bool ReviveToSetRole { get; set; } = false;
        [Description("Si ReviveToSetRole está activado, determina si el jugador revivirá con un rol personalizado; de lo contrario, tendrá un rol normal.")]
        public bool ReviveToCustomRole { get; set; } = false;
        public RoleTypeId ReviveBaseRole { get; set; } = RoleTypeId.Tutorial;
        public uint ReviveCustomRoleId { get; set; } = 25;

        protected override void AbilityAdded(Player player)
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDied;
        }

        protected override void AbilityRemoved(Player player)
        {
            Exiled.Events.Handlers.Player.Died -= OnPlayerDied;
            foreach (CoroutineHandle handle in coroutines)
                Timing.KillCoroutines(handle);
        }

        private void OnPlayerDied(DiedEventArgs ev)
        {
            // Store info about dead players
            if (ev.Player == null)
                return;
            
            recentlyDead[ev.Player] = new DeathInfo
            {
                DeathTime = DateTime.Now,
                Role = ev.Player.Role.Type,
                Position = ev.Player.Position,
                Side = ev.Player.Role.Side
            };
            Log.Info($"Player died: {ev.Player.Nickname}, Role: {ev.Player.Role.Type}, Time: {DateTime.Now}");
        }

        protected override void AbilityUsed(Player player)
        {
            Log.Info($"Ability used by {player.Nickname}. Dictionary has {recentlyDead.Count} dead players");
            
            if (recentlyDead.Count == 0)
            {
                Log.Info("No recently dead players found to revive");
                return;
            }
            
            List<Player> toRemove = new List<Player>();
            
            foreach (var deadPlayer in recentlyDead)
            {
                Log.Info($"Checking dead player: {deadPlayer.Key.Nickname}, Time since death: {(DateTime.Now - deadPlayer.Value.DeathTime).TotalSeconds}s, Distance: {Vector3.Distance(deadPlayer.Value.Position, player.Position)}m");
                if (ReviveTeammatesOnly && deadPlayer.Value.Side != player.Role.Side)
                {
                    Log.Info($"Skipping {deadPlayer.Key.Nickname}: not on the same team");
                    continue;
                }
                
                TimeSpan timeSinceDeath = DateTime.Now - deadPlayer.Value.DeathTime;
                Log.Info($"{timeSinceDeath.TotalSeconds}, {ReviveTimeWindow}");
                if (timeSinceDeath.TotalSeconds > ReviveTimeWindow)
                {
                    toRemove.Add(deadPlayer.Key);
                    continue;
                }

                if ((deadPlayer.Value.Position - player.Position).sqrMagnitude > ReviveRadius * ReviveRadius)
                    continue;

                Player deadPlayerKey = deadPlayer.Key;
                Vector3 revivePosition = deadPlayer.Value.Position;
                float healthToSet = deadPlayerKey.MaxHealth * (ReviveHealthPercent / 100f);

                if (ReviveToSetRole)
                {
                    if (ReviveToCustomRole)
                    {
                        CustomRole.Get(ReviveCustomRoleId)?.AddRole(deadPlayerKey);
                        if (!GrantLoadoutOnRevive)
                            deadPlayerKey.ClearInventory();
                        Log.Debug($"VVUP Custom Abilities: Reviving Mist: {deadPlayerKey.Nickname} has been revived to custom role id {ReviveCustomRoleId} at position {revivePosition}");
                    }
                    else
                    {
                        deadPlayerKey.Role.Set(ReviveBaseRole, GrantLoadoutOnRevive ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.None);
                        Log.Debug($"VVUP Custom Abilities: Reviving Mist: {deadPlayerKey.Nickname} has been revived to role {ReviveBaseRole} at position {revivePosition}");
                    }
                }
                else
                {
                    deadPlayerKey.Role.Set(deadPlayer.Value.Role, GrantLoadoutOnRevive ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.None);
                    Log.Debug($"VVUP Custom Abilities: Reviving Mist: {player.Nickname} has been revived to their original role {deadPlayer.Value.Role} at position {revivePosition}");
                }

                Timing.CallDelayed(0.2f, () =>
                {
                    deadPlayerKey.Position = revivePosition;
                    deadPlayerKey.Health = healthToSet;
                });

                deadPlayerKey.Broadcast(ReviveMessageTime, ReviveMessage);
                Log.Debug($"VVUP Custom Abilities: Reviving Mist: Showing revive message to {deadPlayerKey.Nickname}: {ReviveMessage}");
                toRemove.Add(deadPlayerKey);
            }
            
            // Clean up the dictionary
            foreach (var deadPlayerKey in toRemove)
                recentlyDead.Remove(deadPlayerKey);
        }
        
        private void CheckForDeadTeammates(Player activator)
        {
            
        }
        
        private class DeathInfo
        {
            public DateTime DeathTime { get; set; }
            public RoleTypeId Role { get; set; }
            public Vector3 Position { get; set; }
            public Side Side { get; set; }
        }
    }
}*/
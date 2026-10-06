using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.Coin)]
    public class Scp427 : CustomItem, ICustomItemGlow
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.Coin;
        public override uint Id { get; set; } = 59;
        public override string Name { get; set; } = "<color=#FF0000>SCP-427</color>";
        public override string Description { get; set; } = "Moneda que regenera la salud de quien la lleva. La exposición prolongada convierte al usuario en un montón de carne.";
        public override float Weight { get; set; } = 1;
        [Description("Tiempo, en segundos, que tarda el jugador en morir por la exposición al objeto. 0 desactiva este efecto.")]
        public float TimeToDeath { get; set; } = 300f;
        [Description("Tiempo de exposición, en segundos, tras el que se mostrará el aviso.")]
        public float WarningThreshold { get; set; } = 240f;
        [Description("Salud que se regenera por segundo.")]
        public float RegenerationPerSecond { get; set; } = 0.25f;
        [Description("¿El objeto otorga vida artificial al jugador?")]
        public bool GiveAhp { get; set; } = true;
        [Description("Límite de vida artificial que puede obtener el jugador con este objeto.")]
        public float AhpLimit { get; set; } = 50f;
        public string StartRegenerationMessage { get; set; } = "Sientes una cálida sensación que se extiende por tu cuerpo.";
        public string StopRegenerationMessage { get; set; } = "La cálida sensación desaparece.";
        public string WarningMessage { get; set; } = "Empiezas a notar que tu cuerpo se convierte en una masa informe.";
        public string DeathMessage { get; set; } = "Has sucumbido a los efectos de SCP-427.";
        public float MessageDuration { get; set; } = 5f;
        public bool UseHints { get; set; } = true;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            LockerSpawnPoints = new()
            {
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp018Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp207Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp244Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp268Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp500Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp1576Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp1344Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp1853Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.Scp2176Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
                new()
                {
                    Chance = 10,
                    Type = LockerType.AntiScp207Pedestal,
                    UseChamber = true,
                    Offset = Vector3.zero,
                },
            },
        };
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        private readonly Dictionary<Player, float> exposureTimes = new();
        private readonly Dictionary<Player, CoroutineHandle> activeCoroutines = new();
        private readonly Dictionary<Player, bool> warningShown = new();

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += OnFlippingCoin;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= OnFlippingCoin;
            base.UnsubscribeEvents();
        }
        
        protected override void OnWaitingForPlayers()
        {
            exposureTimes.Clear();
            foreach (var coroutine in activeCoroutines.Values)
                Timing.KillCoroutines(coroutine);
            activeCoroutines.Clear();
            warningShown.Clear();
            base.OnWaitingForPlayers();
        }

        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            if (ev.Player == null)
                return;
            if (activeCoroutines.ContainsKey(ev.Player))
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
            }
            exposureTimes.Remove(ev.Player);
            warningShown.Remove(ev.Player);
            base.OnOwnerChangingRole(ev);
        }

        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (activeCoroutines.ContainsKey(ev.Player))
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                if (!string.IsNullOrWhiteSpace(StopRegenerationMessage))
                    if (UseHints)
                    {
                        ev.Player.ShowHint(StopRegenerationMessage, MessageDuration);
                        Log.Debug($"VVUP Custom Items, SCP-427: Showing stop regeneration hint to {ev.Player.Nickname} due to dropping SCP-427.");
                    }
                    else
                    {
                        ev.Player.Broadcast((ushort)MessageDuration, StopRegenerationMessage);
                        Log.Debug($"VVUP Custom Items, SCP-427: Showing stop regeneration broadcast to {ev.Player.Nickname} due to dropping SCP-427.");
                    }
            }
            base.OnDroppingItem(ev);
        }
        
        private void OnFlippingCoin(FlippingCoinEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.IsAllowed = false;
            if (!exposureTimes.ContainsKey(ev.Player))
                exposureTimes[ev.Player] = 0f;

            if (activeCoroutines.ContainsKey(ev.Player))
            {
                Timing.KillCoroutines(activeCoroutines[ev.Player]);
                activeCoroutines.Remove(ev.Player);
                if (!string.IsNullOrWhiteSpace(StopRegenerationMessage))
                    if (UseHints)
                    {
                        ev.Player.ShowHint(StopRegenerationMessage, MessageDuration);
                        Log.Debug($"VVUP Custom Items, SCP-427: Showing stop regeneration hint to {ev.Player.Nickname} due to flipping SCP-427.");
                    }
                    else
                    {
                        ev.Player.Broadcast((ushort)MessageDuration, StopRegenerationMessage);
                        Log.Debug($"VVUP Custom Items, SCP-427: Showing stop regeneration broadcast to {ev.Player.Nickname} due to flipping SCP-427.");
                    }
            }
            else
            {
                activeCoroutines[ev.Player] = Timing.RunCoroutine(RegenerationCoroutine(ev.Player));
                if (!string.IsNullOrWhiteSpace(StartRegenerationMessage))
                    if (UseHints)
                    {
                        ev.Player.ShowHint(StartRegenerationMessage, MessageDuration);
                        Log.Debug($"VVUP Custom Items, SCP-427: Showing start regeneration hint to {ev.Player.Nickname} due to flipping SCP-427.");
                    }
                    else
                    {
                        ev.Player.Broadcast((ushort)MessageDuration, StartRegenerationMessage);
                        Log.Debug($"VVUP Custom Items, SCP-427: Showing start regeneration broadcast to {ev.Player.Nickname} due to flipping SCP-427.");
                    }
            }
        }
        
        private IEnumerator<float> RegenerationCoroutine(Player player)
        {
            while (TimeToDeath <= 0 || exposureTimes[player] < TimeToDeath)
            {
                yield return Timing.WaitForSeconds(1f);

                if (!player.IsAlive)
                {
                    Log.Debug($"VVUP Custom Items, SCP-427: Stopping regeneration coroutine for {player.Nickname} as they are dead.");
                    activeCoroutines.Remove(player);
                    yield break;
                }
                Log.Debug($"VVUP Custom Items, SCP-427: Regenerating health for {player.Nickname}. Current exposure time: {exposureTimes[player]} seconds.");
                exposureTimes[player] += 1f;

                if (player.Health < player.MaxHealth)
                {
                    player.Health += RegenerationPerSecond;
                    Log.Debug($"VVUP Custom Items, SCP-427: {player.Nickname} health increased by {RegenerationPerSecond} to {player.Health}.");
                }

                if (GiveAhp && player.Health >= player.MaxHealth && player.ArtificialHealth < AhpLimit)
                {
                    player.AddAhp(RegenerationPerSecond, decay: 0);
                    Log.Debug($"VVUP Custom Items, SCP-427: {player.Nickname} AHP increased by {RegenerationPerSecond} to {player.ArtificialHealth}.");
                }
                
                if (WarningThreshold > 0 && exposureTimes[player] >= WarningThreshold && !warningShown.ContainsKey(player))
                {
                    warningShown[player] = true;
                    if (!string.IsNullOrWhiteSpace(WarningMessage))
                        if (UseHints)
                        {
                            player.ShowHint(WarningMessage, MessageDuration);
                            Log.Debug($"VVUP Custom Items, SCP-427: Showing warning hint to {player.Nickname} for prolonged exposure.");
                        }
                        else
                        {
                            player.Broadcast((ushort)MessageDuration, WarningMessage);
                            Log.Debug($"VVUP Custom Items, SCP-427: Showing warning broadcast to {player.Nickname} for prolonged exposure.");
                        }
                }
                
                if (TimeToDeath > 0 && exposureTimes[player] >= TimeToDeath)
                {
                    player.Kill(DeathMessage);
                    exposureTimes.Remove(player);
                    activeCoroutines.Remove(player);
                    yield break;
                }
            }
        }
    }
}
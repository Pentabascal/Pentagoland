using System;
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
using PlayerRoles;
using PlayerRoles.Voice;
using UnityEngine;
using VVUP.Base.API;
using YamlDotNet.Serialization;
using IntercomBase = PlayerRoles.Voice.Intercom;
using IntercomExiled = Exiled.API.Features.Intercom;

namespace VVUP.CustomItems.Items.Other
{
    [CustomItem(ItemType.Radio)]
    public class PortableIntercom : CustomItem, ICustomItemGlow
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.Radio;
        public override uint Id { get; set; } = 47;
        public override string Name { get; set; } = "Intercomunicador portátil";

        public override string Description { get; set; } =
            "Intercomunicador portátil para comunicarse con todas las instalaciones.";

        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new()
                {
                    Chance = 10,
                    Location = SpawnLocationType.InsideGr18,
                },
            },
            RoleSpawnPoints = new List<RoleSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Role = RoleTypeId.Scp106
                }
            },
            RoomSpawnPoints = new List<RoomSpawnPoint>
            {
                new()
                {
                    Chance = 10,
                    Room = RoomType.HczTestRoom,
                    Offset = new Vector3(0.885f, 0.749f, -4.874f)
                },
                new()
                {
                    Chance = 100,
                    Offset = new Vector3(-3.25f, 1, -0.78f),
                    Room = RoomType.Hcz096,
                },
            },
        };
        public float PortableIntercomDuration { get; set; } = 20f;
        [Description("Usa %intercomtimeremaining% para mostrar cuánto tiempo queda de emisión.")]
        public string CountdownTextForPortableIntercom { get; set; } =
            "El intercomunicador portátil dejará de emitir en %intercomtimeremaining% segundos.";
        [Description("Si está activado, volver a alternar el intercomunicador portátil mientras está en uso finalizará la emisión.")]
        public bool EndIntercomOnSecondInteraction { get; set; } = true;
        public bool UseHints { get; set; } = true;
        [Description("Si está activado, el intercomunicador portátil respetará, entre otras cosas, el tiempo de recarga y la desactivación del intercomunicador.")]
        public bool FollowActualIntercomParameters { get; set; } = true;
        public string PortableIntercomFailDueToIntercomCooldownText { get; set; } =
            "El intercomunicador está recargándose. Espera antes de volver a usarlo.";

        public string IntercomRoomPortableIntercomInUseText { get; set; } =
            "El intercomunicador ya está en uso en otra ubicación.";
        
        public bool HasCustomItemGlow { get; set; } = false;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 255, 255, 255);
        public float GlowRange { get; set; } = 0.25f;
        public float GlowIntensity { get; set; } = 0.25f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;
        
        private bool _isPortableIntercomActive = false;
        private static CoroutineHandle _portableIntercomCoroutine;
        private List<Player> _playerWithPortableIntercom = new List<Player>();

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            Exiled.Events.Handlers.Player.TogglingRadio += OnTogglingRadio;
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.TogglingRadio -= OnTogglingRadio;
            base.UnsubscribeEvents();
        }

        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            if (_playerWithPortableIntercom.Contains(ev.Player) && _isPortableIntercomActive)
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is changing item while using the portable intercom, removing them from the list and disabling intercom override.");
                _playerWithPortableIntercom.Remove(ev.Player);
                IntercomBase.TrySetOverride(ev.Player.ReferenceHub, false);
                if (_playerWithPortableIntercom.Count == 0)
                {
                    _isPortableIntercomActive = false;
                    IntercomExiled.PlaySound(false);
                    IntercomExiled.DisplayText = string.Empty;
                    IntercomExiled.State = IntercomState.Cooldown;
                }
            }
            base.OnChanging(ev);
        }
        protected override void OnDroppingItem(DroppingItemEventArgs ev)
        {
            if (_playerWithPortableIntercom.Contains(ev.Player) && _isPortableIntercomActive)
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is dropping item while using the portable intercom, removing them from the list and disabling intercom override.");
                _playerWithPortableIntercom.Remove(ev.Player);
                IntercomBase.TrySetOverride(ev.Player.ReferenceHub, false);
                if (_playerWithPortableIntercom.Count == 0)
                {
                    _isPortableIntercomActive = false;
                    IntercomExiled.PlaySound(false);
                    IntercomExiled.DisplayText = string.Empty;
                    IntercomExiled.State = IntercomState.Cooldown;
                }
            }
            base.OnDroppingItem(ev);
        }

        protected override void OnOwnerChangingRole(OwnerChangingRoleEventArgs ev)
        {
            if (_playerWithPortableIntercom.Contains(ev.Player) && _isPortableIntercomActive &&
                Check(ev.Player.CurrentItem))
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is changing role while using the portable intercom, removing them from the list and disabling intercom override.");
                _playerWithPortableIntercom.Remove(ev.Player);
                IntercomBase.TrySetOverride(ev.Player.ReferenceHub, false);
                if (_playerWithPortableIntercom.Count == 0)
                {
                    _isPortableIntercomActive = false;
                    IntercomExiled.PlaySound(false);
                    IntercomExiled.DisplayText = string.Empty;
                    IntercomExiled.State = IntercomState.Cooldown;
                }
            }
            base.OnOwnerChangingRole(ev);
        }

        protected override void OnWaitingForPlayers()
        {
            Log.Debug("VVUP Custom Items, Portable Intercom: At Waiting for players, clearing portable intercom data.");
            _playerWithPortableIntercom.Clear();
            _isPortableIntercomActive = false;
            if (_portableIntercomCoroutine.IsRunning)
                Timing.KillCoroutines(_portableIntercomCoroutine);
            base.OnWaitingForPlayers();
        }

        private void OnTogglingRadio(TogglingRadioEventArgs ev)
        {
            if (!Check(ev.Player.CurrentItem))
                return;
            if (_isPortableIntercomActive && EndIntercomOnSecondInteraction && _playerWithPortableIntercom.Contains(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is toggling the portable intercom while it is active, ending the intercom.");
                _playerWithPortableIntercom.Remove(ev.Player);
            }
            else if (_isPortableIntercomActive && _playerWithPortableIntercom.Contains(ev.Player))
            {
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is toggling the portable intercom while it is active, but EndIntercomOnSecondInteraction is false, ignoring.");
                return;
            }

            if (FollowActualIntercomParameters && IntercomExiled.State != IntercomState.Ready)
            {
                if (!string.IsNullOrWhiteSpace(PortableIntercomFailDueToIntercomCooldownText))
                    if (UseHints)
                        ev.Player.ShowHint(PortableIntercomFailDueToIntercomCooldownText, 5);
                    else
                        ev.Player.Broadcast(5, PortableIntercomFailDueToIntercomCooldownText);
                Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} tried to use the portable intercom, but it is on cooldown.");
                return;
            }
            Log.Debug($"VVUP Custom Items, Portable Intercom: {ev.Player.Nickname} is toggling the portable intercom, starting the intercom, adding to list, setting Intercom State, setting Intercom Display, setting {ev.Player.Nickname} voice channel, Starting Coroutine");
            _isPortableIntercomActive = true;
            ev.IsAllowed = false;
            ev.Radio.IsEnabled = true;
            _playerWithPortableIntercom.Add(ev.Player);
            IntercomExiled.PlaySound(true);
            IntercomExiled.State = IntercomState.Cooldown;
            IntercomExiled.DisplayText = IntercomRoomPortableIntercomInUseText;
            Timing.CallDelayed(1.5f, () => IntercomBase.TrySetOverride(ev.Player.ReferenceHub, true));
            _portableIntercomCoroutine = Timing.RunCoroutine(PortableIntercomTiming(ev.Player));
        }
        private IEnumerator<float> PortableIntercomTiming(Player player)
        {
            float time = PortableIntercomDuration;
            while (time > 0)
            {
                if (!_playerWithPortableIntercom.Contains(player))
                {
                    Log.Debug(
                        $"VVUP Custom Items, Portable Intercom: {player.Nickname} is no longer in the list of players with the portable intercom, ending the intercom.");
                    _isPortableIntercomActive = false;
                    IntercomExiled.PlaySound(false);
                    IntercomExiled.DisplayText = string.Empty;
                    IntercomExiled.State = IntercomState.Cooldown;
                    IntercomBase.TrySetOverride(player.ReferenceHub, false);
                    yield break;
                }

                yield return Timing.WaitForSeconds(1f);
                time = Mathf.Max(0f, time - 1f);
                Log.Debug(
                    $"VVUP Custom Items, Portable Intercom: {player.Nickname} portable intercom time remaining: {time}, showing message to {player.Nickname} showing how much time they have remaining. Waiting 1 Second.");
                if (!string.IsNullOrWhiteSpace(CountdownTextForPortableIntercom))
                {
                    string message = ProcessStringVaribles(CountdownTextForPortableIntercom, time);
                    if (UseHints)
                        player.ShowHint(message, 1);
                    else
                        player.Broadcast(1, message, shouldClearPrevious: true);
                }
            }

            Log.Debug($"VVUP Custom Items, Portable Intercom: {player.Nickname} portable intercom has ended, removing from list and ending the intercom.");
            _isPortableIntercomActive = false;
            IntercomExiled.PlaySound(false);
            _playerWithPortableIntercom.Remove(player);
            IntercomBase.TrySetOverride(player.ReferenceHub, false);
            yield break;
        }
        private string ProcessStringVaribles(string text, float time)
        {
            return text.Replace("%intercomtimeremaining%", Math.Floor(time).ToString());
        }
    }
}
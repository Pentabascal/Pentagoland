using System;
using System.Linq;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using Exiled.API.Features;
using RueI.API.Elements;
using System.Text;
using RueI.API;
using MEC;

// Copyright (c) 2026 polyshed

namespace SpectatorListRueI
{
    public class EventHandler
    {
        private readonly DynamicElement _spectatorElement;
        private Config _config => Plugin.Instance.Config;
        private static readonly Tag tagSpectator = new("SpectatorListElement");
        
        public EventHandler()
        {
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            
            _spectatorElement = new DynamicElement(Plugin.Instance.Config.Position, (hub =>
            {
                Player player = Player.Get(hub);
                int count = player.CurrentSpectatingPlayers.Count(p => p.Role != RoleTypeId.Overwatch);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(count == 0
                    ? String.Empty
                    : _config.Spectators.Replace("%amount%", count.ToString()));
                
                int iteration = 0;
                foreach (Player spectator in player.CurrentSpectatingPlayers.Where(p => p.Role != RoleTypeId.Overwatch))
                {
                    if (iteration >= _config.MaximumLines)
                    {
                        int overflowCount = count - iteration;
                        sb.AppendLine(_config.OverflowText.Replace("%overflow%", overflowCount.ToString()));
                        break;
                    }
                    
                    sb.AppendLine(_config.PlayerDisplay.Replace("%name%", spectator.CustomName));
                    
                    iteration++;
                }

                return _config.FullText.Replace("%display%", sb.ToString());
            }))
            {
                ResolutionBasedAlign = true,
                UpdateInterval = TimeSpan.FromSeconds(_config.RefreshRate)
            };
        }

        public void UnregisterEvents()
        {
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            Player player = ev.Player;
            Timing.CallDelayed(2f, () =>
            {
                if (!player.IsConnected)
                    return;

                RueDisplay display = RueDisplay.Get(player);
                display.Show(tagSpectator, _spectatorElement);
            });
        }
        
    }
}

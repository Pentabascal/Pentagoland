using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Pentagoland.Utilities
{
    [DataContract]
    public sealed class PentagolandServerStatus
    {
        [DataMember(Order = 1)]
        public int SchemaVersion { get; set; } = 1;

        [DataMember(Order = 2)]
        public string ServerName { get; set; }

        [DataMember(Order = 3)]
        public string ServerTagline { get; set; }

        [DataMember(Order = 4)]
        public bool Online { get; set; }

        [DataMember(Order = 5)]
        public int OnlinePlayers { get; set; }

        [DataMember(Order = 6)]
        public int MaxPlayers { get; set; }

        [DataMember(Order = 7)]
        public string UpdatedAtUtc { get; set; }

        public static PentagolandServerStatus Create(
            int onlinePlayers,
            int maxPlayers,
            string serverName,
            string serverTagline)
        {
            return new PentagolandServerStatus
            {
                ServerName = string.IsNullOrWhiteSpace(serverName) ? "Pentagoland" : serverName.Trim(),
                ServerTagline = string.IsNullOrWhiteSpace(serverTagline) ? string.Empty : serverTagline.Trim(),
                Online = true,
                OnlinePlayers = Math.Max(0, onlinePlayers),
                MaxPlayers = Math.Max(1, maxPlayers),
                UpdatedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
            };
        }

        public string BuildStatusText()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] {1} | Jugadores en línea: {2}/{3}",
                ServerName,
                ServerTagline,
                OnlinePlayers,
                MaxPlayers);
        }
    }
}

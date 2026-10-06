using Exiled.API.Interfaces;

// Copyright (c) 2026 polyshed

namespace SpectatorListRueI
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = false;
        public bool Debug { get; set; } = false;
        public float Position { get; set; } = 750F;
        public string Spectators { get; set; } = "👥 Espectadores (%amount%)";
        public string PlayerDisplay { get; set; } = "-> %name%";
        public string FullText { get; set; } = "<align=left><space=50><size=23>%display%</size></align>";
        public float RefreshRate { get; set; } = 2F;
        public int MaximumLines { get; set; } = 6;
        public string OverflowText { get; set; } = "y %overflow% más...";
    }
}

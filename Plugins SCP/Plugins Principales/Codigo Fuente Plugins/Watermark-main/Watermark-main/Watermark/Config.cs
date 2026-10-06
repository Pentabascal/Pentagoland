using System.ComponentModel;
using Exiled.API.Interfaces;

// Copyright (c) 2026 polyshed

namespace Watermark
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("Watermark text. RueI rich text markup is supported.")]
        public string Text { get; set; } = "<align=center><size=16><u><color=#FFB6C1>P</color><color=#FFCCD4>e</color><color=#FFE2E6>n</color><color=#FFF8F9>t</color><color=#EBDEF0>a</color><color=#CDACDB>g</color><color=#AF7AC5>o</color><color=#A562B7>l</color><color=#C37EBA>a</color><color=#E19ABE>n</color><color=#FFB6C1>d</color></u></size></align>";
        [Description("The vertical position of the watermark text (0 = bottom, 1000 = top). Choose a position not used by other HUD elements.")]
        public float Position { get; set; } = 10f;
    }
}
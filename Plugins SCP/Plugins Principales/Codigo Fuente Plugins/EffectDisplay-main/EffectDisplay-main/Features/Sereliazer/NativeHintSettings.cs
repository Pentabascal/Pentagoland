using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffectDisplay.Features.Sereliazer
{
    public class NativeHintSettings
    {
        [Description("Tamaño del texto.")]
        public int FontSize { get; set; } = 12;
        [Description("Alineación del texto.")]
        public string Aligment { get; set; } = "Left";
    }
}

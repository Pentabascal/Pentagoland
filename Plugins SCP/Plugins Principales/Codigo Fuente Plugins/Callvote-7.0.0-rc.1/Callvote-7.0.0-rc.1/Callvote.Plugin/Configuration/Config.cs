#if EXILED
using Exiled.API.Interfaces;
#endif
using System.ComponentModel;

namespace Callvote.Configuration
{
#if EXILED
    public class Config : IConfig
#else
    public class Config
#endif
    {
#if EXILED
        [Description("Indica si el complemento está activado.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indica si están activados los registros de depuración del complemento.")]
        public bool Debug { get; set; } = false;

#endif
        [Description("Qué proveedor de mensajes debe usar Callvote. Opciones: auto, ruei o broadcasts/bc.")]
        public string MessageProvider { get; set; } = "auto";

        [Description("Establece la coordenada Y del aviso en una escala de 0 a 1000, donde 0 representa la parte inferior de la pantalla. No se aplica a las retransmisiones.")]
        public float HintYCoordinate { get; set; } = 300;

        [Description("Activa o desactiva los módulos.")]
        public bool EnableQueue { get; set; } = true;

        public bool EnableKick { get; set; } = true;

        public bool EnableFf { get; set; } = true;

        public bool EnableKill { get; set; } = true;

        public bool EnableNuke { get; set; } = true;

        public bool EnableRespawnWave { get; set; } = true;

        public bool EnableRoundRestart { get; set; } = true;

        [Description("Cambia la duración de la votación.")]
        public int VoteDuration { get; set; } = 30;

        [Description("Cambia el número máximo de votaciones que puede iniciar cada jugador en una partida.")]
        public int MaxAmountOfVotesPerRound { get; set; } = 10;

        [Description("Cambia el tiempo que debe transcurrir desde el inicio de la ronda antes de poder iniciar una votación de cada módulo.")]
        public float MaxWaitFf { get; set; } = 0;

        public float MaxWaitKill { get; set; } = 0;

        public float MaxWaitKick { get; set; } = 0;

        public float MaxWaitNuke { get; set; } = 0;

        public float MaxWaitRespawnWave { get; set; } = 0;

        public float MaxWaitRestartRound { get; set; } = 0;

        [Description("Cambia el porcentaje necesario para aprobar la votación de cada módulo.")]
        public int ThresholdKick { get; set; } = 30;

        public int ThresholdFf { get; set; } = 30;

        public int ThresholdKill { get; set; } = 30;

        public int ThresholdNuke { get; set; } = 30;

        public int ThresholdRespawnWave { get; set; } = 30;

        public int ThresholdRestartRound { get; set; } = 30;

        [Description("Cambia el tamaño del mensaje de Callvote. (0 = cálculo automático del tamaño predeterminado de Callvote).")]
        public int MessageSize { get; set; } = 0;

        [Description("Cambia el intervalo de actualización del mensaje.")]
        public float RefreshInterval { get; set; } = 1f;

        [Description("Cambia cuánto tiempo se muestra el mensaje con los resultados de Callvote.")]
        public int FinalResultsDuration { get; set; } = 5;

        [Description("Cambia el tamaño de la cola de Callvote, si está activada.")]
        public int QueueSize { get; set; } = 5;

        [Description("Añade un webhook de Discord para enviar los resultados de las votaciones.")]
        public string DiscordWebhook { get; set; } = string.Empty;

        [Description("Si DiscordLab o SCPDiscord están instalados, establece un ID de canal válido para enviar allí los resultados. 0 = desactivado.")]
        public ulong DiscordChannelId { get; set; } = 0;

        [Description("Ajustes relacionados con el menú SS de Callvote.")]
        public bool EnableSSMenu { get; set; } = true;

        public int HeaderSettingId { get; set; } = 887;

        public int YesKeybindSettingId { get; set; } = 888;

        public int NoKeybindSettingId { get; set; } = 889;

        public int MtfKeybindSettingId { get; set; } = 890;

        public int CiKeybindSettingId { get; set; } = 891;
    }
}
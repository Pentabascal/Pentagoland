using System.ComponentModel;

namespace VVUP.ServerEvents.ServerEventsConfigs
{

    public class FreezingTemperaturesConfig
    {
        public string StartEventCassieMessage { get; set; } = "Temperature Failure";
        public string StartEventCassieText { get; set; } = "Fallo de temperatura (evento de congelación de las zonas)";

        [Description("¿Cuánto debe pasar antes del aviso de tiempo de la zona de contención ligera?")]
        public float LightTimeWarning { get; set; } = 300f;

        [Description(
            "¿Cuántos segundos después del aviso debe tardar en congelarse por completo la zona de contención ligera?")]
        public float LightCompleteFreezeTime { get; set; } = 300f;

        [Description("¿Qué aviso debe mostrarse cuando quede la mitad del tiempo en la zona de contención ligera?")]
        public string LightHalfTimeRemainingWarningMessage { get; set; } =
            "5 Minutes before Light Containment Zone Temperature Failure";

        [Description("¿Qué texto debe acompañar al aviso de tiempo restante de la zona de contención ligera?")]
        public string LightHalfTimeRemainingWarningText { get; set; } =
            "Quedan 5 minutos para el fallo de temperatura en la zona de contención ligera.";

        [Description("¿Qué debe decir CASSIE cuando se congele la zona ligera?")]
        public string LightFrozenOverMessage { get; set; } = "Light Containment Zone has been locked down";

        [Description("¿Qué texto debe acompañar al anuncio de CASSIE cuando se congele la zona ligera?")]
        public string LightFrozenOverText { get; set; } = "Light Containment Zone has been locked down";

        [Description("¿Cuánto debe pasar antes del aviso de tiempo de la zona de contención pesada?")]
        public float HeavyTimeWarning { get; set; } = 150f;

        [Description(
            "¿Cuántos segundos después de congelarse la zona de contención ligera debe tardar en congelarse por completo la zona de contención pesada?")]
        public float HeavyCompleteFreezeTime { get; set; } = 150f;

        [Description("¿Qué aviso debe mostrarse cuando quede la mitad del tiempo en la zona de contención pesada?")]
        public string HeavyHalfTimeRemainingWarningMessage { get; set; } =
            "Quedan 2 minutos y 30 segundos para el fallo de temperatura en la zona de contención pesada.";

        [Description("¿Qué texto debe acompañar al aviso de tiempo restante de la zona de contención pesada?")]
        public string HeavyHalfTimeRemainingWarningText { get; set; } =
            "Quedan 2 minutos y 30 segundos para el fallo de temperatura en la zona de contención pesada.";

        [Description("¿Qué debe decir CASSIE cuando se congele la zona pesada?")]
        public string HeavyFrozenOverMessage { get; set; } = "Heavy Containment Zone has been locked down";

        [Description("¿Qué texto debe acompañar al anuncio de CASSIE cuando se congele la zona pesada?")]
        public string HeavyFrozenOverText { get; set; } = "Heavy Containment Zone has been locked down";

        [Description("¿Cuánto debe pasar antes del aviso de tiempo de la zona de entrada?")]
        public float EntranceTimeWarning { get; set; } = 150f;

        [Description(
            "¿Cuántos segundos después de congelarse la zona de contención pesada debe tardar en congelarse por completo la zona de entrada?")]
        public float EntranceCompleteFreezeTime { get; set; } = 150f;

        [Description("¿Qué aviso debe mostrarse cuando quede la mitad del tiempo en la zona de entrada?")]
        public string EntranceHalfTimeRemainingWarningMessage { get; set; } =
            "Quedan 2 minutos y 30 segundos para el fallo de temperatura en la zona de entrada.";

        [Description("¿Qué texto debe acompañar al aviso de tiempo restante de la zona de entrada?")]
        public string EntranceHalfTimeRemainingWarningText { get; set; } =
            "Quedan 2 minutos y 30 segundos para el fallo de temperatura en la zona de entrada.";

        [Description("¿Qué debe decir CASSIE cuando se congele la zona de entrada?")]
        public string EntranceFrozenOverMessage { get; set; } = "Entrance Zone has been locked down";

        [Description("¿Qué texto debe acompañar al anuncio de CASSIE cuando se congele la zona de entrada?")]
        public string EntranceFrozenOverText { get; set; } = "Entrance Zone has been locked down";

        [Description(
            "¿Cuántos segundos después del cierre de las zonas debe tardar en morir el jugador, por si consigue sobrevivir allí por algún motivo?")]
        public float KillPlayersInZoneAfterTime { get; set; } = 10f;

        [Description("¿Qué motivo de muerte se debe mostrar si el servidor mata al jugador?")]
        public string PlayersDeathReason { get; set; } = "Has muerto congelado.";

    }
}
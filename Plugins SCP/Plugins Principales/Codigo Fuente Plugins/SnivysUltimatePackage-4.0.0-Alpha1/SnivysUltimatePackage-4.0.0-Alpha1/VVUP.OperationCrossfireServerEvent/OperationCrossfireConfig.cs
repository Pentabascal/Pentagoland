using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using VVUP.OperationCrossfireServerEvent.Items;

namespace VVUP.OperationCrossfireServerEvent
{
    public class OperationCrossfireConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public string StartEventCassieMessage { get; set; } = "Military Sim Started";
        public string StartEventCassieText { get; set; } = "\nSimulación militar (Operación Crossfire)\nConsulta los objetivos en la parte superior de la pantalla.";
        
        [Description("¿Cada cuánto debe comprobar el evento si hay novedades? (en segundos)")]
        public float CheckForEventsInterval { get; set; } = 1f;
        
        [Description("Proporción de MTF y científicos que aparecerán durante el evento. El resto serán Clase-D. La suma debe ser 1.")]
        public float MtfRatio { get; set; } = 0.5f;
        public float ScientistRatio { get; set; } = 0.25f;

        [Description("Mensaje que se mostrará a quienes se conecten durante el evento.")]
        public string PlayerConnectDuringEventMessage { get; set; } =
            "Hay un evento en curso; espera un momento.";
        [Description("¿Durante cuánto tiempo se mostrará este mensaje?")]
        public float PlayerConnectDuringEventMessageDisplayDuration { get; set; } = 10f;

        public uint PrototypeKeycardBasicId { get; set; } = 44;

        public float ScientistPercentageRequiredToWin { get; set; } = 0.5f;
        
        public float MtfPercentageRequiredToWin { get; set; } = 0.2f;

        public string MtfScientistWinMessage { get; set; } =
            "Los MTF y los científicos han cumplido sus objetivos.";

        public string ClassDWinMessage { get; set; } =
            "Los Clase-D han causado suficientes problemas a los MTF y a los científicos.";
        public string TieMessage { get; set; } = "Ninguno de los bandos ha logrado cumplir sus objetivos.";

        [Description("¿Durante cuánto tiempo se mostrará el mensaje de victoria?")]
        public int EndOfRoundTime { get; set; } = 10;
        [Description("¿Cuánto debe durar el evento? (Se utiliza el temporizador de la bomba nuclear).")]
        public int EventDuration { get; set; } = 1800;

        public string MtfScientistObjective1 { get; set; } = "Encuentra y mejora la tarjeta de acceso prototipo";
        public string MtfScientistObjective2 { get; set; } = "Desbloquea SCP-914";
        public string MtfObjective3 { get; set; } = "Escolta a los científicos fuera de las instalaciones";
        public string ScientistObjective3 { get; set; } = "Escapa de las instalaciones";
        public string ClassDObjective1 { get; set; } = "Encuentra y conserva la tarjeta de acceso prototipo";
        public string ClassDObjective2 { get; set; } = "Elimina a los MTF y toma científicos como rehenes";

        public int StartingBroadcastTime { get; set; } = 60;
        
        [Description("¿Con qué armas deben aparecer los Clase-D? (Se elegirá una al azar).")]
        public List<ItemType> ClassDFirearms { get; set; } = new List<ItemType>
        {
            ItemType.GunAK,
            ItemType.GunShotgun,
            ItemType.GunRevolver,
            ItemType.GunLogicer,
        };
        [Description("¿Con qué tarjeta de acceso deben aparecer los Clase-D?")]
        public ItemType ClassDKeycard { get; set; } = ItemType.KeycardMTFPrivate;

        [Description("Mensaje que se mostrará si los Clase-D no pueden atacar a científicos desarmados.")]
        public string ClassDScientistHostagesUnableToHarm { get; set; } =
            "Los científicos son vuestros rehenes; será mejor mantenerlos con vida.";
        [Description("Mensaje que se mostrará si los Clase-D pueden atacar a científicos desarmados.")]
        public string ClassDScientistsNowAreTargets { get; set; } =
            "Los científicos ya son vulnerables; acabad con ellos.";
        
        public List<OcfPrototypeKeycardBasic> OcfPrototypeKeycardBasics { get; private set; } = new()
        {
            new OcfPrototypeKeycardBasic()
        };

        public List<OcfPrototypeKeycardRefined> OcfPrototypeKeycardRefineds { get; private set; } = new()
        {
            new OcfPrototypeKeycardRefined()
        };
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;

namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class ChaoticConfig
    {
        public string StartEventCassieMessage { get; set; } = "Instalaciones inestables";
        public string StartEventCassieText { get; set; } = "Instalaciones inestables (Evento caótico)";

        [Description("Determina cuánto falta para el siguiente evento caótico (en segundos).")]
        public float TimeForChaosEvent { get; set; } = 60;

        [Description("¿Debe volver a sortearse el evento caótico si un evento concreto está desactivado?")]
        public bool ChaoticEventRerollIfASpecificEventIsDisabled { get; set; } = true;

        [Description(
            "Duración de los avisos en pantalla (se aplica a todos los avisos del evento caótico).")]
        public float BroadcastDisplayTime { get; set; } = 10;

        [Description("¿Debe activarse el evento de robo de objetos?")]
        public bool ItemStealEvent { get; set; } = true;

        [Description("Mensaje que se mostrará cuando roben los objetos de un jugador.")]
        public string ItemStolenText { get; set; } = "Parece que te han robado los objetos.";

        [Description("¿Debe activarse el evento de entrega de objetos aleatorios?")]
        public bool GiveRandomItemEvent { get; set; } = true;

        [Description("Mensaje que se mostrará al entregar un objeto.")]
        public string GiveRandomItemText { get; set; } = "Parece que has recibido un objeto.";

        [Description("¿Se deben entregar solo objetos normales o también objetos personalizados?")]
        public bool GiveRandomItemCustomitems { get; set; } = false;

        [Description("¿Debe activarse el evento de teletransporte aleatorio?")]
        public bool RandomTeleportEvent { get; set; } = true;

        [Description(
            "Si el evento de teletransporte aleatorio está activo, ¿se debe teletransportar a los jugadores a la zona ligera después de la descontaminación?")]
        public bool TeleportToLightAfterDecom { get; set; } = false;

        [Description(
            "Si el evento de teletransporte aleatorio está activo, ¿se debe teletransportar a los jugadores a las instalaciones después de detonar la ojiva?")]
        public bool TeleportToFacilityAfterNuke { get; set; } = false;

        [Description("Mensaje que se mostrará al teletransportar a un jugador.")]
        public string RandomTeleportText { get; set; } = "Vaya, ha sido culpa mía.";

        [Description("¿Debe activarse el evento de falsa detonación de la bomba nuclear?")]
        public bool FakeAutoNuke { get; set; } = true;

        [Description(
            "Mensaje que se mostrará al comenzar la falsa cuenta atrás nuclear (debería coincidir con el aviso real para que resulte convincente).")]
        public string FakeAutoNukeStartText { get; set; } = "Ha comenzado la cuenta atrás de la bomba nuclear";

        [Description("Mensaje que se mostrará al revelar que la cuenta atrás nuclear era falsa.")]
        public string FakeAutoNukeFakeoutText { get; set; } = "¡Era una broma!";

        [Description(
            "Tiempo antes de revelar que la bomba nuclear no va a detonar. Ten en cuenta que no funcionará con menos de 10 segundos.")]
        public float FakeAutoNukeTimeOut { get; set; } = 15f;

        [Description("¿Se debe restaurar el tiempo anterior de la bomba nuclear al revelar la broma? Si no, se establecerá en 30 segundos.")]
        public bool FakeAutoNukeRestoresOldTime { get; set; } = true;

        [Description("¿Debe activarse el evento de retirada de armas?")]
        public bool RemoveWeaponsEvent { get; set; } = true;

        [Description("Mensaje que se mostrará al retirar un arma.")]
        public string RemoveWeaponsText { get; set; } = "Espero que no pensaras meterte en una pelea ahora.";

        [Description("¿Debe activarse el evento de entrega de armas aleatorias?")]
        public bool GiveRandomWeaponsEvent { get; set; } = true;

        [Description("¿Las armas aleatorias solo deben entregarse a quienes no tengan armas?")]
        public bool GiveRandomWeaponsToUnarmedPlayers { get; set; } = true;

        [Description("Mensaje que se mostrará al entregar un arma.")]
        public string GiveRandomWeaponsText { get; set; } = "Toma, aquí tienes un arma.";

        [Description("Mensaje que se mostrará si el inventario del jugador está lleno y no puede recibir un arma.")]
        public string GiveRandomWeaponsTextFail { get; set; } = "No hay armas libres para ti. ¡Bawomp!";

        [Description("¿Se pueden entregar todas las armas?")]
        public bool GiveAllRandomWeapons { get; set; } = true;

        [Description("Si no se pueden entregar todas las armas, ¿cuáles se pueden elegir?")]
        [CanBeNull]
        public List<ItemType> GiveRandomWeaponsDefined { get; set; } = new()
        {
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCrossvec,
        };

        [Description("¿Debe activarse el evento de combate a muerte?")]
        public bool DeathMatchEvent { get; set; } = true;

        [Description("Mensaje que se mostrará al reducir la salud de los jugadores.")]
        public string DeathMatchText { get; set; } = "*Estornuda* Perdón, espero no haber estropeado nada.";

        [Description("¿También deben verse afectados los SCP?")]
        public bool DeathMatchEventAffectsSCPs { get; set; } = false;

        [Description("¿A qué valor se debe establecer la salud de los jugadores?")]
        public float DeathMatchHealth { get; set; } = 1f;

        [Description("¿El evento caótico puede activar otros eventos?")]
        public bool ChaosEventEnablesOtherEvents { get; set; } = true;

        [Description("¿Deben terminar los demás eventos activados cuando comience el siguiente ciclo?")]
        public bool ChaosEventEndsOtherEvents { get; set; } = false;

        [Description("¿Debe activarse el evento «¡FBI, abre!»?")]
        public bool FBIOpenUpEvent { get; set; } = true;

        [Description("Mensaje que se mostrará al objetivo que va a ser teletransportado.")]
        public string FBIOpenUpTargetText { get; set; } =
            "El FBI se acerca rápidamente a tu ubicación. ¡Prepárate!";

        [Description("Mensaje que se mostrará a los MTF y guardias que serán teletransportados hasta un objetivo.")]
        public string FBIOpenUpMTFText { get; set; } = "Lucha o huye.";

        [Description("¿Cuánto debe transcurrir antes de teletransportar a las fuerzas de la Fundación hasta el objetivo?")]
        public float FBITeleportTime { get; set; } = 5f;

        [Description("¿Debe activarse el evento de granadas bajo los pies?")]
        public bool GrenadeFeetEvent { get; set; } = true;

        [Description("¿La mecha de las granadas debe ser aleatoria?")]
        public bool GrenadeFeetRandomFuse { get; set; } = true;

        [Description("¿Cuánto debe durar la mecha de las granadas si no es aleatoria?")]
        public float GrenadeFeetFuse { get; set; } = 5f;

        [Description("Mensaje que se mostrará antes de que caigan granadas sobre los jugadores.")]
        public string GrenadeFeetText { get; set; } = "Mira bien dónde pisas.";

        [Description("¿Debe activarse el evento de objetos médicos peligrosos?")]
        public bool UnsafeMedicalItemsEvent { get; set; } = true;

        [Description("Mensaje que se mostrará para avisar de que los objetos médicos son peligrosos.")]
        public string UnsafeMedicalItemsText { get; set; } = "Usa los objetos médicos con precaución.";

        [Description("¿La duración del evento de objetos médicos peligrosos debe ser aleatoria?")]
        public bool UnsafeMedicalItemsUseRandomTime { get; set; } = true;

        [Description("Si la duración aleatoria está desactivada, ¿cuánto debe durar el evento de objetos médicos peligrosos?")]
        public float UnsafeMedicalItemsFixedTime { get; set; } = 30f;

        [Description("Mensaje que avisará a los jugadores de que ya pueden volver a usar objetos médicos.")]
        public string UnsafeMedicalItemsSafeToUseText { get; set; } = "Ya puedes volver a usar objetos médicos con seguridad.";

        [Description("¿Deben activarse los avisos falsos de oleadas de reaparición?")]
        public bool FakeoutRespawnAnnouncementsEvent { get; set; } = true;

        [Description("¿Se deben usar avisos falsos para los MTF?")]
        public bool FakeoutRespawnAnnouncementsMTFAllow { get; set; } = true;

        [Description("¿Se deben usar avisos falsos para la Insurgencia del Caos?")]
        public bool FakeoutRespawnAnnouncementsChaosAllow { get; set; } = false;

        [Description("¿Se deben usar avisos falsos para la Mano de la Serpiente?")]
        public bool FakeoutRespawnAnnouncementsSerpentsAllow { get; set; } = false;

        [Description("¿Se deben usar avisos falsos para la UIU?")]
        public bool FakeoutRespawnAnnouncementsUIUAllow { get; set; } = false;

        [Description("¿Se debe usar el aviso de los MTF como alternativa si los demás avisos falsos están desactivados?")]
        public bool FakeoutRespawnAnnouncementsMTFFallback { get; set; } = false;

        [Description("¿Qué debe decir el anuncio CASSIE falso de los MTF?")]
        public string FakeoutRespawnAnnouncementsMTFAliveSCPSCassie { get; set; } =
            "MTFUnit epsilon 11 designated {designation} hasentered AwaitingRecontainment {scpnum}";

        public string FakeoutRespawnAnnouncementsMTFSCPSDeadCassie { get; set; } =
            "MTFUnit epsilon 11 designated {designation} hasentered NoSCPsLeft";

        [Description("¿Qué texto debe acompañar al anuncio CASSIE falso de los MTF?")]
        public string FakeoutRespawnAnnouncementsMTFAliveSCPSCassieText { get; set; } =
            "La Unidad de Fuerzas Móviles Epsilon 11, designación {designation}, ha entrado en las instalaciones. Se recomienda a todo el personal restante seguir los protocolos de evacuación habituales hasta que llegue un escuadrón de los MTF. A la espera de la recontención de {scpnum}.";

        public string FakeoutRespawnAnnouncementsMTFSCPSDeadCassieText { get; set; } =
            "La Unidad de Fuerzas Móviles Epsilon 11, designación {designation}, ha entrado en las instalaciones. Se recomienda a todo el personal restante seguir los protocolos de evacuación habituales hasta que llegue un escuadrón de los MTF. Aún quedan amenazas importantes en las instalaciones; proceda con precaución.";

        [Description("¿Qué debe decir el anuncio CASSIE falso de la Insurgencia del Caos?")]
        public string FakeoutRespawnAnnouncementsChaosCassie { get; set; } =
            "Warning . Military Personnel has entered the facility . Designated as, Chaos Insurgency.";

        [Description("¿Qué texto debe acompañar al anuncio CASSIE falso de la Insurgencia del Caos?")]
        public string FakeoutRespawnAnnouncementsChaosCassieText { get; set; } =
            "Aviso. Personal militar ha entrado en las instalaciones. Identificado como <color=green>Insurgencia del Caos</color>.";

        [Description("¿Qué debe decir el anuncio CASSIE falso de la Mano de la Serpiente?")]
        public string FakeoutRespawnAnnouncementsSerpentsCassie { get; set; } = "Serpents Hand hasentered";

        [Description("¿Qué texto debe acompañar al anuncio CASSIE falso de la Mano de la Serpiente?")]
        public string FakeoutRespawnAnnouncementsSerpentsCassieText { get; set; } =
            "La Mano de la Serpiente ha entrado en las instalaciones.";

        [Description("¿Qué debe decir el anuncio CASSIE falso de la UIU?")]
        public string FakeoutRespawnAnnouncementsUIUAliveSCPSCassie { get; set; } =
            "The U I U Squad designated {designation} HasEntered AllRemaining AwaitingRecontainment {scpnum}";

        public string FakeoutRespawnAnnouncementsUIUSCPSDeadCassie { get; set; } =
            "The U I U Squad designated {designation} HasEntered AllRemaining NoSCPsLeft";

        [Description("¿Qué texto debe acompañar al anuncio CASSIE falso de la UIU?")]
        public string FakeoutRespawnAnnouncementsUIUAliveSCPSCassieText { get; set; } =
            "El escuadrón de la UIU, designación {designation}, ha entrado en las instalaciones. A la espera de la recontención de {scpnum}.";

        public string FakeoutRespawnAnnouncementsUIUSCPSDeadCassieText { get; set; } =
            "El escuadrón de la UIU, designación {designation}, ha entrado en las instalaciones. Aún quedan amenazas importantes; proceda con precaución.";

        [Description("¿Debe activarse el evento de disparo rápido de la puerta Tesla?")]
        public bool RapidFireTelsaEvent { get; set; } = true;

        [Description("¿Cuánto debe durar el evento (en segundos)?")]
        public float RapidFireTeslaEventTiming { get; set; } = 30f;

        [Description("¿Cuál debe ser el tiempo de activación de la puerta Tesla de disparo rápido?")]
        public float RapidFireTeslaEventActivationTime { get; set; } = 0.1f;

        [Description("¿Cuál debe ser el alcance de detección en reposo de la puerta Tesla de disparo rápido?")]
        public float RapidFireTeslaEventIdleRange { get; set; } = 1000f;

        [Description("¿Cuál debe ser el alcance de activación de la puerta Tesla de disparo rápido?")]
        public float RapidFireTeslaEventTriggerRange { get; set; } = 500f;

        [Description("¿Cuál debe ser el tiempo de recarga de la puerta Tesla de disparo rápido?")]
        public float RapidFireTeslaEventCooldownTime { get; set; } = 0.1f;

        [Description("¿Debe activarse el evento de jugadores que se cagan encima?")]
        public bool PlayerShittingPantsEvent { get; set; } = true;

        [Description("¿Qué aviso se debe mostrar cuando un jugador se cague encima?")]
        public string PlayerShittingPantsBroadcast { get; set; } = "¡Mierda, me he cagado encima!";

        [Description("¿Debe activarse el evento del simulador de patadas al router?")]
        public bool RouterKickingSimulatorEvent { get; set; } = true;

        [Description("¿Cuántas patadas debe recibir el router?")]
        public int RouterKickingSimulatorRouterKickAmount { get; set; } = 10;

        [Description("¿Cuánto tiempo debe pasar entre cada patada al router?")]
        public float RouterKickingSimulatorTimeBetweenRouterKicks { get; set; } = 10f;

        [Description("¿Cuánto debe durar el lag que sufren los jugadores por cada patada al router?")]
        public float RouterKickingSimulatorLagTime { get; set; } = 3f;

        [Description("¿Qué aviso se debe mostrar cuando empiecen a dar patadas al router del jugador?")]
        public string RouterKickingSimulatorStartBroadcast { get; set; } =
            "Perdona, ya me he cansado un poco de tu router.";

        [Description("¿Qué aviso se debe mostrar al jugador cuando termine el evento?")]
        public string RouterKickingSimulatorEndBroadcast { get; set; } =
            "Ya le he dado suficientes patadas a tu router. Perdona.";

        [Description("¿Cuánto debe durar la espera **ANTES** de que empiece el simulador de patadas al router?")]
        public float RouterKickingSimulatorStartWaitTime { get; set; } = 15f;

        [Description(
            "¿Debe desactivar los ascensores el simulador de patadas al router (para evitar que los jugadores se teletransporten al vacío)?")]
        public bool RouterKickingSimulatorDisablesElevators { get; set; } = true;

        [Description("¿Se deben desactivar eventos como la descontaminación y la ojiva durante este evento?")]
        public bool RouterKickingSimulatorDisablesDecomAndNuke { get; set; } = true;

        [Description("¿Debe activarse el evento de supervelocidad?")]
        public bool SuperSpeedEvent { get; set; } = true;

        [Description("¿Cuánto debe durar la supervelocidad (en segundos)?")]
        public float SuperSpeedDuration { get; set; } = 15;

        [Description("¿Qué intensidad debe tener la supervelocidad? (Máximo: 255)")]
        public byte SuperSpeedIntensity { get; set; } = 200;

        [Description("¿Qué aviso se debe mostrar a los jugadores durante la supervelocidad?")]
        public string SuperSpeedBroadcast { get; set; } = "¡Hay que ir rápido!";
        
        [Description("¿Debe activarse el evento de soltar el inventario?")]
        public bool DropInventoryEvent { get; set; } = true;
        
        [Description("¿Qué aviso se debe mostrar cuando un jugador suelte su inventario?")]
        public string DropInventoryText { get; set; } = "Notas un agujero enorme en tus bolsillos.";
        
        [Description("¿Debe activarse el evento de teletransporte junto a un aliado?")]
        public bool TeleportToAlliesEvent { get; set; } = true;
        
        [Description("¿Qué aviso se debe mostrar al teletransportar a un jugador junto a un aliado?")]
        public string TeleportToAllyPlayerTeleportedText { get; set; } = "Te han teletransportado junto a un aliado.";

        [Description("¿Qué aviso se debe mostrar al teletransportar a un aliado junto a un jugador?")]
        public string TeleportToAllyNewAllyText { get; set; } = "Han teletransportado a un aliado junto a ti.";

        [Description("¿Debe activarse el evento de «Simón dice»?")]
        public bool SimonSaysEvent { get; set; } = true;

        [Description("¿Cuánto tiempo tendrá el jugador para realizar la acción?")]
        public float SimonSaysActionTime { get; set; } = 5;

        [Description("¿También deben verse afectados los SCP por «Simón dice»?")]
        public bool SimonSaysAffectsScps { get; set; } = false;
        
        [Description("¿Qué texto debe mostrarse para «Simón dice: salta»?")]
        public string SimonSaysJumpText { get; set; } = "Simón dice: salta";

        [Description("¿Qué texto debe mostrarse para «Alguien dice: salta»?")]
        public string SomeoneSaysJumpText { get; set; } = "Alguien dice: salta";

        [Description("¿Qué daño se debe infligir si alguien no salta cuando lo dice Simón o salta cuando lo dice otra persona?")]
        public int SimonSaysDamagePunishment { get; set; } = 50;

        [Description("¿Qué texto se debe mostrar si alguien falla en el evento de «Simón dice»?")]
        public string SimonSaysPunishmentText { get; set; } = "No has saltado cuando lo ha dicho Simón.";
        
        [Description("¿Qué texto se debe mostrar si alguien falla en el evento de «Alguien dice»?")]
        public string SomeoneSaysPunishmentText { get; set; } = "Has saltado aunque Simón no lo ha dicho.";
    }
}
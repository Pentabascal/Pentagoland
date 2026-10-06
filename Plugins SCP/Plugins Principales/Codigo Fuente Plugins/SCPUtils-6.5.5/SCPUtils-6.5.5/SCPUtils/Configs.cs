using Exiled.API.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using DamageTypes = Exiled.API.Enums.DamageType;
using Log = Exiled.API.Features.Log;
using ZoneType = Exiled.API.Enums.ZoneType;

namespace SCPUtils
{
    public class Configs : IConfig
    {
        [Description("Configuración de la base de datos - Nombre de la base de datos")]
        public string DatabaseName { get; private set; } = "ScpUtils";

        [Description("IP de la base de datos - Dirección IP donde está alojada")]
        public string DatabaseIp { get; private set; } = "localhost";

        [Description("Puerto de la base de datos - No lo cambies salvo que sea necesario")]
        public ushort DatabasePort { get; private set; } = 27017;

        [Description("Usuario de la base de datos")]
        public string DatabaseUser { get; private set; } = "user";

        [Description("Contraseña de la base de datos - Si la usas en local, puede estar vacía")]
        public string DatabasePassword { get; private set; } = "";

        [Description("Mecanismo de autenticación de la base de datos")]
        public string DatabaseAuthType { get; private set; } = "SCRAM-SHA-256";

        [Description("Cadena de conexión personalizada - Si se modifica, se ignorarán los demás parámetros. Solo para usuarios avanzados")]
        public string CustomConnectionString { get; private set; } = "none";

        [Description("Configuración del plugin")]
        public bool IsEnabled { get; set; } = true;

        [Description("¿Se debe advertir a los SCP por abandonar la partida o suicidarse?")]
        public bool EnableSCPSuicideAutoWarn { get; private set; } = true;

        [Description("¿Se debe reiniciar automáticamente la ronda cuando solo quede un jugador?")]
        public bool EnableRoundRestartCheck { get; private set; } = true;

        [Description("¿Se considera motivo de advertencia abandonar la partida jugando como SCP?")]
        public bool QuitEqualsSuicide { get; private set; } = true;

        [Description("¿Se debe expulsar a los SCP por abandonar la partida o suicidarse al superar cierto límite?")]
        public bool AutoKickOnSCPSuicide { get; private set; } = true;

        [Description("¿Se debe vetar a los SCP por abandonar la partida o suicidarse al superar cierto límite?")]
        public bool EnableSCPSuicideAutoBan { get; private set; } = false;

        [Description("¿Se debe impedir que los SCP vuelvan a jugar como SCP (excepto 049-2) durante X rondas al superar cierto límite? Si se activa, EnableSCPSuicideAutoBan debe estar desactivado")]
        public bool EnableSCPSuicideSoftBan { get; private set; } = true;

        [Description("¿Se debe multiplicar la duración del veto con cada nuevo veto por abandonar la partida o suicidarse como SCP?")]
        public bool MultiplyBanDurationEachBan { get; private set; } = true;

        [Description("¿Se deben restablecer las preferencias del jugador cuando caduque su insignia?")]
        public bool ResetPreferencedOnBadgeExpire { get; private set; } = true;

        [Description("¿Se debe expulsar automáticamente a un jugador si tiene el juego vetado?")]
        public bool AutoKickBannedNames { get; private set; } = true;

        [Description("¿Se deben considerar los Tutoriales como SCP? Si se activa, recibirán advertencias por suicidarse o abandonar la partida, si está habilitado")]
        public bool AreTutorialsSCP { get; private set; } = false;

        [Description("Enviar al chat de administración avisos de expulsiones y vetos automáticos por abandonar la partida o suicidarse como SCP")]
        public bool BroadcastSanctions { get; private set; } = true;

        [Description("Enviar al chat de administración avisos automáticos por abandonar la partida o suicidarse como SCP")]
        public bool BroadcastWarns { get; private set; } = false;

        [Description("¿Se debe restablecer el apodo al conectarse si el jugador no tiene permiso para usar el comando? Se puede evitar con el comando scputils_preference_persist")]
        public bool KeepNameWithoutPermission { get; private set; } = false;

        [Description("¿Se debe restablecer el color al conectarse si el jugador no tiene permiso para usar el comando? Se puede evitar con el comando scputils_preference_persist")]
        public bool KeepColorWithoutPermission { get; private set; } = false;

        [Description("¿Se debe restablecer la visibilidad de la insignia al conectarse si el jugador no tiene permiso para usar el comando? Se puede evitar con el comando scputils_preference_persist")]
        public bool KeepBadgeVisibilityWithoutPermission { get; private set; } = false;

        [Description("¿Está habilitada la función de aviso a los objetivos de SCP-096?")]
        public bool Scp096TargetNotifyEnabled { get; private set; } = true;

        [Description("¿Avisar al último jugador con vida?")]
        public bool NotifyLastPlayerAlive { get; private set; } = true;

        [Description("¿Ignorar las solicitudes DNT?")]
        public bool IgnoreDntRequests { get; private set; } = false;

        [Description("¿Habilitar el módulo de reinicio automático?")]
        public bool EnableAutoRestart { get; private set; } = false;

        [Description("¿Silenciar automáticamente al jugador si se detecta una evasión?")]
        public bool AutoMute { get; private set; } = false;

        [Description("¿Mostrar un aviso si se detectan varias cuentas?")]
        public bool MultiAccountBroadcast { get; private set; } = false;

        [Description("¿Solo el propietario de las esposas (y los aliados de los jugadores esposados) puede quitar las esposas?")]
        public bool HandCuffOwnership { get; private set; } = false;

        [Description("¿Permitir el intercambio de SCP?")]
        public bool AllowSCPSwap { get; private set; } = true;
        [Description("¿Permitir el intercambio de SCP solo si ambos tienen toda la salud? También debes habilitar la opción allow_scp_swap.")]
        public bool AllowSCPSwapOnlyFullHealth { get; private set; } = true;

        [Description("¿Informar en la consola del uso abusivo de comandos?")]
        public bool CommandAbuseReport { get; private set; } = true;

        [Description("¿Mostrar un aviso cuando muera SCP-049-2?")]
        public bool Scp0492DeathBroadcast { get; private set; } = false;

        [Description("¿Permitir reaparecer solo con toda la salud?")]
        public bool RespawnOnlyFullHealth { get; private set; } = true;

        [Description("Destruir los objetos soltados durante el periodo de reaparición")]
        public bool RespawnWindowDestroyItem { get; private set; } = true;

        [Description("¿Mostrar el mensaje informativo de reaparición como pista en lugar de como aviso?")]
        public bool RespawnInformativeHint { get; private set; } = true;

        [Description("¿Permitir etiquetas arcoíris? El jugador debe tener permiso para cambiar el color o un administrador debe asignárselo con el comando \"pp\".")]
        public bool AllowRainbowTags { get; private set; } = true;

        [Description("¿Informar de las evasiones de silencio?")]
        public bool ReportMuteEvasions { get; private set; } = true;

        [Description("Mensaje de advertencia automática por suicidarse como SCP")]
        public Exiled.API.Features.Broadcast SuicideWarnMessage { get; private set; } = new Exiled.API.Features.Broadcast("<color=red>ADVERTENCIA:\nSegún las normas del servidor, suicidarse como SCP es una infracción. Si lo haces demasiadas veces, recibirás un veto.</color>", 30, true, Broadcast.BroadcastFlags.Normal);

        [Description("Mensaje de bienvenida (si está habilitado)")]
        public Exiled.API.Features.Broadcast WelcomeMessage { get; private set; } = new Exiled.API.Features.Broadcast("<color=green>¡Te damos la bienvenida al servidor, %player%!</color>", 12, true, Broadcast.BroadcastFlags.Normal);

        [Description("Mensaje de descontaminación (si está habilitado)")]
        public Exiled.API.Features.Broadcast DecontaminationMessage { get; private set; } = new Exiled.API.Features.Broadcast("<color=yellow>Ha comenzado la descontaminación</color>", 12, false, Broadcast.BroadcastFlags.Normal);

        [Description("Motivo de expulsión automática por suicidio (si está habilitado)")]
        public string SuicideKickMessage { get; private set; } = "Suicidarse como SCP";

        [Description("Mensaje de expulsión automática por usar un apodo restringido")]
        public string AutoKickBannedNameMessage { get; private set; } = "Estás usando un apodo restringido o demasiado parecido a uno. Cámbialo, por favor.";

        [Description("Motivo de veto automático por suicidio (si está habilitado)")]
        public string AutoBanMessage { get; private set; } = "Has superado el límite de suicidios como SCP";

        [Description("Mensaje si el jugador no tiene permiso para usar este comando")]
        public string UnauthorizedNickNameChange { get; private set; } = "<color=red>No tienes permiso.</color>";

        [Description("Mensaje si el jugador no tiene permiso para usar este comando")]
        public string UnauthorizedColorChange { get; private set; } = "<color=red>No tienes permiso.</color>";

        [Description("Mensaje si el jugador no tiene permiso para usar este comando")]
        public string UnauthorizedBadgeChangeVisibility { get; private set; } = "<color=red>No tienes permiso.</color>";

        [Description("Mensaje si el jugador no tiene permiso para usar este comando")]
        public string NicknameCooldownMessage { get; private set; } = "<color=red>Este comando tiene un tiempo de espera. Vuelve a intentarlo dentro de unos minutos.</color>";

        [Description("Mensaje si el jugador intenta cambiar su apodo por uno restringido")]
        public string InvalidNicknameText { get; private set; } = "El propietario del servidor ha restringido este apodo. ¡Usa otro, por favor!";

        /*[Description("Database name, change it only if you are running multiple servers")]
        public string DatabaseName { get; private set; } = "SCPUtils";

        [Description("In which folder database should be stored?")]
        public string DatabaseFolder { get; private set; } = "EXILED";*/


        [Description("URL del webhook de Discord para informar de evasiones de silencio")]
        public string WebhookUrl { get; private set; } = "None";

        [Description("Apodo del bot del webhook de Discord")]
        public string WebhookNickname { get; private set; } = "The Frontman";

        [Description("Aviso que se muestra cuando muere un SCP")]
        public Exiled.API.Features.Broadcast ScpDeathMessage { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>SCP %playername% (%scpname%) ha muerto a manos de %killername%. Causa de la muerte: %reason%</color>", 12, true, Broadcast.BroadcastFlags.Normal);

        [Description("Aviso que se muestra cuando un SCP se suicida")]
        public Exiled.API.Features.Broadcast ScpSuicideMessage { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>SCP %playername% (%scpname%) se ha suicidado. Causa de la muerte: %reason%</color>", 12, true, Broadcast.BroadcastFlags.Normal);

        [Description("Texto que se muestra si el jugador no es el propietario de las esposas")]
        public Exiled.API.Features.Broadcast UnhandCuffDenied { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>No eres el propietario de las esposas de este jugador, así que no puedes quitárselas.</color>", 8, true, Broadcast.BroadcastFlags.Normal);

        [Description("Tiempo para reiniciar automáticamente si solo queda un jugador en el servidor (si está habilitado)")]
        public ushort AutoRestartTime { get; private set; } = 15;

        [Description("Duración del mensaje al objetivo de SCP-096 (si está habilitado)")]
        public ushort Scp096TargetMessageDuration { get; private set; } = 12;

        [Description("Duración del mensaje al último jugador con vida (si está habilitado)")]
        public ushort LastPlayerAliveMessageDuration { get; private set; } = 12;

        [Description("Duración predeterminada de los avisos enviados con el comando scputils_broadcast")]
        public ushort BroadcastDuration { get; private set; } = 12;

        [Description("Duración predeterminada de las pistas enviadas con el comando scputils_broadcast")]
        public ushort HintsDuration { get; private set; } = 12;

        [Description("Número mínimo de suicidios a partir del cual el jugador dejará de recibir expulsiones o vetos, sin tener en cuenta el porcentaje de suicidios o abandonos como SCP (si está habilitado)")]
        public int ScpSuicideTollerance { get; private set; } = 5;

        [Description("Duración base del veto por suicidio o abandono como SCP (si está habilitado)")]
        public int AutoBanDuration { get; private set; } = 15;

        [Description("Número base de rondas de veto automático por suicidio o abandono como SCP (si está habilitado)")]
        public int AutoBanRoundsCount { get; private set; } = 2;

        [Description("Longitud máxima del apodo al usar el comando para cambiarlo")]
        public int NicknameMaxLength { get; private set; } = 32;

        [Description("¿Durante cuántos segundos no se debe advertir por suicidio a un jugador después de que SCP-079 active una Tesla? (2 segundos suelen bastar en la mayoría de los servidores)")]
        public int Scp079TeslaEventWait { get; private set; } = 2;

        [Description("Tiempo mínimo de juego, en segundos, para considerar completo el día al usar el comando bpt")]
        public int BptMinSeconds { get; private set; } = 1200;

        [Description("Tiempo de espera para cambiar el apodo, en segundos")]
        public int ChangeNicknameCooldown { get; private set; } = 120;

        [Description("Número mínimo de jugadores en el servidor para que se cuente PT")]
        public int MinPlayersPtCount { get; private set; } = 1;

        [Description("Tiempo máximo, en segundos desde el inicio de la ronda, para enviar una solicitud de intercambio de SCP")]
        public int MaxAllowedTimeScpSwapRequest { get; private set; } = 60;

        [Description("Tiempo máximo, en segundos desde el inicio de la ronda, para aceptar una solicitud de intercambio de SCP")]
        public int MaxAllowedTimeScpSwapRequestAccept { get; private set; } = 75;

        [Description("Número máximo de intercambios permitidos")]
        public int MaxAllowedSwaps { get; private set; } = 1;

        [Description("Tiempo de espera de los comandos, en segundos")]
        public double CommandCooldownSeconds { get; private set; } = 5;

        [Description("Tiempo máximo permitido para usar el comando de reaparición; establece 0 para deshabilitarlo")]
        public int RespawnCommandTime { get; private set; } = 45;

        [Description("Lista de variables de sesión de SCP personalizados; si añades aquí el ID, se denegará el intercambio")]
        public List<string> DeniedSwapCustomInfo { get; private set; } = new List<string>() { "<color=#960018>SCP-20743</color>", "SCP-20743", "SCP-20743-1", "<color=#960018>SCP-20743-1</color>" };

        [Description("Porcentaje de abandonos o suicidios como SCP que debe alcanzar un jugador para recibir un veto (puedes añadir un margen en la configuración)")]
        public float AutoBanThreshold { get; private set; } = 30.5f;

        [Description("Porcentaje de abandonos o suicidios como SCP que debe alcanzar un jugador para ser expulsado (puedes añadir un margen en la configuración)")]
        public float AutoKickThreshold { get; private set; } = 15.5f;

        [Description("Cada cuántos segundos debe cambiar la etiqueta arcoíris")]
        public float RainbowTagSeconds { get; private set; } = 1.7f;

        [Description("Colores restringidos para el comando .scputils_change_color. Usa el comando colors en la consola del juego para verlos")]
        public List<string> RestrictedRoleColors { get; private set; } = new List<string>() { "Color1", "Color2" };

        [Description("Apodos restringidos para el comando .scputils_change_nickname")]
        public List<string> BannedNickNames { get; private set; } = new List<string>() { "@everyone", "@here", "Admin" };

        [Description("ASN que deben incluirse en la lista de bloqueo. Los jugadores que se conecten desde uno de ellos deben añadirse a la lista de permitidos con el comando scputils_whitelist_asn (50889 es el ASN de GeForce NOW)")]
        public List<string> ASNBlacklist { get; private set; } = new List<string>() { "50889" };

        [Description("ASN que se deben ignorar en el detector de multicuentas (50889 es el ASN de GeForce NOW)")]
        public List<string> ASNWhiteslistMultiAccount { get; private set; } = new List<string>() { "50889" };

        [Description("Mensaje que reciben los jugadores no incluidos en la lista de permitidos al conectarse desde un ASN bloqueado")]
        public string AsnKickMessage { get; private set; } = "El ASN desde el que te conectas está bloqueado en este servidor. Ponte en contacto con el personal para solicitar que se te incluya en la lista de permitidos.";

        [Description("Mensaje que se muestra al jugador que se convierte en objetivo de SCP-096")]
        public string Scp096TargetNotifyText { get; private set; } = "<color=red>Atención:</color>\n<color=purple>¡Te has convertido en objetivo de SCP-096!</color>";

        [Description("Mensaje que se muestra al último jugador con vida de un equipo")]
        public string LastPlayerAliveNotificationText { get; private set; } = "<color=red>Atención:</color>\n<color=purple>¡Eres el último jugador con vida de tu equipo!</color>";

        [Description("Mensaje que se muestra al jugador al volver a conectarse si recibió una advertencia mientras estaba desconectado")]
        public Exiled.API.Features.Broadcast OfflineWarnNotification { get; private set; } = new Exiled.API.Features.Broadcast("<color=red>Aviso de advertencia:</color>\n<color=yellow>Has recibido una advertencia recientemente por abandonar la partida mientras jugabas como SCP. Si sigues haciéndolo, podrías recibir un veto.</color>", 30, true, Broadcast.BroadcastFlags.Normal);

        [Description("Mensaje que se muestra al jugador cuando recibe un veto para jugar como SCP")]
        public Exiled.API.Features.Broadcast RoundBanNotification { get; private set; } = new Exiled.API.Features.Broadcast("<color=red>Has recibido un veto:</color>\n<color=yellow><size=27>No podrás jugar como SCP durante %roundnumber% rondas debido a tus infracciones anteriores.</size></color>", 30, true, Broadcast.BroadcastFlags.Normal);

        [Description("Mensaje que se muestra al jugador cuando aparece como SCP estando vetado y otro jugador ocupa su lugar")]
        public Exiled.API.Features.Broadcast RoundBanSpawnNotification { get; private set; } = new Exiled.API.Features.Broadcast("<color=red>Tienes un veto para jugar como SCP:</color>\n<color=yellow><size=27>Te han sustituido porque tienes un veto activo para jugar como SCP. Tendrán que sustituirte %roundnumber% vez/veces más antes de que puedas volver a jugar como SCP.</size></color>", 30, true, Broadcast.BroadcastFlags.Normal);

        [Description("Aviso de solicitud de intercambio de SCP")]
        public Exiled.API.Features.Broadcast SwapRequestBroadcast { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>%player% (%scp%) quiere intercambiar su rol contigo. Para aceptar, abre la consola con la tecla ò y escribe .accept; para rechazar, escribe .deny. Te quedan %seconds% segundos para responder.</color>", 20, true, Broadcast.BroadcastFlags.Normal);

        [Description("Aviso de cancelación de una solicitud de intercambio de SCP")]
        public Exiled.API.Features.Broadcast SwapRequestCanceledBroadcast { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>Se ha cancelado la solicitud de intercambio.</color>", 12, true, Broadcast.BroadcastFlags.Normal);

        [Description("Aviso de rechazo de una solicitud de intercambio de SCP")]
        public Exiled.API.Features.Broadcast SwapRequestDeniedBroadcast { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>Se ha rechazado la solicitud de intercambio.</color>", 12, true, Broadcast.BroadcastFlags.Normal);

        [Description("Aviso informativo sobre las solicitudes de intercambio de SCP")]
        public Exiled.API.Features.Broadcast SwapRequestInfoBroadcast { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>Juegas como SCP. Durante %seconds% segundos puedes intercambiar tu rol con otro SCP usando el comando swap en la tecla ò. Usa .scplist para ver quién juega como SCP.</color>", 15, true, Broadcast.BroadcastFlags.Normal);

        [Description("Información sobre el comando de reaparición")]
        public Exiled.API.Features.Broadcast RespawnBroadcast { get; private set; } = new Exiled.API.Features.Broadcast("<color=blue>Si la reaparición se queda bloqueada durante unos segundos, puedes usar el comando .respawn en la consola para reaparecer.</color>", 10, true, Broadcast.BroadcastFlags.Normal);

        [Description("A qué hora debe ejecutarse la tarea de reinicio automático del servidor")]
        public string AutoRestartTimeTask { get; private set; } = "1:35:0";

        [Description("Texto que se muestra fuera del mensaje incrustado de Discord")]
        public string ExtraText { get; private set; } = "@everyone";

        [Description("Mensaje de tiempo de espera de los comandos")]
        public string CooldownMessage { get; private set; } = "<color=red>¡No se ha podido ejecutar el comando! Estás sujeto a un tiempo de espera o tienes los comandos vetados. Espera 5 segundos y vuelve a intentarlo. Si el problema persiste, puede que tengas un veto para usar comandos. Para consultar el motivo y la duración, abre la consola al entrar en el servidor. Se ha informado al personal de esta acción abusiva para que tome las medidas oportunas.</color>";
        // public string CooldownMessage { get; private set; } = "<color=red>Esecuzione del comando fallita! Attualmente sei sotto cooldown oppure bannato, attendi 5 secondi e riprova, se l'errore è persistente significa che sei bannato, per vedere durata e motivazione apri la console appena entri nel server, questa azione illecita è stata segnalata allo staff.</color>";

        [Description("Grupos para los que el plugin debe ignorar la opción DNT")]
        public List<string> DntIgnoreList { get; private set; } = new List<string>() { "testusergroup1", "testusergroup2" };

        [Description("Clases autorizadas a ver la información de MTF y de la próxima reaparición")]
        public List<PlayerRoles.Team> AllowedMtfInfoTeam { get; private set; } = new List<PlayerRoles.Team>() { PlayerRoles.Team.FoundationForces, PlayerRoles.Team.Scientists, PlayerRoles.Team.Dead };

        [Description("Clases autorizadas a ver la información de Caos y de la próxima reaparición")]
        public List<PlayerRoles.Team> AllowedChaosInfoTeam { get; private set; } = new List<PlayerRoles.Team>() { PlayerRoles.Team.ClassD, PlayerRoles.Team.ChaosInsurgency, PlayerRoles.Team.Dead };

        [Description("Lista de colores permitidos para las etiquetas arcoíris")]
        public List<string> AllowedRainbowtagColors { get; private set; } = new List<string>() { "pink", "red", "default", "brown", "silver", "light_green", "crismon", "cyan", "aqua", "deep_pink", "tomato", "yellow", "magenta", "blue_green", "orange", "lime", "green", "emerald", "carmine", "nickel", "mint", "army_green", "pumpkin" };


        /*

          public Dictionary<Team, ImmunityPlayers> ImmunityPlayers { get; private set; } = new Dictionary<Team, ImmunityPlayers>()
          {
              //Class-D example dictionary config
              {
                Team.CDP, //key
                new ImmunityPlayers
                {                  
                   Attacker = new List<Team>() { Team.RSC, Team.MTF }, //Attacker List
                   ShouldBeCuffed = true, //Cuffed
                   ImmunityZones = new List<ZoneType>() { ZoneType.Entrance, ZoneType.Surface } //Zone List                  
                }
              },

              //Rip example dictionary config
              {
                Team.RIP, //key
                new ImmunityPlayers
                {
                    Attacker = new List<Team>() { Team.TUT }, //Attacker List
                    ShouldBeCuffed = false, //Cuffed
                    ImmunityZones = new List<ZoneType>() { ZoneType.Entrance, ZoneType.Surface, ZoneType.LightContainment, ZoneType.HeavyContainment, ZoneType.Unspecified } //Zone list
                }
              }
          };

          */

        [Description("Añade como clave el equipo que quieres proteger del objetivo y, como valor, los equipos enemigos. En la documentación de GitHub puedes consultar todos los equipos.")]

        public Dictionary<PlayerRoles.Team, List<PlayerRoles.Team>> CuffedImmunityPlayers { get; private set; } = new Dictionary<PlayerRoles.Team, List<PlayerRoles.Team>>()
        {
            {
                PlayerRoles.Team.ClassD,
                new List<PlayerRoles.Team>
                {
                    PlayerRoles.Team.Scientists, PlayerRoles.Team.FoundationForces
                }
            },
               {
                PlayerRoles.Team.Scientists,
                new List<PlayerRoles.Team>
                {
                    PlayerRoles.Team.ClassD, PlayerRoles.Team.ChaosInsurgency
                }
            },

        };


        [Description("Indica si los equipos protegidos deben estar esposados para recibir protección. Si no añades ningún equipo, la protección se aplicará en cualquier caso.")]

        public List<PlayerRoles.Team> CuffedProtectedTeams { get; private set; } = new List<PlayerRoles.Team>() { PlayerRoles.Team.ClassD, PlayerRoles.Team.Scientists };

        [Description("Establece qué SCP pueden elegir los jugadores para intercambiarse si ese SCP no ha aparecido; la solicitud se concederá de inmediato.")]
        public List<PlayerRoles.RoleTypeId> AllowedSwapGenerationList { get; private set; } = new List<PlayerRoles.RoleTypeId>() { PlayerRoles.RoleTypeId.Scp049, PlayerRoles.RoleTypeId.Scp079, PlayerRoles.RoleTypeId.Scp096, PlayerRoles.RoleTypeId.Scp106, PlayerRoles.RoleTypeId.Scp173, PlayerRoles.RoleTypeId.Scp939, PlayerRoles.RoleTypeId.Scp3114 };

        [Description("Establece qué SCP no pueden intercambiarse si no han aparecido. Los jugadores aún podrán realizar intercambios normales (por ejemplo, SCP-079 no puede convertirse en SCP-173 si SCP-173 no ha aparecido).")]
        public List<PlayerRoles.RoleTypeId> DisallowedScpsSwapGenerationList { get; private set; } = new List<PlayerRoles.RoleTypeId>() { PlayerRoles.RoleTypeId.Scp0492, PlayerRoles.RoleTypeId.Scp079 };

        [Description("Indica en qué zonas está protegido el equipo. Zonas disponibles: Surface, Entrance, HeavyContainment, LightContainment, Unspecified")]

        public Dictionary<PlayerRoles.Team, List<ZoneType>> CuffedSafeZones { get; private set; } = new Dictionary<PlayerRoles.Team, List<ZoneType>>()
        {
            {
                PlayerRoles.Team.ClassD,
                new List<ZoneType>
                {
                   ZoneType.Entrance, ZoneType.Surface, ZoneType.HeavyContainment, ZoneType.LightContainment, ZoneType.Other, ZoneType.Unspecified
                }
            },
               {
                PlayerRoles.Team.Scientists,
                new List<ZoneType>
                {
                    ZoneType.Entrance, ZoneType.Surface, ZoneType.HeavyContainment, ZoneType.LightContainment, ZoneType.Other, ZoneType.Unspecified
                }
            },

        };

        [Description("Traducciones de los tipos de daño")]

        public Dictionary<string, string> DamageTypesTranslations { get; private set; } = new Dictionary<string, string>() { { DamageTypes.AK.ToString().ToUpper(), DamageTypes.AK.ToString().ToUpper() }, { DamageTypes.Asphyxiation.ToString().ToUpper(), DamageTypes.Asphyxiation.ToString().ToUpper() }, { DamageTypes.Bleeding.ToString().ToUpper(), DamageTypes.Bleeding.ToString().ToUpper() },
        { DamageTypes.Com15.ToString().ToUpper(), DamageTypes.Com15.ToString().ToUpper() }, { DamageTypes.Com18.ToString().ToUpper(), DamageTypes.Com18.ToString().ToUpper() }, { DamageTypes.Crossvec.ToString().ToUpper(), DamageTypes.Crossvec.ToString().ToUpper() }, { DamageTypes.Crushed.ToString().ToUpper(), DamageTypes.Crushed.ToString().ToUpper() }, { DamageTypes.Custom.ToString().ToUpper(), DamageTypes.Custom.ToString().ToUpper() },
        { DamageTypes.Decontamination.ToString().ToUpper(), DamageTypes.Decontamination.ToString().ToUpper() }, { DamageTypes.E11Sr.ToString().ToUpper(), DamageTypes.E11Sr.ToString().ToUpper() }, { DamageTypes.Explosion.ToString().ToUpper(), DamageTypes.Explosion.ToString().ToUpper() }, { DamageTypes.Falldown.ToString().ToUpper(), DamageTypes.Falldown.ToString().ToUpper() }, { DamageTypes.FemurBreaker.ToString().ToUpper(), DamageTypes.FemurBreaker.ToString().ToUpper() },
        { DamageTypes.Firearm.ToString().ToUpper(), DamageTypes.Firearm.ToString().ToUpper() }, { DamageTypes.FriendlyFireDetector.ToString().ToUpper(), DamageTypes.FriendlyFireDetector.ToString().ToUpper() }, { DamageTypes.Fsp9.ToString().ToUpper(), DamageTypes.Fsp9.ToString().ToUpper() }, { DamageTypes.Hypothermia.ToString().ToUpper(), DamageTypes.Hypothermia.ToString().ToUpper() }, { DamageTypes.Logicer.ToString().ToUpper(), DamageTypes.Logicer.ToString().ToUpper() },
        { DamageTypes.MicroHid.ToString().ToUpper(), DamageTypes.MicroHid.ToString().ToUpper() }, { DamageTypes.ParticleDisruptor.ToString().ToUpper(), DamageTypes.ParticleDisruptor.ToString().ToUpper() }, { DamageTypes.PocketDimension.ToString().ToUpper(), DamageTypes.PocketDimension.ToString().ToUpper() }, { DamageTypes.Poison.ToString().ToUpper(), DamageTypes.Poison.ToString().ToUpper() }, { DamageTypes.Recontainment.ToString().ToUpper(), DamageTypes.Recontainment.ToString().ToUpper() },
        { DamageTypes.Scp.ToString().ToUpper(), DamageTypes.Scp.ToString().ToUpper() }, { DamageTypes.Scp018.ToString().ToUpper(), DamageTypes.Scp018.ToString().ToUpper() }, { DamageTypes.Scp049.ToString().ToUpper(), DamageTypes.Scp049.ToString().ToUpper() }, { DamageTypes.Scp0492.ToString().ToUpper(), DamageTypes.Scp0492.ToString().ToUpper() }, { DamageTypes.Scp096.ToString().ToUpper(), DamageTypes.Scp096.ToString().ToUpper() }, { DamageTypes.Scp106.ToString().ToUpper(), DamageTypes.Scp106.ToString().ToUpper() },
        { DamageTypes.Scp173.ToString().ToUpper(), DamageTypes.Scp173.ToString().ToUpper() }, { DamageTypes.Scp207.ToString().ToUpper(), DamageTypes.Scp207.ToString().ToUpper() }, { DamageTypes.Scp939.ToString().ToUpper(), DamageTypes.Scp939.ToString().ToUpper() }, { DamageTypes.SeveredHands.ToString().ToUpper(), DamageTypes.SeveredHands.ToString().ToUpper() },
        { DamageTypes.Shotgun.ToString().ToUpper(), DamageTypes.Shotgun.ToString().ToUpper() }, { DamageTypes.Tesla.ToString().ToUpper(), DamageTypes.Tesla.ToString().ToUpper() }, { DamageTypes.Unknown.ToString().ToUpper(), DamageTypes.Unknown.ToString().ToUpper() }, { DamageTypes.Warhead.ToString().ToUpper(), DamageTypes.Warhead.ToString().ToUpper() }, {DamageTypes.CardiacArrest.ToString().ToUpper(), DamageTypes.CardiacArrest.ToString().ToUpper()}, { DamageTypes.Jailbird.ToString().ToUpper(), DamageTypes.Jailbird.ToString().ToUpper()}, {DamageTypes.Scp3114.ToString(), DamageTypes.Scp3114.ToString()}};

        [Description("Nombre del comando para retirar una advertencia")]
        public string UnwarnCommand { get; set; } = "scputils_player_unwarn";

        [Description("Alias del comando para retirar una advertencia")]
        public string[] UnwarnCommandAliases { get; set; } = new[] { "unwarn", "sunwarn", "su_player_unw", "su_punw", "su_puw", "scpu_player_unw", "scpu_punw", "scpu_puw" };

        [Description("Nombre del comando para solicitar un intercambio")]
        public string SwapRequestCommand { get; set; } = "scputils_swap_request";

        [Description("Alias del comando para solicitar un intercambio")]
        public string[] SwapRequestCommandAliases { get; set; } = new[] { "swap", "swapr", "su_sr", "scpu_sr", "swap_request" };

        [Description("Nombre del comando para cancelar una solicitud de intercambio")]
        public string SwapRequestCancelCommand { get; set; } = "scputils_swap_request_cancel";

        [Description("Alias del comando para cancelar una solicitud de intercambio")]
        public string[] SwapRequestCancelCommandAliases { get; set; } = new[] { "cancel", "su_src", "swaprc", "scpu_src", "swap_request_cancel" };

        [Description("Nombre del comando para aceptar una solicitud de intercambio")]
        public string SwapRequestAcceptCommand { get; set; } = "scputils_swap_request_accept";

        [Description("Alias del comando para aceptar una solicitud de intercambio")]
        public string[] SwapRequestAcceptCommandAliases { get; set; } = new[] { "accept", "su_sra", "swapra", "scpu_sra", "swap_request_accept" };

        [Description("Nombre del comando para rechazar una solicitud de intercambio")]
        public string SwapRequestDenyCommand { get; set; } = "scputils_swap_request_deny";

        [Description("Alias del comando para rechazar una solicitud de intercambio")]
        public string[] SwapRequestDenyCommandAliases { get; set; } = new[] { "deny", "su_srd", "swaprd", "scpu_srd", "swap_request_deny" };

        [Description("Nombre del comando de reaparición")]
        public string RespawnCommand { get; set; } = "scputils_respawn";

        [Description("Alias del comando de reaparición")]
        public string[] RespawnCommandAliases { get; set; } = new[] { "respawn", "su_resp", "resp" };

        [Description("Aviso que se envía al personal conectado cuando un jugador entra con más de una cuenta")]
        public Exiled.API.Features.Broadcast AlertStaffBroadcastMultiAccount { get; private set; } = new Exiled.API.Features.Broadcast(
            "<size=40><color=red>Aviso</color></size>\n<size=35>El jugador <color=yellow>{player}</color> ha entrado con <color=yellow>{accountNumber}</color> cuentas</size>\n<size=30>Consulta la consola pulsando <color=yellow>ò</color></size>",
            10);

        [Description("Aviso que se envía al personal conectado cuando un jugador cambia de IP")]
        public Exiled.API.Features.Broadcast AlertStaffBroadcastChangeIP { get; private set; } = new Exiled.API.Features.Broadcast(
            "<size=40><color=red>Aviso</color></size>\n<size=35>El jugador <color=yellow>{player}</color> ha cambiado su IP de <color=yellow>{oldIP}</color> a <color=yellow>{newIP}</color></size>\n<size=35>Consulta la consola pulsando <color=yellow>ò</color></size>",
            10);
        public bool Debug { get; set; } = true;

        public void ConfigValidator()
        {
            if (MaxAllowedTimeScpSwapRequest > MaxAllowedTimeScpSwapRequestAccept)
            {
                Log.Warn("MaxAllowedTimeScpSwapRequest is higher than MaxAllowedTimeScpSwapRequestAccept, players might not be able to accept some requests doublecheck config!");
            }
            if (ScpSuicideTollerance < 0)
            {
                Log.Warn("Invalid config scputils_scp_suicide_tollerance, loading dafault one!");
                ScpSuicideTollerance = 5;

            }
            if (AutoKickThreshold >= AutoBanThreshold)
            {
                Log.Warn("Invalid config scputils_auto_kick_threshold OR scputils_auto_ban_threshold, loading dafault one!");
                AutoBanThreshold = 30.5f;

            }
            if (AutoRestartTime < 0)
            {
                Log.Warn("Invalid config scputils_auto_restart_time, loading dafault one!");
                AutoRestartTime = 15;
            }
            if (Scp079TeslaEventWait < 0)
            {
                Log.Warn("Invalid config scputils_scp_079_tesla_event_wait, loading dafault one!");
                Scp079TeslaEventWait = 2;
            }
            if (IgnoreDntRequests)
            {
                Log.Warn("You have set in server configs to ignore Do Not Track requests but that's a violation on Verified Server Rules (if your server is verified) and could cause punishement such as delist [Rule 8.11]");
            }

            if (EnableSCPSuicideSoftBan)
            {
                EnableSCPSuicideAutoBan = false;
            }

            if (DatabasePassword == "StrongPassword")
            {
                Log.Warn("You are using a weak database password (default one), everyone on github can see it, change it, please remember that it's important to use a strong password to avoid hackers!");
            }

            if (!IsEnabled)
            {
                Log.Warn("You disabled the plugin in server configs!");
            }
        }
    }
}

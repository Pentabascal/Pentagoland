using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace UltimateHUD
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Configuración del reloj:")]
        public bool EnableClock { get; set; } = true;
        public string Clock { get; set; } = "<align=left><space=65><color={color}><size=20><b>Hora:</b> {time}</size></color></align>";
        [Description("Zona horaria UTC | 2 = UTC+2")]
        public int TimeZone { get; set; } = 2;
        [Description("GAMEPLAY = solo jugadores | SPECTATOR = solo espectadores | BOTH = jugadores y espectadores")]
        public string ClockVisual { get; set; } = "BOTH";
        public int ClockYCordinate { get; set; } = 980;

        [Description("Configuración de TPS:")]
        public bool EnableTps { get; set; } = true;
        public string Tps { get; set; } = "<align=right><color={color}><size=20><b>TPS:</b> {tps}/{maxTps}</size></color></align>";
        [Description("GAMEPLAY = solo jugadores | SPECTATOR = solo espectadores | BOTH = jugadores y espectadores")]
        public string TpsVisual { get; set; } = "BOTH";
        public int TpsYCordinate { get; set; } = 980;

        [Description("Configuración de la duración de la ronda:")]
        public bool EnableRoundTime { get; set; } = true;
        public string RoundTime { get; set; } = "<align=left><space=5><color={color}><size=20><b>Duración de la ronda:</b> {round_time}</size></color></align>";
        [Description("GAMEPLAY = solo jugadores | SPECTATOR = solo espectadores | BOTH = jugadores y espectadores")]
        public string RoundTimeVisual { get; set; } = "BOTH";
        public int RoundTimeYCordinate { get; set; } = 950;

        [Description("Configuración del HUD de jugador:")]
        public bool EnablePlayerHud { get; set; } = true;
        [Description("Puedes usar {displayname} en lugar de {nickname}.")]
        public string PlayerHud { get; set; } = "<align=center><size=18><color=#D6FFFB><b>NOMBRE:</b></color> <color=#FFFFFF>{nickname}</color> <color=#94A3B8>|</color> <color=#D6FFFB><b>ROL:</b></color> {role} <color=#94A3B8>|</color> <color=#D6FFFB><b>BAJAS:</b></color> <color=yellow>{kills}</color></size></align>";
        [Description("Posición vertical del HUD de jugador; un valor algo menor lo baja hacia el borde inferior.")]
        public int PlayerHudYCordinate { get; set; } = 50;
        [Description("Posición vertical de la subclase, por encima del HUD del jugador.")]
        public int SubclassHudYCordinate { get; set; } = 75;
        [Description("Plantilla del indicador de subclase.")]
        public string SubclassHud { get; set; } = "<align=center><size=18><color=#C4B5FD><b>SUBCLASE:</b></color> {subclass}</size></align>";

        [Description("Lista de espectadores:")]
        public bool EnableSpectatorList { get; set; } = true;
        public string SpectatorListHeader { get; set; } = "<align=left><space=60><size=19><color={color}>Espectadores: {count}</color></size></align>";
        public string SpectatorListPlayers { get; set; } = "<align=left><space=60><size=16><color={color}>• {nickname}</color></size></align>";
        public List<RoleTypeId> HiddenForRoles { get; set; } = [RoleTypeId.Overwatch];
        public int SpectatorListYCordinate { get; set; } = 750;

        [Description("Contador de munición:")]
        public bool EnableAmmoCounter { get; set; } = true;
        public string WeaponName { get; set; } = "<align=left><space=70><size=21><color={color}>{weapon}</color></size></align>";
        public string AmmoCounter { get; set; } = "<align=left><space=70><size=21><b><color={color}>{current} / {max}</color></b></size></align>";
        public int AmmoCounterYCordinate { get; set; } = 140;

        [Description("Configuración del HUD de espectador:")]
        public bool EnableSpectatorHud { get; set; } = true;
        [Description("Puedes usar {displayname} en lugar de {nickname}.")]
        public string SpectatorHud { get; set; } = "<align=center><size=20><color=#7DD3FC><b>OBSERVANDO A:</b></color> <color=#FFFFFF>{nickname}</color> <color=#94A3B8>|</color> <color=#C4B5FD><b>ROL:</b></color> {role} <color=#94A3B8>|</color> <color=#7DD3FC><b>BAJAS:</b></color> <color=#FDE68A>{kills}</color></size></align>";
        [Description("Posición vertical del texto de espectador.")]
        public int SpectatorHudYCordinate { get; set; } = 130;
        [Description("Si está activado, no se mostrará el nombre del jugador observado cuando sea un esqueleto (SCP-3114), igual que en la interfaz de espectador del juego.")]
        public bool HideSkeletonNickname { get; set; } = true;

        [Description("Información del mapa para espectadores:")]
        public bool EnableSpectatorMapInfo { get; set; } = true;
        public string SpectatorMapInfo { get; set; } = "<size=20><b>Generadores:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Ojiva:</b> <color={warheadColor}>{warheadStatus}</color></size>";
        [Description("Posición vertical de mapa; la fila combinada usa SpectatorHudYCordinate.")]
        public int MapInfoYCordinate { get; set; } = 70;

        [Description("Información del servidor para espectadores:")]
        public bool EnableSpectatorServerInfo { get; set; } = true;
        public string SpectatorServerInfo { get; set; } = "<size=20><color=#7DD3FC><b>JUGADORES:</b></color> <color=#FBBF24>{players}/{maxPlayers}</color></size>";
        [Description("Posición vertical de servidor; la fila combinada usa SpectatorHudYCordinate.")]
        public int ServerInfoYCordinate { get; set; } = 70;
    }
}
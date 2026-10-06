using System.Collections.Generic;
using System.ComponentModel;
using Achievements;

namespace MvpSystem;

public class Config
{
    [Description("Activa los mensajes de depuración.")] public bool Debug { get; set; } = false;

    [Description(
        "Música que se reproducirá para cada MVP. Usa el SteamID como clave y el nombre del archivo como valor; asegúrate de que los archivos estén en la carpeta Mvp/Music.")]
    public Dictionary<string, string> MvpMusic { get; set; } = new()
    {
        { "steamid", "name.ogg" },
        { "steamid2", "name.ogg" }
    };

    [Description("Volumen de la música del MVP. 100 = normal; un valor menor baja el volumen y uno mayor lo amplifica (por encima de 100 puede distorsionarse).")]
    public float MusicVolume { get; set; } = 100f;

    [Description(
        "Puntuación que determina quién será el MVP. El jugador con más puntos en total se lleva el título.")]
    public MvpScoring Scoring { get; set; } = new();

    [Description("Si está instalado el plugin StatsSystem, también se contabilizarán sus estadísticas cuando esta opción esté activada.")]
    public bool StatsSystemIntegration { get; set; } = true;

    [Description("Duración del aviso. Puede que tengas que aumentarla si la duración de la ronda configurada es mayor.")]
    public ushort Duration { get; set; } = 30;

    [Description("Formato común de los textos de estadísticas. Incluye las etiquetas y el texto que quieras mostrar al principio.")]
    public string Start { get; set; } = "<size=31><line-height=0.9em>";

    [Description("Formato del título del MVP.")]
    public string MvpTitle { get; set; } = "<color=#78e2ff><b>{name}</b></color> es el MVP.";

    [Description("Formato de cada estadística. Si lo dejas vacío, no se mostrará.")]
    public string MostKillsAsScp { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> consiguió más bajas como <color=#ff0000><b>{role}</b></color>: <color=#45ff7a><b>{kills}</b></color>";

    public string FirstToKillScp { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> fue el primero en eliminar a un <color=#ff0000><b>SCP</b></color>";

    [Description("Sustituye a first_to_kill_scp si el jugador ha eliminado a más de un SCP.")]
    public string MostScpsKilled { get; set; } = "<color=#78e2ff><b>{name}</b></color> eliminó a {scps}";

    public string MostScpsKilledListItem { get; set; } = "<color=#ff0000><b>{scp}</b></color>";

    public string MostKillsAsHuman { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> consiguió más bajas como humano: <color=#45ff7a><b>{kills}</b></color>";

    public string FirstToEscape { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> fue el primero en escapar, en <color=#45ff7a><b>{time}</b></color>, como <b>{role}</b>";

    public string BestAchievement { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> desbloqueó <color=#45ff7a><b>{achievement}</b></color>: {description}";

    public string MostDamageDealt { get; set; } =
        "<color=#78e2ff><b>{name}</b></color> infligió más daño: <color=#45ff7a><b>{damage}</b></color>";

    [Description("Cierra las etiquetas abiertas en Start y añade el texto que quieras mostrar al final.")]
    public string End { get; set; } = "</line-height></size>";

    [Description(
        "Logros que se tendrán en cuenta durante la ronda. El orden determina la prioridad: los que aparecen antes tienen preferencia. De forma predeterminada se incluyen todos los logros válidos; faltan Christmas, Halloween y They Are Just Resources... por motivos técnicos.")]
    public List<AchievementName> Achievements { get; set; } =
    [
        AchievementName.BePoliteBeEfficient,
        AchievementName.ChangeInCommand,
        AchievementName.Overcurrent,
        AchievementName.Escape207,
        AchievementName.TurnThemAll,
        AchievementName.AccessGranted,
        AchievementName.CrisisAverted,
        AchievementName.DidntEvenFeelThat,
        AchievementName.MicrowaveMeal,
        AchievementName.EscapeArtist,
        AchievementName.Pacified,
        AchievementName.IllPassThanks,
        AchievementName.ForScience,
        AchievementName.FireInTheHole,
        AchievementName.ItsAlwaysLeft,
        AchievementName.SecureContainProtect,
        AchievementName.IsThisThingOn,
        AchievementName.WalkItOff,
        AchievementName.Friendship,
        AchievementName.HeWillBeBack,
        AchievementName.ExecutiveAccess,
        AchievementName.AnomalouslyEfficient,
        AchievementName.MelancholyOfDecay,
        AchievementName.ThatCanBeUseful,
        AchievementName.TMinus,
        AchievementName.ProceedWithCaution,
        AchievementName.DontBlink,
        AchievementName.DeltaCommand,
        AchievementName.LightsOut
    ];

    public Dictionary<AchievementName, string> AchievementDescriptions { get; set; } = new()
    {
        { AchievementName.LightsOut, "Reapareció como Fuerza Móvil de Contención" },
        { AchievementName.DeltaCommand, "Reapareció como Insurgencia del Caos" },
        { AchievementName.DontBlink, "Consiguió esquivar a <color=#ff0000>SCP-173</color>" },
        {
            AchievementName.ProceedWithCaution,
            "Atravesó una puerta Tesla vigilada por <color=#ff0000>SCP-079</color>"
        },
        { AchievementName.TMinus, "Sobrevivió a la detonación de la ojiva Alfa" },
        { AchievementName.ThatCanBeUseful, "Encontró un arma siendo <color=#FF8000>Clase D</color>" },
        {
            AchievementName.MelancholyOfDecay,
            "Capturó a un jugador en los cinco segundos posteriores a salir del suelo como <color=#ff0000>SCP-106</color>"
        },
        {
            AchievementName.AnomalouslyEfficient,
            "Eliminó a un jugador durante el primer minuto de la partida como <color=#ff0000>SCP</color>"
        },
        { AchievementName.ExecutiveAccess, "Consiguió una tarjeta de acceso del nivel máximo" },
        { AchievementName.HeWillBeBack, "Consiguió escapar de la dimensión de bolsillo" },
        {
            AchievementName.Friendship,
            "Mejoró su tarjeta de acceso junto a un miembro de <color=#FF8000>Clase D</color> siendo <color=#FFFF7C>científico</color>"
        },
        { AchievementName.WalkItOff, "Sobrevivió a una caída con menos de la mitad de la salud" },
        { AchievementName.IsThisThingOn, "Emitió un mensaje «útil» por el intercomunicador" },
        { AchievementName.SecureContainProtect, "Eliminó al último <color=#ff0000>SCP</color> de la ronda como miembro de la Fundación" },
        { AchievementName.ItsAlwaysLeft, "Escapó como miembro de <color=#FF8000>Clase D</color>" },
        { AchievementName.FireInTheHole, "Eliminó a un enemigo con una granada" },
        { AchievementName.ForScience, "Escapó como <color=#FFFF7C>científico</color>" },
        {
            AchievementName.IllPassThanks,
            "Eliminó a alguien que estaba usando el Micro H.I.D siendo un <color=#ff0000>SCP</color>"
        },
        { AchievementName.Pacified, "Eliminó a <color=#ff0000>SCP-096</color> mientras entraba en cólera" },
        { AchievementName.EscapeArtist, "Fue el primero en escapar de la instalación" },
        { AchievementName.MicrowaveMeal, "Eliminó a un <color=#ff0000>SCP</color> con el Micro H.I.D" },
        { AchievementName.DidntEvenFeelThat, "Sobrevivió a un impacto que lo habría matado gracias a la adrenalina" },
        { AchievementName.CrisisAverted, "Usó SCP-500 cuando estaba a punto de morir" },
        {
            AchievementName.AccessGranted,
            "Eliminó a un <color=#FFFF7C>científico</color> con una tarjeta de acceso siendo <color=#FF8000>Clase D</color>"
        },
        { AchievementName.TurnThemAll, "Curó a diez personas como <color=#ff0000>SCP-049</color>" },
        { AchievementName.Escape207, "Escapó bajo los efectos de SCP-207" },
        { AchievementName.SomethingDoneRight, "Eliminó a un SCP como <color=#FFFF7C>científico</color>" },
        {
            AchievementName.PropertyOfChaos,
            "Escapó con más de dos objetos SCP siendo <color=#FF8000>Clase D</color>"
        },
        { AchievementName.ThatWasClose, "Canceló la detonación de la ojiva Alfa en los últimos 15 segundos" },
        { AchievementName.Overcurrent, "Intentó recargar el Micro H.I.D" },
        { AchievementName.ChangeInCommand, "Desarmó a un miembro de la Fundación" },
        { AchievementName.BePoliteBeEfficient, "Eliminó a cinco enemigos en menos de 30 segundos" }
    };

    public Dictionary<AchievementName, string> AchievementNames { get; set; } = new()
    {
        { AchievementName.LightsOut, "A oscuras" },
        { AchievementName.DeltaCommand, "Nosotros, el Mando Delta..." },
        { AchievementName.DontBlink, "No parpadees" },
        { AchievementName.ProceedWithCaution, "Procede con cautela" },
        { AchievementName.TMinus, "Cuenta atrás: 90 segundos..." },
        { AchievementName.ThatCanBeUseful, "... ¿Estás pensando lo mismo que yo?" },
        { AchievementName.MelancholyOfDecay, "Melancolía de la decadencia" },
        { AchievementName.AnomalouslyEfficient, "Eficiencia anómala" },
        { AchievementName.ExecutiveAccess, "Acceso ejecutivo" },
        { AchievementName.HeWillBeBack, "Volverá..." },
        { AchievementName.Friendship, "Amistad" },
        { AchievementName.WalkItOff, "No ha sido nada" },
        { AchievementName.IsThisThingOn, "¿Funciona esto?" },
        { AchievementName.SecureContainProtect, "Asegurar. Contener. Proteger." },
        { AchievementName.ItsAlwaysLeft, "¡Siempre a la izquierda, hermanos!" },
        { AchievementName.FireInTheHole, "¡Al hoyo!" },
        { AchievementName.ForScience, "¡Por la ciencia!" },
        { AchievementName.IllPassThanks, "No, gracias" },
        { AchievementName.Pacified, "Pacificado" },
        { AchievementName.EscapeArtist, "Maestro de la fuga" },
        { AchievementName.MicrowaveMeal, "Plato al microondas" },
        { AchievementName.DidntEvenFeelThat, "¡Ja! ¡Ni lo he notado!" },
        { AchievementName.CrisisAverted, "Crisis evitada" },
        { AchievementName.AccessGranted, "Acceso concedido" },
        { AchievementName.TurnThemAll, "Mi cura es la más eficaz..." },
        { AchievementName.Escape207, "A lomos de la cafeína" },
        { AchievementName.SomethingDoneRight, "Si quieres que algo salga bien..." },
        { AchievementName.PropertyOfChaos, "Propiedad de la Insurgencia del Caos" },
        { AchievementName.ThatWasClose, "Por los pelos..." },
        { AchievementName.Overcurrent, "Sobrecarga" },
        { AchievementName.ChangeInCommand, "Cambio de mando" },
        { AchievementName.BePoliteBeEfficient, "Sé educado. Sé eficaz." }
    };
}

public class MvpScoring
{
    [Description(
        "Puntuación mínima que debe alcanzar un jugador para optar al título de MVP. Si nadie llega a ella, no se anuncia ningún MVP; así no se premia a quien no haya contribuido.")]
    public float MinimumScore { get; set; } = 5f;

    [Description("Puntos otorgados por cada baja como humano.")]
    public float PerHumanKill { get; set; } = 2f;

    [Description("Puntos otorgados por cada baja como SCP.")]
    public float PerScpKill { get; set; } = 2f;

    [Description("Puntos otorgados por cada SCP eliminado (eliminar un SCP tiene un gran impacto).")]
    public float PerScpKilled { get; set; } = 8f;

    [Description("Puntos otorgados por cada punto de daño infligido.")]
    public float PerDamage { get; set; } = 0.02f;

    [Description("Puntos otorgados por escapar de la instalación.")]
    public float Escape { get; set; } = 4f;

    [Description("Puntos extra para quien escape primero (el más rápido).")]
    public float FirstEscapeBonus { get; set; } = 4f;

    [Description("Puntos extra para quien elimine primero a un SCP.")]
    public float FirstScpKillBonus { get; set; } = 4f;

    [Description("Puntos base otorgados por conseguir cualquiera de los logros registrados.")]
    public float AchievementBase { get; set; } = 3f;

    [Description("Puntos adicionales según la prioridad del logro (los de mayor prioridad valen más).")]
    public float AchievementPriorityBonus { get; set; } = 0.3f;
}
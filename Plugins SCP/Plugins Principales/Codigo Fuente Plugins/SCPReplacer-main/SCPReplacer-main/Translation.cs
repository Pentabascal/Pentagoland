namespace SCPReplacer
{
    using Exiled.API.Interfaces;
    using System.ComponentModel;

    /// <summary>
    /// The plugin's user-facing messages.
    /// </summary>
    public class Translation : ITranslation
    {
        /// <summary>
        /// Gets or sets the broadcast shown to everyone when an SCP quits and the lottery opens.
        /// </summary>
        [Description("Anuncio que se muestra a todos cuando un SCP abandona la partida y se abre el sorteo. {0} es la etiqueta «SCP-XXX» con color, {1} es el número de SCP sin formato y {2} es la duración del sorteo en segundos.")]
        public string LotteryOpenedBroadcast { get; set; } = "{0} necesita un reemplazo. ¡Pulsa ~ y escribe .volunteer {1} en los próximos {2} segundos para tener la oportunidad de ocupar su lugar!";

        /// <summary>
        /// Gets or sets the message shown to a player after they successfully volunteer.
        /// </summary>
        [Description("Mensaje que se muestra al jugador cuando se apunta correctamente al sorteo. {0} es la etiqueta «SCP-XXX» con color.")]
        public string VolunteerConfirmed { get; set; } = "Te has apuntado al sorteo para convertirte en {0}.";

        /// <summary>
        /// Gets or sets the message shown to whoever wins the lottery.
        /// </summary>
        [Description("Mensaje que se muestra a quien gana el sorteo. {0} es la etiqueta «SCP-XXX» con color.")]
        public string LotteryWon { get; set; } = "¡Has ganado el sorteo! Ahora eres {0}.";

        /// <summary>
        /// Gets or sets the message shown when nobody volunteers in time.
        /// </summary>
        [Description("Mensaje que se muestra si nadie se ofrece a tiempo.")]
        public string LotteryNoVolunteers { get; set; } = "<color=orange>Nadie se ha ofrecido a tiempo</color>: el SCP no será reemplazado.";

        /// <summary>
        /// Gets or sets the message shown when an SCP successfully gives up their role early.
        /// </summary>
        [Description("Mensaje que se muestra cuando un SCP renuncia correctamente a su rol al principio de la ronda. {0} es el nombre del nuevo rol con color.")]
        public string HumanForfeitConfirmed { get; set; } = "Has renunciado a tu rol de SCP. Ahora tienes el rol de {0}.";

        /// <summary>
        /// Gets or sets the header prepended to every broadcast this plugin sends.
        /// </summary>
        [Description("Encabezado que se añade al principio de todos los anuncios de este complemento.")]
        public string BroadcastHeader { get; set; } = "<color=red>[SCP Replacer]</color>\n";

        /// <summary>
        /// Gets or sets the message broadcast to everyone else once an SCP has been successfully replaced.
        /// </summary>
        [Description("Mensaje que se anuncia a los demás cuando se reemplaza correctamente a un SCP. {0} es la etiqueta «SCP-XXX» con color.")]
        public string ReplacementAnnouncement { get; set; } = "{0} ha sido reemplazado.";

        /// <summary>
        /// Gets or sets the response when a command is used from somewhere other than a player (e.g. the server console).
        /// </summary>
        [Description("Respuesta cuando alguien que no es un jugador, como la consola del servidor, usa un comando.")]
        public string NotAPlayer { get; set; } = "Solo los jugadores pueden usar este comando.";

        /// <summary>
        /// Gets or sets the usage text shown when .volunteer is used with the wrong arguments.
        /// </summary>
        [Description("Texto de uso que se muestra cuando .volunteer se ejecuta con argumentos incorrectos.")]
        public string VolunteerUsage { get; set; } = "Uso: .volunteer <número de SCP>. Ejemplo: .volunteer 079 o .v 079";

        /// <summary>
        /// Gets or sets the response when a player who can't take part in the lottery tries to volunteer.
        /// </summary>
        [Description("Respuesta cuando un jugador que no puede participar en el sorteo (un SCP o alguien en modo Overwatch) intenta apuntarse.")]
        public string VolunteerNotEligible { get; set; } = "No puedes participar en el sorteo ahora mismo.";

        /// <summary>
        /// Gets or sets the response when the requested SCP isn't up for replacement.
        /// </summary>
        [Description("Respuesta cuando el SCP solicitado no está disponible para reemplazo. {0} es la lista de números de SCP que sí lo están.")]
        public string VolunteerUnavailable { get; set; } = "Ese SCP no está disponible. Puedes apuntarte para reemplazar a: {0}";

        /// <summary>
        /// Gets or sets the response when no SCP is currently up for replacement.
        /// </summary>
        [Description("Respuesta cuando no hay ningún SCP disponible para reemplazo.")]
        public string VolunteerNoneEligible { get; set; } = "Ahora mismo no hay ningún SCP pendiente de reemplazo.";

        /// <summary>
        /// Gets or sets the response when a player volunteers twice for the same SCP.
        /// </summary>
        [Description("Respuesta cuando un jugador se apunta dos veces para reemplazar al mismo SCP.")]
        public string VolunteerAlreadyEntered { get; set; } = "Ya te has ofrecido para reemplazar a este SCP.";

        /// <summary>
        /// Gets or sets the response when .human is used while the forfeit command is disabled.
        /// </summary>
        [Description("Respuesta cuando se usa .human mientras el comando para renunciar al rol está desactivado.")]
        public string HumanDisabled { get; set; } = "Este comando está desactivado en este momento.";

        /// <summary>
        /// Gets or sets the response when a player who isn't an SCP uses .human.
        /// </summary>
        [Description("Respuesta cuando un jugador que no es un SCP (o es SCP-049-2) usa .human.")]
        public string HumanNotScp { get; set; } = "Debes ser un SCP (excepto SCP-049-2) para usar este comando.";

        /// <summary>
        /// Gets or sets the response when .human is used after the cutoff.
        /// </summary>
        [Description("Respuesta cuando se usa .human después de que haya pasado el límite de tiempo para abandonar el rol.")]
        public string HumanTooLate { get; set; } = "Este comando solo se puede usar al principio de la ronda.";

        /// <summary>
        /// Gets or sets the response when .human is used while below the required health.
        /// </summary>
        [Description("Respuesta cuando se usa .human con menos salud de la necesaria.")]
        public string HumanLowHealth { get; set; } = "No tienes suficiente salud para usar este comando.";

        /// <summary>
        /// Gets or sets the response when the player who left or gave up an SCP tries to volunteer for it.
        /// </summary>
        [Description("Respuesta cuando quien abandonó o cedió un rol de SCP intenta apuntarse para reemplazarlo.")]
        public string VolunteerFormerHolder { get; set; } = "No puedes ofrecerte para reemplazar al SCP que dejaste.";

        /// <summary>
        /// Gets or sets the broadcast shown to everyone when a plugin update has been installed and the server will restart once the round ends.
        /// </summary>
        [Description("Anuncio que se muestra a todos cuando se instala una actualización del complemento y el servidor se reiniciará al terminar la ronda. {0} es el nombre del complemento. Déjalo vacío para desactivarlo.")]
        public string AutoUpdateRestartBroadcast { get; set; } = "<color=orange>[Actualización]</color> {0} se ha actualizado. El servidor se reiniciará al terminar esta ronda para aplicar la actualización.";
    }
}
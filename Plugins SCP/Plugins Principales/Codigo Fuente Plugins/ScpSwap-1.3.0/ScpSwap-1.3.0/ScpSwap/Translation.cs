// -----------------------------------------------------------------------
// <copyright file="Translation.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpSwap
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using ScpSwap.Configs;
    using ScpSwap.Models;

    /// <inheritdoc />
    public class Translation : ITranslation
    {
        /// <summary>
        /// Gets or sets a collection of custom names with their correlating <see cref="RoleTypeId"/>.
        /// </summary>
        [Description("Colección de nombres personalizados asociados a su RoleTypeId correspondiente.")]
        public Dictionary<string, RoleTypeId> TranslatableSwaps { get; set; } = new Dictionary<string, RoleTypeId>
        {
            { "173", RoleTypeId.Scp173 },
            { "peanut", RoleTypeId.Scp173 },
            { "939", RoleTypeId.Scp939 },
            { "079", RoleTypeId.Scp079 },
            { "79", RoleTypeId.Scp079 },
            { "computer", RoleTypeId.Scp079 },
            { "106", RoleTypeId.Scp106 },
            { "larry", RoleTypeId.Scp106 },
            { "096", RoleTypeId.Scp096 },
            { "96", RoleTypeId.Scp096 },
            { "shyguy", RoleTypeId.Scp096 },
            { "049", RoleTypeId.Scp049 },
            { "49", RoleTypeId.Scp049 },
            { "doctor", RoleTypeId.Scp049 },
            { "0492", RoleTypeId.Scp0492 },
            { "492", RoleTypeId.Scp0492 },
            { "zombie", RoleTypeId.Scp0492 },
            { "3114", RoleTypeId.Scp3114 },
            { "skeleton", RoleTypeId.Scp3114 },
        };

        /// <summary>
        /// Gets or sets the message to be displayed to all Scp subjects at the start of the round.
        /// </summary>
        [Description("Mensaje que se muestra al inicio de la ronda a todos los jugadores que sean SCP.")]
        public Broadcast StartMessage { get; set; } = new Broadcast("<color=yellow><b>¿Sabías que puedes intercambiar tu clase con la de otros SCP?</b></color> Solo tienes que escribir <color=orange>.scpswap (número de rol)</color> en la consola del juego (no en la consola de administración) para intercambiarla.", 15);

        /// <summary>
        /// Gets or sets the broadcast to display to the receiver of a swap request.
        /// </summary>
        [Description("Aviso que se muestra a quien recibe una solicitud de intercambio.")]
        public Broadcast RequestBroadcast { get; set; } = new Broadcast("<i>¡Tienes una solicitud de intercambio de SCP!\nConsulta la consola pulsando [`] o [~]</i>", 5);

        /// <summary>
        /// Gets or sets the console message to send to the receiver of a swap request.
        /// </summary>
        [Description("Mensaje de consola que se envía a quien recibe una solicitud de intercambio.")]
        public ConsoleMessage RequestConsoleMessage { get; set; } = new ConsoleMessage("$SenderName, que juega como $RoleName, te ha enviado una solicitud de intercambio. ¿Quieres intercambiar roles con esa persona? Escribe \".scpswap accept\" para aceptar o \".scpswap decline\" para rechazar.", "yellow");

        /// <summary>
        /// Gets or sets the console message to send to players when the swap succeeds.
        /// </summary>
        [Description("Mensaje de consola que se envía a los jugadores cuando el intercambio se completa correctamente.")]
        public ConsoleMessage SwapSuccessful { get; set; } = new ConsoleMessage("¡Intercambio realizado correctamente!", "green");

        /// <summary>
        /// Gets or sets the console message to send to the receiver of a swap request that has timed out.
        /// </summary>
        [Description("Mensaje de consola que se envía a quien recibe una solicitud de intercambio cuando esta caduca.")]
        public ConsoleMessage TimeoutReceiver { get; set; } = new ConsoleMessage("Tu solicitud de intercambio ha caducado.", "red");

        /// <summary>
        /// Gets or sets the console message to send to the sender of a swap request that has timed out.
        /// </summary>
        [Description("Mensaje de consola que se envía a quien inició una solicitud de intercambio cuando esta caduca.")]
        public ConsoleMessage TimeoutSender { get; set; } = new ConsoleMessage("El jugador no ha respondido a tu solicitud.", "red");

        /// <summary>
        /// Gets or sets the various command instances to be translated.
        /// </summary>
        [Description("Configuración de los distintos comandos.")]
        public CommandTranslations CommandTranslations { get; set; } = new CommandTranslations();
        
        public string ExecutorIsntPlayer { get; set; } = "El comando debe ejecutarse dentro del juego.";
        public string RoundIsntStarted { get; set; } = "La ronda aún no ha comenzado.";
        public string SwapPeriodEnded { get; set; } = "El plazo para intercambiar roles SCP ha terminado.";
        public string AllowUserSwapByPermission { get; set; } = "No tienes permiso para usar este comando.";
        public string NotAnScp { get; set; } = "Debes ser un SCP para usar este comando.";
        public string AlreadyHasPendingRequest { get; set; } = "¡Ya tienes una solicitud de intercambio pendiente!";
        public string CannotSwapWithYourself { get; set; } = "No puedes intercambiarte contigo mismo.";
        public string CannotSwapOffThisScp { get; set; } = "No puedes cambiar de rol desde este SCP.";
        public string RequestSent { get; set; } = "¡Solicitud de intercambio enviada!";
        public string CannotFindRole { get; set; } = "No se encuentra el rol especificado.";
        public string SuccessfulSwap { get; set; } = "Intercambio realizado correctamente.";
        public string CannotFindPlayerWithRole { get; set; } = "No se ha encontrado ningún jugador con el rol solicitado.";
        public string NoPendingRequest { get; set; } = "No tienes ninguna solicitud de intercambio pendiente.";
        public string SwapRequestCancelled { get; set; } = "Solicitud de intercambio cancelada.";
    }
}
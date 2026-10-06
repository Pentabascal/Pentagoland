// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace ScpSwap
{
    using System;
    using System.ComponentModel;
    using Exiled.API.Interfaces;
    using PlayerRoles;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be shown.
        /// </summary>
        [Description("Indica si se deben mostrar mensajes de depuración.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets the duration, in seconds, before a swap request gets automatically deleted.
        /// </summary>
        [Description("Tiempo, en segundos, antes de que se elimine automáticamente una solicitud de intercambio.")]
        public float RequestTimeout { get; set; } = 20f;

        /// <summary>
        /// Gets or sets the duration, in seconds, after the round starts that swap requests can be sent.
        /// </summary>
        [Description("Tiempo, en segundos desde el inicio de la ronda, durante el que se pueden enviar solicitudes de intercambio.")]
        public float SwapTimeout { get; set; } = 60f;

        /// <summary>
        /// Gets or sets a value indicating whether a player can switch to a class if there is nobody playing as it.
        /// </summary>
        [Description("Indica si un jugador puede cambiar a una clase SCP cuando nadie la está usando.")]
        public bool AllowNewScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a collection of roles blacklisted from being swapped to.
        /// </summary>
        [Description("Colección de roles SCP a los que no se puede cambiar.")]
        public RoleTypeId[] BlacklistedScps { get; set; } =
        {
            RoleTypeId.Scp0492,
        };

        /// <summary>
        /// Gets or sets a collection of the names of custom scps blacklisted from being swapped to. This must match the name the developer integrated the SCP into this plugin's API with.
        /// </summary>
        [Description("Colección de nombres de SCP personalizados a los que no se puede cambiar. Deben coincidir con el nombre con el que el desarrollador registró el SCP en la API de este plugin.")]
        public string[] BlacklistedNames { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Gets or sets roles of SCPs who cannot be swapped off from.
        /// </summary>
        [Description("Colección de roles SCP desde los que no se permite cambiar.")]
        public RoleTypeId[] BlacklistedSwapFromScps { get; set; } =
        {
            RoleTypeId.Scp0492,
        };

        /// <summary>
        /// Enables/Disables the SCP Swap plugin based off of permissions. Permission is "scpswap.allowed"
        /// </summary>
        [Description(
            "Activa o desactiva el intercambio para los usuarios según sus permisos. El permiso es scpswap.allowed.")]
        public bool AllowUserSwapByPermission { get; set; } = false;
    }
}
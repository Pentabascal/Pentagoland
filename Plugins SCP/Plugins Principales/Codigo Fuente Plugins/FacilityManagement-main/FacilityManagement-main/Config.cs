using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Interfaces;
using Exiled.Events.Handlers;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles;
using PlayerRoles.Voice;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace FacilityManagement
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = false;
        public bool Debug { get; set; } = false;

        [Description("Otorga munición infinita a las armas.")]
        public List<ItemType> InfiniteAmmo { get; set; } = new()
        {
            ItemType.ParticleDisruptor,
        };

        [Description("Modifica el consumo de energía del MicroHID.")]
        public float EnergyMicroHid { get; set; } = 1f;
        [Description("Modifica el consumo de batería de la radio.")]
        public float EnergyRadio { get; set; } = 1f;

        [Description("Configura el generador.")]
        public GeneratorBuild CustomGenerator { get; set; } = new()
        {
            UnlockCooldown = 0.1f,
            LeverDelay = 0.5f,
            DoorPanelCooldown = 0.9f,
            InteractionCooldown = 0,
            DeactivationTime = 125,
            RequiredPermission = Exiled.API.Enums.KeycardPermissions.ArmoryLevelOne,
        };

        [Description("Define el tiempo que tarda el ascensor en teletransportarse.")]
        public Dictionary<ElevatorType, float> LiftMoveDuration { get; set; } = new();

        [Description(@"Texto personalizado del intercomunicador. Si no se define un texto específico, se mostrará el texto predeterminado del cliente.
        # Consulta el README de GitHub para obtener más información (https://github.com/louis1706/FacilityManagement/blob/main/readme.md)")]
        public Dictionary<IntercomDisplay.IcomText, string> CustomText { get; set; } = new()
        {
            {IntercomDisplay.IcomText.Ready, "Listo"},
            {IntercomDisplay.IcomText.Transmitting, "{intercom_speaker_nickname} está hablando. Quedan {intercom_speech_remaining_time} s"},
            {IntercomDisplay.IcomText.TrasmittingBypass, "{intercom_speaker_nickname} está hablando. Quedan {intercom_speech_remaining_time} s"},
            {IntercomDisplay.IcomText.Restarting, "Reiniciando. Espera {intercom_remaining_cooldown} s"},
            {IntercomDisplay.IcomText.AdminUsing, "El administrador {intercom_speaker_nickname} está hablando"},
            {IntercomDisplay.IcomText.Muted, "Issou, estás silenciado"},
            {IntercomDisplay.IcomText.Wait, "Espera"},
            {IntercomDisplay.IcomText.Unknown, "Desconocido"},
        };
        [Description("Si se establece, el texto del intercomunicador se actualizará una vez por segundo; si se deja vacío, se actualizará en cada ciclo.")]
        public float? IntercomRefresh { get; set; } = null;

        [Description("Indica si deben eliminarse todos los objetos y cadáveres de la instalación tras la detonación.")]
        public bool WarheadCleanup { get; set; } = true;
        [Description("Configura las puertas Tesla.")]
        public TeslaBuild CustomTesla { get; set; } = new()
        {
            ActivationTime = 0.75f,
            IdleRange = 6.55f,
            TriggerRange = 5.1f,
            IgnoredRoles = new()
        };

        [Description("Configura SCP-914.")]
        public Scp914Build CustomScp914 { get; set; } = new()
        {
            KnobChangeCooldown = 0.15f,
            DoorCloseTime = 0.8f,
            ItemUpgradeTime = 10,
            DoorOpenTime = 13.5f,
            ActivationCooldown = 16,
        };


        [Description("Establece la resistencia de los cristales rompibles.")]
        public Dictionary<GlassType, GlassBuild> CustomWindows { get; set; } = new()
        {
            {
                GlassType.Scp079Trigger,
                new GlassBuild{
                        Health = 5,
                        DisableScpDamage = true,
                }
            },
        };

        [Description("Define los tipos de daño que ignoran las puertas rompibles (0 permite destruirlas con cualquier tipo de daño; -1 las hace indestructibles).")]
        public Dictionary<DoorType, DoorBuild> CustomDoors { get; set; } = new()
        {
            {
                DoorType.CheckpointEzHczA,
                new DoorBuild{
                            Health = 30,
                            RequiredPermission = KeycardPermissions.Checkpoints | KeycardPermissions.ScpOverride,
                            RequireAllPermission = false,
                            DamageTypeIgnored = DoorDamageType.Grenade | DoorDamageType.Weapon | DoorDamageType.Scp096,
                }
            },
            {
                DoorType.CheckpointEzHczB,
                new DoorBuild{
                            Health = 30,
                            RequiredPermission = KeycardPermissions.Checkpoints | KeycardPermissions.ScpOverride,
                            RequireAllPermission = false,
                            DamageTypeIgnored = DoorDamageType.Grenade | DoorDamageType.Weapon | DoorDamageType.Scp096,
                }
            },
            {
                DoorType.GR18Inner,
                new DoorBuild{
                            Health = 150,
                            RequiredPermission = KeycardPermissions.None,
                            RequireAllPermission = null,
                            DamageTypeIgnored = 0,
                }
            },
        };

        [Description("Otorga AHP a los RoleType indicados (antes se llamaba Hume Shield y estaba asociado a los SCP; ahora está separado y esta opción solo otorga AHP).")]
        public Dictionary<RoleTypeId, AhpProccessBuild> RoleTypeHumeShield { get; set; } = new()
        {
            {
                RoleTypeId.Tutorial,
                new AhpProccessBuild{
                    Amount = 60,
                    Regen = 1.5f,
                    Efficacy = 1,
                    Sustain = 5,
                }
            },
        };
        [Description("Permite modificar los valores predeterminados de los ItemType elegidos.")]
        public Dictionary<ItemType, ItemBuild> CustomItem { get; set; } = new()
        {
            {
                ItemType.KeycardJanitor, new ItemBuild
                {
                    Custom = new Dictionary<string, string>()
                    {
                        { nameof(Exiled.API.Features.Items.Keycard.Permissions), (KeycardPermissions.ContainmentLevelOne | KeycardPermissions.ArmoryLevelOne).ToString() },
                    }
                }
            },
            {
                ItemType.KeycardResearchCoordinator, new ItemBuild
                {
                    Custom = new Dictionary<string, string>()
                    {
                        { nameof(Exiled.API.Features.Items.Keycard.Permissions), KeycardPermissions.ContainmentLevelTwo.ToString() },
                    }
                }
            },
            {
                ItemType.Coin, new ItemBuild
                {
                    Custom = new Dictionary<string, string>()
                    {
                        { nameof(Exiled.API.Features.Items.Item.Scale), new Vector3(1, 2, 1).ToString() },
                    }
                }
            },
            {
                ItemType.GrenadeHE, new ItemBuild
                {
                    Custom = new Dictionary<string, string>()
                    {
                        { nameof(Exiled.API.Features.Items.ExplosiveGrenade.FuseTime), 1.ToString() },
                        { nameof(Exiled.API.Features.Items.ExplosiveGrenade.Scale), (Vector3.one * 2).ToString() },
                    }
                }
            },
        };

        [Description("Permite modificar los valores predeterminados de los RoleTypeId elegidos.")]
        public Dictionary<RoleTypeId, RoleBuild> CustomRole { get; set; } = new()
        {
            {
                RoleTypeId.Scp3114, new RoleBuild
                {
                    Custom = new Dictionary<string, string>()
                    {
                        { nameof(Exiled.API.Features.Roles.Scp3114Role.DisguiseDuration), 600.ToString() },
                        { nameof(Exiled.API.Features.Roles.FpcRole.IsUsingStamina), false.ToString() },
                        { nameof(Exiled.API.Features.Roles.FpcRole.IsInvisible), true.ToString() },
                    },
                }
            },
        };
    }
}

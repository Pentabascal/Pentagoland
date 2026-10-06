namespace Common_Utilities
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using ConfigObjects;
    using Configs;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using Scp914;
    using UnityEngine;

    public class Config : IConfig
    {
        [Description("Indica si el plugin está activado.")]
        public bool IsEnabled { get; set; } = true;
        
        [Description("Indica si se deben mostrar mensajes de depuración.")]
        public bool Debug { get; set; } = false;

        [Description("Roles que, si son esposados en la zona de escape, cambiarán al rol indicado.")]
        public Dictionary<RoleTypeId, RoleTypeId> DisarmedEscapeSwitchRole { get; set; } =
            new()
            {
                {
                    RoleTypeId.NtfCaptain, RoleTypeId.ChaosMarauder
                },
                {
                    RoleTypeId.ChaosMarauder, RoleTypeId.NtfCaptain
                },
            };
        
        [Description("Texto que se muestra con el intervalo indicado a continuación.")]
        public string TimedBroadcast { get; set; } = "<color=#bfff00>Este servidor usa </color><color=red>EXILED Common-Utilities</color><color=#bfff00>. ¡Disfruta de tu estancia!</color>";

        [Description("Duración de cada mensaje periódico.")]
        public ushort TimedBroadcastDuration { get; set; } = 5;

        [Description("Tiempo entre mensajes periódicos. Para desactivarlos, establece este valor en 0.")]
        public float TimedBroadcastDelay { get; set; } = 0;

        [Description("Mensaje que se muestra al jugador cuando se conecta por primera vez al servidor. Déjalo vacío para desactivarlo.")]
        public string JoinMessage { get; set; } = string.Empty;

        [Description("Duración en segundos del mensaje de bienvenida.")]
        public ushort JoinMessageDuration { get; set; } = 5;

        [Description("Tiempo en segundos desde el inicio de la ronda hasta que se active automáticamente la ojiva. Establece el valor en -1 para desactivarla.")]
        public float AutonukeTime { get; set; } = -1;

        [Description("Indica si la ojiva debe quedar bloqueada durante la cuenta atrás de la activación automática.")]
        public bool AutonukeLock { get; set; } = true;

        [Description("Mensaje para todos los jugadores cuando se active la ojiva automáticamente. Con una duración de 2 o más, se mostrará en pantalla; con 1, será un anuncio de CASSIE; con 0, se desactivará.")]
        public Broadcast AutonukeBroadcast { get; set; } = new()
        {
            Content = "La ojiva se ha activado automáticamente.",
            Duration = 10,
            Show = true,
            Type = global::Broadcast.BroadcastFlags.Normal,
        };

        [Description("Indica si se debe mostrar la salud del jugador bajo su nombre al mirarlo.")]
        public bool PlayerHealthInfo { get; set; } = false;

        [Description("Indica si el fuego amigo debe activarse automáticamente al terminar la ronda (se desactivará antes de que empiece la siguiente).")]
        public bool FriendlyFireOnRoundEnd { get; set; } = false;

        [Description("Multiplicador del consumo de batería de la radio. Establece el valor en 0 para desactivar el consumo.")]
        public float RadioBatteryDrainMultiplier { get; set; } = 1f;

        [Description("Indica si se debe cambiar el color de las luces mientras la ojiva está activa.")] 
        public bool ChangeWarheadColor { get; set; } = false;
        
        [Description("Color de las luces mientras la ojiva está activa, en formato RGBA con valores entre 0 y 1. Se ignora si ChangeWarheadColor está desactivado.")]
        public Color WarheadColor { get; set; } = new(1f, 0.2f, 0.2f, 1);

        [Description("Tiempo máximo en segundos que un jugador puede permanecer inactivo antes de ser expulsado. Establece el valor en -1 para desactivar el sistema AFK.")]
        public int AfkLimit { get; set; } = -1;
        
        [Description("Roles que el sistema AFK debe ignorar.")]
        public List<RoleTypeId> AfkIgnoredRoles { get; set; } = new()
        {
            RoleTypeId.Scp079,
            RoleTypeId.Spectator,
            RoleTypeId.Tutorial,
            RoleTypeId.Filmmaker,
            RoleTypeId.Overwatch,
        };

        [Description("Indica si las probabilidades se suman (50 + 50 = 100) o se aplican por separado (50 + 50 = dos probabilidades del 50 %).")]
        public bool AdditiveProbabilities { get; set; } = false;

        [Description(
            "Lista de objetos iniciales por rol. ItemName indica el objeto que se entregará; Chance, la probabilidad porcentual de recibirlo; y Group permite limitarlo a jugadores de determinados grupos de Remote Admin (usa 'none' para permitirlo a todos). Puedes repetir un mismo objeto.")]
        public Dictionary<RoleTypeId, RoleInventory> StartingInventories { get; set; } = new()
        {
            {
                RoleTypeId.ClassD, new RoleInventory
                {
                    Slot1 = new List<ItemChance>
                    {
                        new()
                        {
                            ItemName = ItemType.KeycardJanitor.ToString(),
                            Chance = 10,
                            Group = "none",
                        },
                        new()
                        {
                            ItemName = ItemType.Coin.ToString(),
                            Chance = 100,
                            Group = "none",
                        },
                    },
                    Slot2 = new List<ItemChance>
                    {
                        new()
                        {
                            ItemName = ItemType.Flashlight.ToString(),
                            Chance = 100,
                            Group = "none",
                        },
                    },
                    Ammo = new List<StartingAmmo>
                    {
                        new()
                        {
                            Type = ItemType.Ammo556x45,
                            Amount = 200,
                            Group = "none",
                        },
                    },
                }
            },
        };

        [Description("Lista de recetas personalizadas de SCP-914. Original es el objeto que se mejora; New, el objeto resultante; y Chance, la probabilidad porcentual de que se produzca la mejora. Puedes definir varios resultados para un mismo objeto. Para objetos personalizados, usa su nombre.")]
        // This should be named Scp914ItemChanGes instead of ChanCes (capitalized to show the difference), consider changing at some point (breaking change)
        public Dictionary<Scp914KnobSetting, List<ItemUpgradeChance>> Scp914ItemChances { get; set; } = new()
        {
            {
                Scp914KnobSetting.Rough, new List<ItemUpgradeChance>
                {
                    new()
                    {
                        Original = ItemType.KeycardO5.ToString(),
                        New = ItemType.MicroHID.ToString(),
                        Chance = 50,
                    },
                }
            },
        };
        
        [Description("Lista de recetas personalizadas de SCP-914 para roles. Original es el rol que cambiará; New, el nuevo rol; y Chance, la probabilidad porcentual de que se produzca el cambio. Para roles personalizados, usa su nombre.")]
        public Dictionary<Scp914KnobSetting, List<PlayerUpgradeChance>> Scp914ClassChanges { get; set; } = new()
        {
            {
                Scp914KnobSetting.Rough, new List<PlayerUpgradeChance>
                {
                    new()
                    {
                        Original = RoleTypeId.ClassD.ToString(),
                        New = RoleTypeId.Spectator.ToString(),
                        Chance = 100,
                    },
                }
            },
        };

        [Description("Lista de opciones de teletransporte de SCP-914. Si estableces \"zone\" en un valor distinto de Unspecified, se elegirá una sala aleatoria de esa zona que no figure en ignoredRooms, en lugar del tipo de sala indicado.")]
        public Dictionary<Scp914KnobSetting, List<Scp914TeleportChance>> Scp914TeleportChances { get; set; } = new()
        {
            {
                Scp914KnobSetting.Rough, new List<Scp914TeleportChance>
                {
                    new()
                    {
                        Room = RoomType.LczClassDSpawn,
                        Offset = Vector3.up,
                        Chance = 50,
                    },
                    new()
                    {
                        Zone = ZoneType.LightContainment,
                        IgnoredRooms = new()
                        {
                            RoomType.Lcz173,
                        },
                        Chance = 100,
                    },
                }
            },
        };

        [Description("Efectos aleatorios que se aplicarán a los jugadores al pasar por SCP-914 con determinados ajustes.")]
        public Dictionary<Scp914KnobSetting, List<Scp914EffectChance>> Scp914EffectChances { get; set; } = new()
        {
            {
                Scp914KnobSetting.Rough, new List<Scp914EffectChance>
                {
                    new()
                    {
                        Effect = EffectType.Bleeding,
                        Chance = 100,
                    },
                }
            },
        };

        [Description("Indica si los efectos de SCP-914 son excluyentes, es decir, si solo se puede aplicar uno cada vez que se procese a un jugador.")]
        public bool Scp914EffectsExclusivity { get; set; } = false;

        [Description("Indica si los SCP son inmunes a los efectos obtenidos con SCP-914.")]
        public bool ScpsImmuneTo914Effects { get; set; } = false;

        [Description("Intervalo en segundos entre limpiezas de cadáveres. Establece el valor en 0 para desactivarlas.")]
        public float RagdollCleanupDelay { get; set; } = 0f;

        [Description("Indica si los cadáveres solo deben limpiarse en la Dimensión de Bolsillo.")]
        public bool RagdollCleanupOnlyPocket { get; set; } = false;
        
        [Description("Intervalo en segundos entre limpiezas de objetos. Establece el valor en 0 para desactivarlas.")]
        public float ItemCleanupDelay { get; set; } = 0f;

        [Description("Indica si los objetos solo deben limpiarse en la Dimensión de Bolsillo.")]
        public bool ItemCleanupOnlyPocket { get; set; } = false;
        
        [Description("Modificadores del daño infligido por cada rol. El valor es un multiplicador, no una cantidad de daño: 1 = daño normal, 1,5 = un 50 % más y 0,5 = un 50 % menos.")]
        public Dictionary<RoleTypeId, float> RoleDamageDealtMultipliers { get; set; } = new()
        {
            {
                RoleTypeId.Scp173, 1.0f
            },
        };

        [Description("Multiplicadores del daño recibido por cada rol: 1 = daño normal, 1,5 = un 50 % más y 0,5 = un 50 % menos.")]
        public Dictionary<RoleTypeId, float> RoleDamageReceivedMultipliers { get; set; } = new()
        {
            {
                RoleTypeId.Scp173, 1.0f
            },
        };
        
        [Description("Modificadores del daño de cada arma. El valor es un multiplicador, no una cantidad de daño: 1 = daño normal, 1,5 = un 50 % más y 0,5 = un 50 % menos.")]
        public Dictionary<DamageType, float> DamageMultipliers { get; set; } = new()
        {
            {
                DamageType.E11Sr, 1.0f
            },
        };

        [Description("Salud que recuperará cada rol al matar a alguien.")]
        public Dictionary<RoleTypeId, float> HealthOnKill { get; set; } = new()
        {
            {
                RoleTypeId.Scp173, 0
            },
        };
        
        [Description("Salud inicial predeterminada de cada rol.")]
        public Dictionary<RoleTypeId, int> HealthValues { get; set; } = new();
    }
}

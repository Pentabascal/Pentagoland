using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using PlayerRoles;

namespace VVUP.ScpChanges
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        
        public string Scp1576Text { get; set; } = "<size=24><align=left>Espectadores: %spectators%. Tiempo hasta la siguiente oleada: %timebeforespawnwave% segundos</align></size>";
        public float Scp1576TextDuration { get; set; } = 15f;
        [Description("Añade %customroles% a Scp1576Text para mostrar los roles personalizados al usarlo.")]
        public Dictionary<uint, string> Scp1576CustomRolesAlive { get; set; } = new()
        {
            { 25, "<color=#ff00ff><size=30>Guardia de la Mano de la Serpiente</color></size>" },
            { 26, "<color=#ff00ff><size=30>Encantador de la Mano de la Serpiente</color></size>" },
            { 27, "<color=#ff00ff><size=30>Agente de la Mano de la Serpiente</color></size>" },
        };
        [Description("Añade %roles% a Scp1576Text para mostrar los roles con vida al usarlo.")]
        public Dictionary<RoleTypeId, string>AliveRoles { get; set; } = new()
        {
            { RoleTypeId.Scp049, "<color=#ff0000><size=30>SCP-049</color></size>" },
            { RoleTypeId.Scp0492, "<color=#ff0000><size=30>SCP-049-2</color></size>" },
            { RoleTypeId.Scp096, "<color=#ff0000><size=30>SCP-096</color></size>" },
            { RoleTypeId.Scp173, "<color=#ff0000><size=30>SCP-173</color></size>" },
            { RoleTypeId.Scp106, "<color=#ff0000><size=30>SCP-106</color></size>" },
            { RoleTypeId.Scp939, "<color=#ff0000><size=30>SCP-939</size></color></size>" },
        };
        [Description("Añade %teams% a Scp1576Text para mostrar los equipos con jugadores vivos al usarlo.")]
        public Dictionary<Team, string> AliveTeams { get; set; } = new()
        {
            { Team.FoundationForces, "<size=30>MTF</size>" },
            { Team.ChaosInsurgency, "<size=30>Insurgencia del Caos</size>" },
            { Team.Scientists, "<size=30>Científicos</size>" },
            { Team.ClassD, "<size=30>Clase-D</size>" },
            { Team.SCPs, "<size=30>SCP</size>" },
        };
        [Description("El comportamiento antiguo de SCP-106 reduce su salud, pero le otorga resistencia al daño de las balas.")]
        public bool OldScp106Behavior { get; set; } = false;
        public int Scp106Health { get; set; } = 600;
        [Description("0,1 = 90 % de resistencia; 0,2 = 80 % de resistencia, etc.")]
        public float Scp106DamageResistance { get; set; } = 0.1f;
        [Description("Si SCP-106 tiene resistencia al daño, ¿también debe aplicarse a su escudo Hume?")]
        public bool ResistanceWithHume { get; set; } = false;
        [Description("Aplica el efecto de bolsillo de un solo golpe a todos los ataques de SCP-106. Debe permanecer desactivado si solo una subclase debe capturar de un golpe.")]
        public bool Scp106OneShot { get; set; } = false;
        [Description("Mata de un solo golpe con cualquier ataque de SCP-049. Debe permanecer desactivado si solo una subclase debe hacerlo.")]
        public bool Scp049OneShot { get; set; } = false;
        [Description("¿SCP-096 obtiene furia ilimitada cuando detona la ojiva?")]
        public bool Scp096UnlimitedRageDuringNuke { get; set; } = true;
        [Description("¿Se deben aplicar los ajustes de los Flamencos?")]
        public bool FlamingoAdjustments { get; set; } = true;
        [Description("¿Cuánto daño base inflige el Flamenco?")]
        public float FlamingoBaseDamage { get; set; } = 12f;
        [Description("¿Cuál es el multiplicador de daño de los Flamencos contra los SCP?")]
        public float FlamingoScpDamageMultiplier { get; set; } = 1.5f;
    }
}
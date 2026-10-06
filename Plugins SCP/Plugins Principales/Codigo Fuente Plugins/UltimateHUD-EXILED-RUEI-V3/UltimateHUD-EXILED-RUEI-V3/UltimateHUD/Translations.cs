using Exiled.API.Enums;
using Exiled.API.Interfaces;
using PlayerRoles;
using System.Collections.Generic;

namespace UltimateHUD
{
    public class RoleName
    {
        public RoleTypeId Role { get; set; }
        public string Name { get; set; }
    }

    public class CustomRoleName
    {
        public string CustomRole { get; set; }
        public string Name { get; set; }
    }

    public class WeaponName
    {
        public FirearmType Weapon { get; set; }
        public string Name { get; set; }
    }

    public class WarheadStatusName
    {
        public WarheadStatus Status { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
    public class Translations : ITranslation
    {
        public List<WarheadStatusName> WarheadStatuses { get; set; } = new List<WarheadStatusName>()
        {
            new WarheadStatusName { Status = WarheadStatus.Armed,      Name = "Armada",      Color = "red" },
            new WarheadStatusName { Status = WarheadStatus.NotArmed,   Name = "No armada",   Color = "green" },
            new WarheadStatusName { Status = WarheadStatus.InProgress, Name = "En curso",    Color = "orange" },
            new WarheadStatusName { Status = WarheadStatus.Detonated,  Name = "Detonada",    Color = "#8B0000" },
        };

        public List<RoleName> GameRoles { get; set; } = new List<RoleName>()
        {
            new RoleName { Role = RoleTypeId.Tutorial, Name = "Tutorial" },
            new RoleName { Role = RoleTypeId.ClassD, Name = "Clase D" },
            new RoleName { Role = RoleTypeId.Scientist, Name = "Científico" },
            new RoleName { Role = RoleTypeId.FacilityGuard, Name = "Guardia de seguridad" },
            new RoleName { Role = RoleTypeId.Filmmaker, Name = "Cineasta" },
            new RoleName { Role = RoleTypeId.Overwatch, Name = "Observador" },
            new RoleName { Role = RoleTypeId.NtfPrivate, Name = "Soldado de la Fundación" },
            new RoleName { Role = RoleTypeId.NtfSergeant, Name = "Sargento de la Fundación" },
            new RoleName { Role = RoleTypeId.NtfSpecialist, Name = "Especialista de la Fundación" },
            new RoleName { Role = RoleTypeId.NtfCaptain, Name = "Capitán de la Fundación" },
            new RoleName { Role = RoleTypeId.ChaosConscript, Name = "Recluta de la Insurgencia" },
            new RoleName { Role = RoleTypeId.ChaosRifleman, Name = "Fusilero de la Insurgencia" },
            new RoleName { Role = RoleTypeId.ChaosRepressor, Name = "Hostigador de la Insurgencia" },
            new RoleName { Role = RoleTypeId.ChaosMarauder, Name = "Merodeador de la Insurgencia" },
            new RoleName { Role = RoleTypeId.Scp049, Name = "SCP-049" },
            new RoleName { Role = RoleTypeId.Scp0492, Name = "SCP-049-2" },
            new RoleName { Role = RoleTypeId.Scp079, Name = "SCP-079" },
            new RoleName { Role = RoleTypeId.Scp096, Name = "SCP-096" },
            new RoleName { Role = RoleTypeId.Scp106, Name = "SCP-106" },
            new RoleName { Role = RoleTypeId.Scp173, Name = "SCP-173" },
            new RoleName { Role = RoleTypeId.Scp939, Name = "SCP-939" },
            new RoleName { Role = RoleTypeId.Scp3114, Name = "SCP-3114" },
            
        };

        public List<WeaponName> WeaponName { get; set; } = new List<WeaponName>()
        {
            new WeaponName { Weapon = FirearmType.Com15, Name = "COM-15" },
            new WeaponName { Weapon = FirearmType.Com18, Name = "COM-18" },
            new WeaponName { Weapon = FirearmType.E11SR, Name = "Epsilon" },
            new WeaponName { Weapon = FirearmType.FSP9, Name = "FSP-9" },
            new WeaponName { Weapon = FirearmType.Logicer, Name = "Logicer" },
            new WeaponName { Weapon = FirearmType.Revolver, Name = "Revólver" },
            new WeaponName { Weapon = FirearmType.AK, Name = "AK" },
            new WeaponName { Weapon = FirearmType.Shotgun, Name = "Escopeta" },
            new WeaponName { Weapon = FirearmType.Com45, Name = "COM-45" },
            new WeaponName { Weapon = FirearmType.ParticleDisruptor, Name = "Disruptor de partículas X3" },
            new WeaponName { Weapon = FirearmType.FRMG0, Name = "FR-MG-0" },
            new WeaponName { Weapon = FirearmType.A7, Name = "A7" },
            new WeaponName { Weapon = FirearmType.Scp127, Name = "SCP-127" }
        };

        public List<CustomRoleName> CustomGameRoles { get; set; } = new List<CustomRoleName>()
        {
            new CustomRoleName { CustomRole = "Serpents Hand Agent", Name = "Agente de la Mano de la Serpiente" },
            new CustomRoleName { CustomRole = "Serpents Hand Guardian", Name = "Guardián de la Mano de la Serpiente" },
            new CustomRoleName { CustomRole = "Serpents Hand Enchanter", Name = "Hechicero de la Mano de la Serpiente" }
        };
    }
}
using CommandSystem;
using SwiftArcadeMode.Features.SCPs.Upgrades;

namespace SwiftArcadeMode.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EnableSCPUpgradeCommand : ConfigCommand
    {
        public override string Command => "allowscpupgrades";

        public override string[] Aliases => ["scpupgrades", "scpup", "su"];

        public override string Description => "Activa o desactiva la obtención natural de mejoras para los SCP.";

        public override string Name => "Permitir mejoras de SCP";

        public override void ChangeOption(bool option) => UpgradePathGiver.AllowLeveling = option;
    }
}

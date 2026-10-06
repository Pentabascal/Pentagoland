using CommandSystem;
using SwiftArcadeMode.Features.Humans.Perks;

namespace SwiftArcadeMode.Commands.RA
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class EnableNaturalSpawningCommand : ConfigCommand
    {
        public override string Command => "allowperkspawning";

        public override string[] Aliases => ["spawnperks", "perkspawns", "pspawn", "aps"];

        public override string Description => "Activa o desactiva la aparición natural de ventajas (monedas grandes).";

        public override string Name => "Permitir aparición de ventajas";

        public override void ChangeOption(bool option) => PerkSpawner.AllowSpawn = option;
    }
}

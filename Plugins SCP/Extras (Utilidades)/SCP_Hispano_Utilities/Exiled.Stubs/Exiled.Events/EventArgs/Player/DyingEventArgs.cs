namespace Exiled.Events.EventArgs.Player
{
    public sealed class DyingEventArgs
    {
        public DyingEventArgs(global::Exiled.API.Features.Player player, global::Exiled.API.Features.Player attacker)
        {
            Player = player;
            Attacker = attacker;
        }

        public global::Exiled.API.Features.Player Player { get; }
        public global::Exiled.API.Features.Player Attacker { get; }
    }
}

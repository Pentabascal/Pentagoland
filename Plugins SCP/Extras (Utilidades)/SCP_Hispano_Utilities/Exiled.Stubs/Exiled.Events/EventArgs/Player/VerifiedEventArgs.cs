namespace Exiled.Events.EventArgs.Player
{
    public sealed class VerifiedEventArgs
    {
        public VerifiedEventArgs(global::Exiled.API.Features.Player player)
        {
            Player = player;
        }

        public global::Exiled.API.Features.Player Player { get; }
    }
}

namespace Exiled.Events.EventArgs.Player
{
    public sealed class ChattedEventArgs
    {
        public ChattedEventArgs(global::Exiled.API.Features.Player player, string message)
        {
            Player = player;
            Message = message;
        }

        public global::Exiled.API.Features.Player Player { get; }
        public string Message { get; }
    }
}

namespace SCPReplacer
{
    using System;
    using Exiled.API.Features;
    using SCPReplacer.Models;
    using PlayerHandlers = Exiled.Events.Handlers.Player;
    using ServerHandlers = Exiled.Events.Handlers.Server;

    /// <summary>
    /// The main plugin class.
    /// </summary>
    public class Plugin : Plugin<Config, Translation>
    {
        private const string LegacyLotteryBroadcast = "{0} has left! Type .volunteer {1} within {2} seconds for a chance to take their place!";

        private EventHandlers? eventHandlers;

        /// <summary>
        /// Gets the only existing instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin? Instance { get; private set; }

        /// <inheritdoc/>
        public override string Author => "Storption";

        /// <inheritdoc/>
        public override string Name => "SCPReplacer";

        /// <inheritdoc/>
        public override string Prefix => "SCPReplacer";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(9, 14, 2);

        /// <inheritdoc/>
        public override Version Version { get; } = new Version(1, 2, 1);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            if (Translation.LotteryOpenedBroadcast == LegacyLotteryBroadcast)
                Log.Warn("Translation file reset recommended: it still contains the pre-1.2.0 lottery message. Delete SCPReplacer's translation file (EXILED/Configs/Translations/SCPReplacer/<port>.yml) and restart to regenerate it with the updated text. If you use a single merged translations file, delete only the SCPReplacer section.");

            eventHandlers = new EventHandlers();
            PlayerHandlers.Left += eventHandlers.OnLeft;
            ServerHandlers.RoundStarted += eventHandlers.OnRoundStarted;
            ServerHandlers.WaitingForPlayers += eventHandlers.OnWaitingForPlayers;
            ServerHandlers.RoundEnded += eventHandlers.OnRoundEnded;

            Modules.AutoUpdate.RegisterEvents();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            if (eventHandlers is not null)
            {
                PlayerHandlers.Left -= eventHandlers.OnLeft;
                ServerHandlers.RoundStarted -= eventHandlers.OnRoundStarted;
                ServerHandlers.WaitingForPlayers -= eventHandlers.OnWaitingForPlayers;
                ServerHandlers.RoundEnded -= eventHandlers.OnRoundEnded;
            }

            ScpToReplace.ClearAll();

            Modules.AutoUpdate.UnregisterEvents();

            eventHandlers = null;
            Instance = null;

            base.OnDisabled();
        }
    }
}
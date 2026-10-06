namespace SCPReplacer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Enums;
    using Exiled.CustomRoles.API;
    using Exiled.CustomRoles.API.Features;
    using MEC;
    using PlayerRoles;

    /// <summary>
    /// Tracks a single SCP currently awaiting replacement via the volunteer lottery.
    /// </summary>
    public class ScpToReplace
    {
        private static readonly List<ScpToReplace> Pending = new();

        private CoroutineHandle timer;

        private ScpToReplace(RoleTypeId role, string formerUserId)
        {
            Role = role;
            FormerUserId = formerUserId;
            Volunteers = new List<Player>();
        }

        /// <summary>
        /// Gets the SCP role this replacement is for.
        /// </summary>
        public RoleTypeId Role { get; }

        /// <summary>
        /// Gets the SCP number this replacement is for (e.g. "079").
        /// </summary>
        public string Name => Role.ScpNumber();

        /// <summary>
        /// Gets the user ID of the player who left or gave up this SCP, who can't volunteer for it again.
        /// </summary>
        public string FormerUserId { get; }

        /// <summary>
        /// Gets the players who have volunteered so far.
        /// </summary>
        public List<Player> Volunteers { get; }

        /// <summary>
        /// Gets a value indicating whether anything is currently awaiting replacement.
        /// </summary>
        public static bool AnyPending => Pending.Count > 0;

        /// <summary>
        /// Gets the SCP numbers currently awaiting replacement.
        /// </summary>
        public static IEnumerable<string> PendingNames => Pending.Select(r => r.Name);

        /// <summary>
        /// Finds a pending replacement by SCP number, if one exists.
        /// </summary>
        public static ScpToReplace? Find(string scpName)
        {
            return Pending.FirstOrDefault(r => Util.SameScp(r.Name, scpName));
        }

        /// <summary>
        /// Registers a new SCP as awaiting replacement.
        /// </summary>
        public static ScpToReplace Create(RoleTypeId scpRole, string formerUserId)
        {
            ScpToReplace replacement = new(scpRole, formerUserId);
            replacement.timer = Timing.CallDelayed(Plugin.Instance!.Config.LotteryPeriodSeconds, replacement.Resolve);
            Pending.Add(replacement);
            return replacement;
        }

        /// <summary>
        /// Opens the volunteer lottery for a vacated SCP role and announces it to everyone.
        /// </summary>
        public static void Open(RoleTypeId scpRole, string formerUserId)
        {
            string scpNumber = scpRole.ScpNumber();
            if (Find(scpNumber) is not null)
                return;

            Create(scpRole, formerUserId);

            Config config = Plugin.Instance!.Config;
            Translation translation = Plugin.Instance!.Translation;
            string message = translation.BroadcastHeader + string.Format(translation.LotteryOpenedBroadcast, scpRole.ColoredScpLabel(), scpNumber, config.LotteryPeriodSeconds);

            if (config.Debug)
                Log.Debug($"Lottery opened for SCP-{scpNumber}, resolving in {config.LotteryPeriodSeconds}s.");

            Broadcast broadcast = new(message, (ushort)config.LotteryPeriodSeconds);
            foreach (Player p in Player.List)
                p.Broadcast(broadcast);
        }

        /// <summary>
        /// Cancels and clears every pending replacement - called on round start and end, when returning to the lobby, and when the plugin is disabled.
        /// </summary>
        public static void ClearAll()
        {
            foreach (ScpToReplace role in Pending)
                Timing.KillCoroutines(role.timer);

            Pending.Clear();
        }

        private void Resolve()
        {
            Pending.Remove(this);

            Player? chosen = Volunteers
                .Where(p => p.CanVolunteer())
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefault();

            Config config = Plugin.Instance!.Config;
            Translation translation = Plugin.Instance!.Translation;

            if (chosen is null)
            {
                if (config.Debug)
                    Log.Debug($"No eligible volunteers for SCP-{Name} ({Volunteers.Count} entered) - it will not be replaced.");

                string noVolunteersMessage = translation.BroadcastHeader + translation.LotteryNoVolunteers;
                foreach (Player p in Player.List)
                    p.Broadcast(new Broadcast(noVolunteersMessage, 5));
                return;
            }

            if (config.Debug)
                Log.Debug($"{chosen.Nickname} won the lottery for SCP-{Name} out of {Volunteers.Count} volunteer(s).");

            foreach (CustomRole customRole in chosen.GetCustomRoles())
                customRole.RemoveRole(chosen);

            chosen.DisableAllEffects();
            chosen.Role.Set(Role, SpawnReason.LateJoin);

            string coloredScpLabel = Role.ColoredScpLabel();

            foreach (Player p in Player.List)
            {
                string message = p == chosen
                    ? translation.BroadcastHeader + string.Format(translation.LotteryWon, coloredScpLabel)
                    : translation.BroadcastHeader + string.Format(translation.ReplacementAnnouncement, coloredScpLabel);

                p.Broadcast(new Broadcast(message, 5));
            }
        }
    }
}
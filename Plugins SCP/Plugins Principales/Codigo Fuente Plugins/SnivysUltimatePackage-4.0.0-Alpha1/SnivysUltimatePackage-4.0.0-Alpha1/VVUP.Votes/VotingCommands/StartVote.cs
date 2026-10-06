using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using ICommand = CommandSystem.ICommand;

namespace VVUP.Votes.VotingCommands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class StartVote : ICommand
    {
        public string Command { get; } = "StartVote";
        public string[] Aliases { get; } = { "SV" };
        public string Description { get; } = "Starts a vote, shown to all players";
        public static bool IsVoteActive { get; set; } = false;
        public static Dictionary<int, string> VoteOptions = new Dictionary<int, string>();
        public static Dictionary<string, int> PlayerVotes = new Dictionary<string, int>();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Config.IsEnabled)
            {
                response = "This command is disabled.";
                return false;
            }

            if (!sender.CheckPermission("vvvotes.start"))
            {
                response = "You do not have permission to use this command.";
                return false;
            }

            if (IsVoteActive)
            {
                response = "A vote is already active, please wait until it finishes.";
                return false;
            }

            if (arguments.Count < 3)
            {
                response =
                    "You provided an invalid amount of arguments. \n Order: <Vote Name> | <Option 1> | <Option 2> [| Option 3-5]";
                return false;
            }

            string input = string.Join(" ", arguments);
            string[] parts = input.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()).ToArray();
            
            if (parts.Length < 3)
            {
                response =
                    "You must provide a vote name and at least two options. \n Order: <Vote Name> | <Option 1> | <Option 2> [| Option 3-5]";
                return false;
            }

            string votePrompt = parts[0];
            string[] options = parts.Skip(1).ToArray();

            if (options.Length < 2 || options.Length > 5)
            {
                response = "You must provide between 2 and 5 options.";
                return false;
            }

            VoteOptions.Clear();
            PlayerVotes.Clear();

            for (int i = 0; i < options.Length; i++)
            {
                VoteOptions[i + 1] = options[i];
            }

            IsVoteActive = true;

            string broadcastText = Plugin.Instance.Config.StartVoteMapBroadcast.Replace("%prompt%", votePrompt);

            for (int i = 1; i <= 5; i++)
            {
                broadcastText = broadcastText.Replace($"%option{i}%", VoteOptions.ContainsKey(i) ? $".vote {i}: {VoteOptions[i]}," : string.Empty);
            }

            Map.Broadcast(Plugin.Instance.Config.MapBroadcastTime, broadcastText,
                Broadcast.BroadcastFlags.Normal, true);

            Timing.CallDelayed(Plugin.Instance.Config.VoteDuration, () =>
            {
                var results = PlayerVotes.GroupBy(x => x.Value)
                    .Select(group => new { Option = group.Key, Count = group.Count() })
                    .OrderByDescending(x => x.Count);

                string resultMessage = Plugin.Instance.Config.EndVoteMapBroadcast;
                string resultsText = results.Aggregate(string.Empty, (current, result) => current + $" <size=30>{VoteOptions[result.Option]}: {result.Count} votes.</size>");

                resultMessage = resultMessage.Replace("%results%", resultsText);

                Map.Broadcast(Plugin.Instance.Config.MapBroadcastTime, resultMessage,
                    Broadcast.BroadcastFlags.Normal, true);
                IsVoteActive = false;
            });

            response = "You have started a vote.";
            Log.Debug($"VVUP Votes: {sender.LogName} started a vote with prompt: {votePrompt}");
            return true;
        }
    }
}

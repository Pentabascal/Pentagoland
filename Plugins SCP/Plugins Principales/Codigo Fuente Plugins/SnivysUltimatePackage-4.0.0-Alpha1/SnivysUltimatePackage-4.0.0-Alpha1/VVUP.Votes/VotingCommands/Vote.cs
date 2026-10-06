using System;
using CommandSystem;
using Exiled.API.Features;

namespace VVUP.Votes.VotingCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Vote : ICommand
    {
        public string Command { get; } = "vote";
        public string[] Aliases { get; } = Array.Empty<string>();
        public string Description { get; } = "Votes during a vote";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Config.IsEnabled)
            {
                response = "The vote system is disabled.";
                return false;
            }

            if (!StartVote.IsVoteActive)
            {
                response = "There is no active vote.";
                return false;
            }

            if (arguments.Count != 1 || !int.TryParse(arguments.At(0), out int voteOption))
            {
                response = "Usage: .vote <option number>";
                return false;
            }

            if (!StartVote.VoteOptions.ContainsKey(voteOption))
            {
                response = "Invalid vote option. Please choose a valid option.";
                return false;
            }

            string playerId = ((CommandSender)sender).SenderId;
            if (StartVote.PlayerVotes.ContainsKey(playerId))
            {
                response = "You have already voted!";
                return false;
            }

            StartVote.PlayerVotes[playerId] = voteOption;
            response = $"You voted for: {StartVote.VoteOptions[voteOption]}";
            Log.Debug($"VVUP Votes: {sender.LogName} responded to the active vote");
            return true;
        }
    }
}
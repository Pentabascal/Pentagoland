#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#else
using Callvote.Commands.ParentCommands;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
#endif
using System;
using System.Linq;
using Callvote.API.Features.Votes;
using CommandSystem;

namespace Callvote.Commands.MiscellaneousCommands
{
#if !EXILED
    [CommandHandler(typeof(CallVoteParentCommand))]
#endif
    public class RigCommand : ICommand
    {
        public string Command => "rig";

        public string[] Aliases => ["r"];

        public string Description => "Permite amañar la votación actual.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);
#if EXILED
            if ((player != null && !player.CheckPermission("cv.superadmin+")) || (player == null && sender is not ServerConsoleSender))
#else
            if ((player != null && !player.HasPermissions("cv.superadmin+")) || (player == null && sender is not ServerConsoleSender))
#endif
            {
                response = CallvotePlugin.Instance.Translation.NoPermission;
                return false;
            }

            if (!VoteHandler.IsVoteActive)
            {
                response = CallvotePlugin.Instance.Translation.NoVoteInProgress;
                return false;
            }

            if (arguments.Count == 0)
            {
                response = CallvotePlugin.Instance.Translation.WrongSyntax;
                return false;
            }

            if (arguments.Count == 1)
            {
                if (!VoteHandler.CurrentVote.Rig(arguments.ElementAt(0), out VoteOption v))
                {
                    response = CallvotePlugin.Instance.Translation.NoOptionAvailable.Replace("%Option%", arguments.ElementAt(0));
                    return false;
                }

                response = $"Se ha amañado 1 voto a favor de {v.Detail}.";
                return true;
            }

            if (!int.TryParse(arguments.ElementAt(1), out int votes))
            {
                response = CallvotePlugin.Instance.Translation.InvalidArgument;
                return false;
            }

            if (!VoteHandler.CurrentVote.Rig(arguments.ElementAt(0), out VoteOption vote, votes))
            {
                response = CallvotePlugin.Instance.Translation.NoOptionAvailable.Replace("%Option%", arguments.ElementAt(0));
                return false;
            }

            response = $"Se han amañado {votes} votos a favor de {vote.Detail}.";
            return true;
        }
    }
}
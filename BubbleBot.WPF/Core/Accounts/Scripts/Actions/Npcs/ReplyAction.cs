using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Npcs
{
    public class ReplyAction : ScriptAction
    {
        // Constructor
        public ReplyAction(int replyId)
        {
            ReplyId = replyId;
        }

        // Properties
        public int ReplyId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Npcs.Reply(ReplyId))
            {
                account.Scripts.StopScript(LanguageManager.Translate("180", ReplyId));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}
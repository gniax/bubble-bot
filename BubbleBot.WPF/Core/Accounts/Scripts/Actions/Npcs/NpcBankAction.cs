using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Npcs
{
    public class NpcBankAction : ScriptAction
    {
        // Constructor
        public NpcBankAction(int npcId, int replyId)
        {
            NpcId = npcId;
            ReplyId = replyId;
        }

        // Properties
        public int NpcId { get; }
        public int ReplyId { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Npcs.UseNpc(NpcId, 1))
            {
                account.Scripts.StopScript(LanguageManager.Translate("179"));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}
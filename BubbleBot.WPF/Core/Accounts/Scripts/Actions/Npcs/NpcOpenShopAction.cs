using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Npcs
{
    public class NpcOpenShopAction : ScriptAction
    {
        // Constructor
        public NpcOpenShopAction(int npcId, uint actionIndex)
        {
            NpcId = npcId;
            ActionIndex = actionIndex;
        }

        // Properties
        public int NpcId { get; }
        public uint ActionIndex { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Npcs.UseNpc(NpcId, (int)ActionIndex))
            {
                account.Scripts.StopScript(LanguageManager.Translate("178", NpcId, ActionIndex));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}

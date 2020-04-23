using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class UseAction : ScriptAction
    {
        // Constructor
        public UseAction(short elementCellId, int skillInstanceUid)
        {
            ElementCellId = elementCellId;
            SkillInstanceUid = skillInstanceUid;
        }

        // Properties
        public short ElementCellId { get; }
        public int SkillInstanceUid { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Managers.Interactives.UseInteractive(ElementCellId, SkillInstanceUid))
            {
                account.Scripts.StopScript(LanguageManager.Translate("176", ElementCellId));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}
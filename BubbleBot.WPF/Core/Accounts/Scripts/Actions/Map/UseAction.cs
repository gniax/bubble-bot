using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class UseAction : ScriptAction
    {

        // Properties
        public short ElementCellId { get; private set; }
        public int SkillInstanceUid { get; private set; }


        // Constructor
        public UseAction(short elementCellId, int skillInstanceUid)
        {
            ElementCellId = elementCellId;
            SkillInstanceUid = skillInstanceUid;
        }

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

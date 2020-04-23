using System.Threading.Tasks;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Map
{
    public class UseByIdAction : ScriptAction
    {
        // Constructor
        public UseByIdAction(int elementId, int skillInstanceUid)
        {
            ElementId = elementId;
            SkillInstanceUid = skillInstanceUid;
        }

        // Properties
        public int ElementId { get; }
        public int SkillInstanceUid { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (!account.Game.Managers.Interactives.UseInteractive(ElementId, SkillInstanceUid))
            {
                account.Scripts.StopScript(LanguageManager.Translate("177", ElementId));
                return FailedResult;
            }

            return ProcessingResult;
        }
    }
}
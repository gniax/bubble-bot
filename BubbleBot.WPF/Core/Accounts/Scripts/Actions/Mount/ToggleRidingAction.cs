using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Mount
{
    public class ToggleRidingAction : ScriptAction
    {

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Character.Mount.HasMount)
            {
                account.Game.Character.Mount.ToggleRiding();
                await Task.Delay(1000);
            }

            return ScriptActionResults.DONE;
        }

    }
}

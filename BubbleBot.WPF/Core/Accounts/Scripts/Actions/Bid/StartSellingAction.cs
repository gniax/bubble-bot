using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Bid
{
    public class StartSellingAction : ScriptAction
    {

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Bid.StartSelling())
            {
                return ProcessingResult;
            }

            return DoneResult;
        }

    }
}

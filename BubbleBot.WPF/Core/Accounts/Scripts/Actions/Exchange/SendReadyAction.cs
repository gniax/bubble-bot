using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class SendReadyAction : ScriptAction
    {
        public SendReadyAction(Account source)
        {
            Source = source;
        }
        public SendReadyAction() { }
        // Properties
        public Account Source { get; }

        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (Source != null)
            {
               if (account.HasGroup && !account.IsGroupChief)
                    return DoneResult;

                Source.Game.Exchange.SendReady();
            }
            else account.Game.Exchange.SendReady();

            return DoneResult;
        }
    }
}
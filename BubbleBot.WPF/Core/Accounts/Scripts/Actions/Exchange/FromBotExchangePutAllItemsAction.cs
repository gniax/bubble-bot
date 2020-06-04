using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class FromBotExchangePutAllItemsAction : ScriptAction
    {
        // Constructor
        public FromBotExchangePutAllItemsAction(string groupmng, string idmng)
        {
            GroupMng = groupmng;
            IdMng = idmng;
        }

        //Property
        public string GroupMng { get; }
        public string IdMng { get; }

        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return ScriptActionResults.DONE;

            if (await account.Game.Exchange.FromBotPutAllItems(GroupMng, IdMng)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}
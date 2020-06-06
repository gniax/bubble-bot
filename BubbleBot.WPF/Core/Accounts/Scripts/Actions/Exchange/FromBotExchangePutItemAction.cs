using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class FromBotExchangePutItemAction : ScriptAction
    {
        // Constructor
        public FromBotExchangePutItemAction(string groupmng, string idmng, int gid, uint qty)
        {
            GroupMng = groupmng;
            IdMng = idmng;
            GID = gid;
            Quantity = qty;
        }

        //Property
        public string GroupMng { get; }
        public string IdMng { get; }
        public int GID { get; }
        public uint Quantity { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return ScriptActionResults.DONE;

            if (account.Game.Exchange.FromBotPutItem(GroupMng, IdMng, GID, Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}
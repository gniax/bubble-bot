using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Exchange
{
    public class FromBotExchangePutKamasAction : ScriptAction
    {
        // Constructor
        public FromBotExchangePutKamasAction(string groupmng, string idmng, uint qty)
        {
            GroupMng = groupmng;
            IdMng = idmng;
            Quantity = qty;
        }

        // Properties
        public string GroupMng { get; }
        public string IdMng { get; }
        public uint Quantity { get; }

    internal override async Task<ScriptActionResults> Process(Account account)
        {
            if (account.HasGroup && !account.IsGroupChief)
                return ScriptActionResults.DONE;

            if (account.Game.Exchange.FromBotPutKamas(GroupMng, IdMng,Quantity)) await Task.Delay(2000);

            return ScriptActionResults.DONE;
        }
    }
}
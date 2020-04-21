using System.Threading.Tasks;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class JoinFriendAction : ScriptAction
    {
        // Constructor
        public JoinFriendAction(string name)
        {
            Name = name;
        }

        // Properties
        public string Name { get; }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.IsBusy)
                return FailedResult;

            account.Network.SendMessage(new FriendJoinRequestMessage(Name));
            return DoneResult;
        }
    }
}
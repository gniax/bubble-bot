using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Manager
{

    public class GetAccountId : ScriptAction
    {

        // Properties
        public string PlayerManagerGroupe { get; private set; }
        public string PlayerManagerIdentifiant { get; private set; }


        // Constructor
        public GetAccountId(string playerMngGrp, string playerMngId)
        {
            PlayerManagerGroupe = playerMngGrp;
            PlayerManagerIdentifiant = playerMngId;

        }


        internal override Task<ScriptActionResults> Process(Account account)
        {
            if (account.Game.Exchange.GetAccountId(PlayerManagerGroupe, PlayerManagerIdentifiant))
                return ProcessingResult;

            return DoneResult;
        }

    }
}

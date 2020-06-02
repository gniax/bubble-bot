using BubbleBot.Configurations;
using Org.BouncyCastle.Asn1.Cms;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BubbleBot.Core.Accounts.Scripts.Actions.Global
{
    public class WaitSignalAction : ScriptAction
    {
        // Constructor
        public WaitSignalAction(string msg, int timeout)
        {
            Message = msg;
            Timeout = timeout;
        }

        // Properties
        public string Message { get; }
        public int Timeout { get; }


        internal override async Task<ScriptActionResults> Process(Account account)
        {
            account.Scripts.SignalMessage = ""; // Need to clear value just to be sure
            int i = 0;
            while (account.Scripts.SignalMessage != Message && i <= Timeout)
            {
                await Task.Delay(100);

                if (Timeout > 0)
                    i += 100;
            }

            if (account.Scripts.SignalMessage == Message)
            {
                account.Scripts.SignalMessage = "";
                return ScriptActionResults.DONE;
            }

            return ScriptActionResults.FAILED;
        }
    }
}
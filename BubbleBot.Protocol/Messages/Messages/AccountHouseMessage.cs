using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class AccountHouseMessage : Message
    {

        // Properties
        public List<AccountHouseInformations> Houses { get; set; }


        // Constructors
        public AccountHouseMessage() { }

        public AccountHouseMessage(List<AccountHouseInformations> houses = null)
        {
            Houses = houses;
        }

    }
}

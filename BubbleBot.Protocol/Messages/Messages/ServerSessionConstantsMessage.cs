using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class ServerSessionConstantsMessage : Message
    {

        // Properties
        public List<ServerSessionConstant> Variables { get; set; }


        // Constructors
        public ServerSessionConstantsMessage() { }

        public ServerSessionConstantsMessage(List<ServerSessionConstant> variables = null)
        {
            Variables = variables;
        }

    }
}

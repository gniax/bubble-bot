using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class StartupActionsListMessage : Message
    {

        // Properties
        public List<StartupActionAddObject> Actions { get; set; }


        // Constructors
        public StartupActionsListMessage() { }

        public StartupActionsListMessage(List<StartupActionAddObject> actions = null)
        {
            Actions = actions;
        }

    }
}

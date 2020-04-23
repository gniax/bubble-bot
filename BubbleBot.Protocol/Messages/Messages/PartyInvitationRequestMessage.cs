namespace BubbleBot.Protocol.Messages
{
    public class PartyInvitationRequestMessage : Message
    {

        // Properties
        public string Name { get; set; }


        // Constructors
        public PartyInvitationRequestMessage() { }

        public PartyInvitationRequestMessage(string name = "")
        {
            Name = name;
        }

    }
}

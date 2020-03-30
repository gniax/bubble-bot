using System.IO;

namespace BubbleBot.Server.Messages
{
    public class ReconnectSuccessMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 43;


        // Properties
        public short MessageId => ProtocolId;
        public string Name { get; private set; }
        public string Avatar { get; private set; }


        // Constructor
        public ReconnectSuccessMessage() { }


        public void Serialize(BinaryWriter writer)
        {

        }

        public void Deserialize(BinaryReader reader)
        {

        }

    }

}
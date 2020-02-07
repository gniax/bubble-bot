using System.IO;

namespace BubbleBot.Server.Messages
{
    public class ShowFunctionalitiesRequestMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 18;


        // Properties
        public short MessageId => ProtocolId;


        // Constructor
        public ShowFunctionalitiesRequestMessage() { }


        public void Serialize(BinaryWriter writer)
        {
        }

        public void Deserialize(BinaryReader reader)
        {
        }

    }
}
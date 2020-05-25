using System.IO;

namespace BubbleBot.Server.Messages
{
    public class IncompatibleVersionsMessage : IServerMessage
    {

        public const short ProtocolId = 53;

        // Fields
        public short MessageId => ProtocolId;

        public IncompatibleVersionsMessage() { }

        public void Serialize(BinaryWriter writer)
        {

        }

        public void Deserialize(BinaryReader reader)
        {

        }

    }
}

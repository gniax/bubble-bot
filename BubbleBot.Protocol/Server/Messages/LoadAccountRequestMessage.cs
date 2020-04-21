using System.IO;

namespace BubbleBot.Server.Messages
{
    public class LoadAccountRequestMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 45;


        // Properties
        public short MessageId => ProtocolId;
        public string Username { get; private set; }


        // Constructor
        public LoadAccountRequestMessage() { }

        public LoadAccountRequestMessage(string username)
        {
            Username = username;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Username);
        }

        public void Deserialize(BinaryReader reader)
        {
            Username = reader.ReadString();
        }

    }

}
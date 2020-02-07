using System.IO;

namespace BubbleBot.Server.Messages
{
    public class ConnectAccountRequestMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 8;


        // Properties
        public short MessageId => ProtocolId;
        public string Username { get; private set; }


        // Constructor
        public ConnectAccountRequestMessage() { }

        public ConnectAccountRequestMessage(string username)
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
using System.IO;

namespace BubbleBot.Server.Messages
{
    public class ConnectedAccountMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 44;


        // Properties
        public short MessageId => ProtocolId;
        public string Username { get; private set; }


        // Constructor
        public ConnectedAccountMessage() { }

        public ConnectedAccountMessage(string username)
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
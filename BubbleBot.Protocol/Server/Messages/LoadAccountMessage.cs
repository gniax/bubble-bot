using System.IO;

namespace BubbleBot.Server.Messages
{
    public class LoadAccountMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 47;


        // Properties
        public short MessageId => ProtocolId;
        public string Username { get; private set; }


        // Constructor
        public LoadAccountMessage() { }

        public LoadAccountMessage(string username)
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
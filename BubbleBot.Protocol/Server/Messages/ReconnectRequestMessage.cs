using System.IO;

namespace BubbleBot.Server.Messages
{
    public class ReconnectRequestMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 42;


        // Properties
        public short MessageId => ProtocolId;
        public string Username { get; private set; }
        public string Password { get; private set; }


        // Constructor
        public ReconnectRequestMessage() { }

        public ReconnectRequestMessage(string username, string password)
        {
            Username = username;
            Password = password;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Username);
            writer.Write(Password);
        }

        public void Deserialize(BinaryReader reader)
        {
            Username = reader.ReadString();
            Password = reader.ReadString();
        }

    }

}
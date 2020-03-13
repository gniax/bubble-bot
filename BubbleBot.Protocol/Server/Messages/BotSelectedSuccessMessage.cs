using System.IO;

namespace BubbleBot.Server.Messages
{
    public class BotSelectedSuccesMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 15;


        // Properties
        public short MessageId => ProtocolId;
        public string Account { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Server { get; private set; }
        public string Breed { get; private set; }
        public byte Level { get; private set; }


        // Constructor
        public BotSelectedSuccesMessage() { }

        public BotSelectedSuccesMessage(string account, int id, string name, string server, string breed, byte level)
        {
            Account = account;
            Id = id;
            Name = name;
            Server = server;
            Breed = breed;
            Level = level;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Account);
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Server);
            writer.Write(Breed);
            writer.Write(Level);
        }

        public void Deserialize(BinaryReader reader)
        {
            Account = reader.ReadString();
            Id = reader.ReadInt32();
            Name = reader.ReadString();
            Server = reader.ReadString();
            Breed = reader.ReadString();
            Level = reader.ReadByte();
        }

    }

}
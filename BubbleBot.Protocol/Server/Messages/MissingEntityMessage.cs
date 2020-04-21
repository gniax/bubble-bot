using System.IO;

namespace BubbleBot.Server.Messages
{
    public class MissingEntityMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 51;


        // Properties
        public short MessageId => ProtocolId;
        public string Type { get; private set; }
        public uint GID { get; private set; }
        public string Name { get; private set; }


        // Constructor
        public MissingEntityMessage() { }
        public MissingEntityMessage(string type, uint gid, string name)
        {
            Type = type;
            GID = gid;
            Name = name;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(GID);
            writer.Write(Name);
        }

        public void Deserialize(BinaryReader reader)
        {
            Type = reader.ReadString();
            GID = reader.ReadUInt32();
            Name = reader.ReadString();
        }

    }

}
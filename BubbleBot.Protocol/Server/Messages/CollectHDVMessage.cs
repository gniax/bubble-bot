using System.IO;

namespace BubbleBot.Server.Messages
{
    public class CollectHDVMessage : IServerMessage
    {

        public const short ProtocolId = 52;

        // Fields
        public short MessageId => ProtocolId;
        public int ObjectId { get; private set; }
        public string ObjectName { get; private set; }
        public string Server { get; private set; }
        public int PriceLot1 { get; private set; }
        public int PriceLot10 { get; private set; }
        public int PriceLot100 { get; private set; }
        public int AveragePrice { get; private set; }

        public CollectHDVMessage() { }
        
        public CollectHDVMessage(int objectid, string objectname, string server, int pricelot1, int pricelot10, int pricelot100, int averageprice)
        {
            ObjectId = objectid;
            ObjectName = objectname;
            Server = server;
            PriceLot1 = pricelot1;
            PriceLot10 = pricelot10;
            PriceLot100 = pricelot100;
            AveragePrice = averageprice;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(ObjectId);
            writer.Write(ObjectName);
            writer.Write(Server);
            writer.Write(PriceLot1);
            writer.Write(PriceLot10);
            writer.Write(PriceLot100);
            writer.Write(AveragePrice);
        }

        public void Deserialize(BinaryReader reader)
        {
            ObjectId = reader.ReadInt32();
            ObjectName = reader.ReadString();
            Server = reader.ReadString();
            PriceLot1 = reader.ReadInt32();
            PriceLot10 = reader.ReadInt32();
            PriceLot100 = reader.ReadInt32();
            AveragePrice = reader.ReadInt32();
        }

    }
}

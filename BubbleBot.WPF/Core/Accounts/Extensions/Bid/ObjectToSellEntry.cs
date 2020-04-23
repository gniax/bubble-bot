using System.IO;

namespace BubbleBot.Core.Accounts.Extensions.Bid
{
    public class ObjectToSellEntry
    {
        // Constructor
        public ObjectToSellEntry(string name, uint gid, uint lot, uint qty, uint minPrice, uint basePrice)
        {
            Name = name;
            GID = gid;
            Lot = lot;
            Quantity = qty;
            MinPrice = minPrice;
            BasePrice = basePrice;
        }

        // Properties
        public string Name { get; }
        public uint GID { get; }
        public uint Lot { get; }
        public uint Quantity { get; }
        public uint MinPrice { get; }
        public uint BasePrice { get; }


        public void Save(BinaryWriter bw)
        {
            bw.Write(Name);
            bw.Write(GID);
            bw.Write(Lot);
            bw.Write(Quantity);
            bw.Write(MinPrice);
            bw.Write(BasePrice);
        }

        public static ObjectToSellEntry Load(BinaryReader br)
        {
            return new ObjectToSellEntry(br.ReadString(), br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(),
                br.ReadUInt32(), br.ReadUInt32());
        }
    }
}
using System.IO;

namespace BubbleBot.Server.Messages
{
    public class BotInformationsMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 41;

        // Properties
        public short MessageId => ProtocolId;
        public string Account { get; private set; }
        public byte Level { get; private set; }
        public byte EnergyPercent { get; private set; }
        public byte WeightPercent { get; private set; }
        public int Kamas { get; private set; }
        public int MapId { get; private set; }
        public string MapPosition { get; private set; }
        public string State { get; private set; }


        // Constructor
        public BotInformationsMessage() { }

        public BotInformationsMessage(string account, byte level, byte energyPercent, byte weightPercent, int kamas, int mapId, string mapPosition, string state)
        {
            Account = account;
            Level = level;
            EnergyPercent = energyPercent;
            WeightPercent = weightPercent;
            Kamas = kamas;
            MapId = mapId;
            MapPosition = mapPosition;
            State = state;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Account);
            writer.Write(Level);
            writer.Write(EnergyPercent);
            writer.Write(WeightPercent);
            writer.Write(Kamas);
            writer.Write(MapId);
            writer.Write(MapPosition);
            writer.Write(State);
        }

        public void Deserialize(BinaryReader reader)
        {
            Account = reader.ReadString();
            Level = reader.ReadByte();
            EnergyPercent = reader.ReadByte();
            WeightPercent = reader.ReadByte();
            Kamas = reader.ReadInt32();
            MapId = reader.ReadInt32();
            MapPosition = reader.ReadString();
            State = reader.ReadString();
        }

    }

}
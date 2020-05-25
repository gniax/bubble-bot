using System.IO;

namespace BubbleBot.Server.Messages
{
    public class DTVersionsMessage : IServerMessage
    {

        // Fields
        public const short ProtocolId = 34;


        // Properties
        public short MessageId => ProtocolId;
        public string AppVersion { get; private set; }
        public string BuildVersion { get; private set; }
        public string AssetsVersion { get; private set; }
        public string StaticDataVersion { get; private set; }
        public bool IsFromUpdate { get; private set; }


        // Constructor
        public DTVersionsMessage() { }

        public DTVersionsMessage(string appVersion, string buildVersion, string assetsVersion, string staticDataVersion, bool isfromupdate = false)
        {
            AppVersion = appVersion;
            BuildVersion = buildVersion;
            AssetsVersion = assetsVersion;
            StaticDataVersion = staticDataVersion;
            IsFromUpdate = isfromupdate;
        }


        public void Serialize(BinaryWriter writer)
        {
            writer.Write(AppVersion);
            writer.Write(BuildVersion);
            writer.Write(AssetsVersion);
            writer.Write(StaticDataVersion);
            writer.Write(IsFromUpdate);
        }

        public void Deserialize(BinaryReader reader)
        {
            AppVersion = reader.ReadString();
            BuildVersion = reader.ReadString();
            AssetsVersion = reader.ReadString();
            StaticDataVersion = reader.ReadString();
            IsFromUpdate = reader.ReadBoolean();
        }

    }

}
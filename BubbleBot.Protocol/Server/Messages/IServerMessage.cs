using System.IO;

namespace BubbleBot.Server.Messages
{
    public interface IServerMessage
    {

        short MessageId { get; }

        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);

    }
}

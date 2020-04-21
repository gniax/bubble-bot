using System.IO;
using BubbleBot.Protocol.Enums;

namespace BubbleBot.Core.Accounts.Extensions.Flood
{
    public class FloodSentence
    {
        // Constructor
        public FloodSentence(string content, ChatActivableChannelsEnum channel, bool onPlayerJoined, bool onPlayerLeft)
        {
            Content = content;
            Channel = channel;

            if (Channel == ChatActivableChannelsEnum.PSEUDO_CHANNEL_PRIVATE)
            {
                OnPlayerJoined = onPlayerJoined;
                OnPlayerLeft = onPlayerLeft;
            }
            else
            {
                OnPlayerJoined = false;
                OnPlayerLeft = false;
            }
        }

        // Properties
        public string Content { get; }
        public ChatActivableChannelsEnum Channel { get; }
        public bool OnPlayerJoined { get; }
        public bool OnPlayerLeft { get; }


        public void Save(BinaryWriter bw)
        {
            bw.Write(Content);
            bw.Write((byte) Channel);
            bw.Write(OnPlayerJoined);
            bw.Write(OnPlayerLeft);
        }

        public static FloodSentence Load(BinaryReader br)
        {
            return new FloodSentence(br.ReadString(), (ChatActivableChannelsEnum) br.ReadByte(), br.ReadBoolean(),
                br.ReadBoolean());
        }
    }
}
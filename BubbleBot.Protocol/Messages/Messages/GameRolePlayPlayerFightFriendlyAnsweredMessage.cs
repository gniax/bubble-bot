namespace BubbleBot.Protocol.Messages
{
    public class GameRolePlayPlayerFightFriendlyAnsweredMessage : Message
    {

        // Properties
        public int FightId { get; set; }
        public uint SourceId { get; set; }
        public uint TargetId { get; set; }
        public bool Accept { get; set; }


        // Constructors
        public GameRolePlayPlayerFightFriendlyAnsweredMessage() { }

        public GameRolePlayPlayerFightFriendlyAnsweredMessage(int fightId = 0, uint sourceId = 0, uint targetId = 0, bool accept = false)
        {
            FightId = fightId;
            SourceId = sourceId;
            TargetId = targetId;
            Accept = accept;
        }

    }
}

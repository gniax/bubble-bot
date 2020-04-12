using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class AchievementDetailedListMessage : Message
    {

        // Properties
        public List<Achievement> StartedAchievements { get; set; }
        public List<Achievement> FinishedAchievements { get; set; }


        // Constructors
        public AchievementDetailedListMessage() { }

        public AchievementDetailedListMessage(List<Achievement> startedAchievements = null, List<Achievement> finishedAchievements = null)
        {
            StartedAchievements = startedAchievements;
            FinishedAchievements = finishedAchievements;
        }

    }
}

using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
    public class PrismAlignmentBonusResultMessage : Message
    {

        // Properties
        public AlignmentBonusInformations AlignmentBonus { get; set; }


        // Constructors
        public PrismAlignmentBonusResultMessage() { }

        public PrismAlignmentBonusResultMessage(AlignmentBonusInformations alignmentBonus = null)
        {
            AlignmentBonus = alignmentBonus;
        }

    }
}

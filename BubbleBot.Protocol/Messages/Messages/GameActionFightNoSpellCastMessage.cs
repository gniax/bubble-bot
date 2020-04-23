namespace BubbleBot.Protocol.Messages
{
    public class GameActionFightNoSpellCastMessage : Message
    {

        // Properties
        public uint SpellLevelId { get; set; }


        // Constructors
        public GameActionFightNoSpellCastMessage() { }

        public GameActionFightNoSpellCastMessage(uint spellLevelId = 0)
        {
            SpellLevelId = spellLevelId;
        }

    }
}

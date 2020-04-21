using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Fights.Fighters
{
    public class FightPlayerEntry : FighterEntry
    {
        // Constructor
        public FightPlayerEntry(GameFightFighterInformations infos) : base(infos)
        {
            if (infos is GameFightCharacterInformations a)
            {
                Name = a.Name;
                Level = (byte) a.Level;
            }
            else if (infos is GameFightMutantInformations b)
            {
                Name = b.Name;
                Level = 0; // Todo: Get the monster level
            }
        }

        // Properties
        public string Name { get; }

        public byte Level { get; }
    }
}
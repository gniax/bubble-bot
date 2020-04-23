using BubbleBot.Protocol.Messages;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Map.Entities
{
    public class PlayerEntry : MovableEntity
    {
        // Constructor
        public PlayerEntry(GameRolePlayCharacterInformations infos)
        {
            Id = infos.ContextualId;
            Name = infos.Name;
            CellId = (short) infos.Disposition.CellId;
            Level = (byte) (infos.AlignmentInfos.CharacterPower - Id);
        }

        public PlayerEntry(GameRolePlayMutantInformations mutantInfos)
        {
            Id = mutantInfos.ContextualId;
            Name = mutantInfos.Name;
            CellId = (short) mutantInfos.Disposition.CellId;
            Level = 0; // TODO get monster level maybe?
        }

        // Properties
        public int Id { get; }
        public string Name { get; }
        public byte Level { get; }


        #region Updates

        public void Update(TeleportOnSameMapMessage message)
        {
            CellId = (short) message.CellId;
        }

        #endregion
    }
}
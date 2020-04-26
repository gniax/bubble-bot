using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Types;
using BubbleBot.Data;

namespace BubbleBot.Core.Accounts.InGame.Map.Entities
{
    public class NpcEntry
    {
        // Constructor
        public NpcEntry(GameRolePlayNpcInformations infos)
        {
            Id = infos.ContextualId;
            NpcId = infos.NpcId;
            CellId = (short) infos.Disposition.CellId;
            Data = DataManager.Get<Protocol.Data.Npcs>((int) NpcId);
        }

        // Properties
        public int Id { get; }
        public uint NpcId { get; }
        public short CellId { get; }
        public Protocol.Data.Npcs Data { get; }

        public string Name => Data.NameId;
    }
}
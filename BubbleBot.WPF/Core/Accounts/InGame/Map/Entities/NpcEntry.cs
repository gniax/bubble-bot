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
            CellId = (short)infos.Disposition.CellId;
            GetDataInformations();
        }

        // Properties
        public int Id { get; }
        public uint NpcId { get; }
        public short CellId { get; }
        public Protocol.Data.Npcs Data { get; private set; }

        public string Name => Data.NameId;

        private async void GetDataInformations()
        {
            Data = await DataManager.Get<Protocol.Data.Npcs>((int)NpcId);
        }
    }
}
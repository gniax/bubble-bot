using System.Collections.Generic;
using System.Linq;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Core.Accounts.InGame.Map.Entities
{
    public class MonstersGroupEntry : MovableEntity
    {
        // Constructor
        public MonstersGroupEntry(GameRolePlayGroupMonsterInformations infos)
        {
            Id = infos.ContextualId;
            CellId = (short) infos.Disposition.CellId;
            Followers = new List<MonsterEntry>(infos.StaticInfos.Underlings.Count);

            Leader = new MonsterEntry(infos.StaticInfos.MainCreatureLightInfos);
            infos.StaticInfos.Underlings.ForEach(u => Followers.Add(new MonsterEntry(u)));
        }

        // Properties
        public int Id { get; }
        public MonsterEntry Leader { get; }
        public List<MonsterEntry> Followers { get; }

        public int MonstersCount => Followers.Count + 1;
        public int TotalLevel => Leader.Level + Followers.Sum(f => f.Level);


        public bool ContainsMonster(int gid)
        {
            if (Leader.GenericId == gid)
                return true;

            for (var i = 0; i < Followers.Count; i++)
                if (Followers[i].GenericId == gid)
                    return true;

            return false;
        }
    }
}
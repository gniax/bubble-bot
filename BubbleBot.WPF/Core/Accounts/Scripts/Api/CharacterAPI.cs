using System;
using System.Reflection;
using BubbleBot.Protocol.Enums;
using MoonSharp.Interpreter;

namespace BubbleBot.Core.Accounts.Scripts.Api
{
    [MoonSharpUserData]
    [Obfuscation(Exclude = false, Feature = "-rename", ApplyToMembers = true)]
    public class CharacterAPI : IDisposable
    {
        // Fields
        private Account _account;


        // Constructor
        public CharacterAPI(Account account)
        {
            _account = account;
        }


        public bool IsAlive()
        {
            return _account.Game.Character.LifeStatus == PlayerLifeStatusEnum.STATUS_ALIVE_AND_KICKING;
        }

        public bool IsTombstone()
        {
            return _account.Game.Character.LifeStatus == PlayerLifeStatusEnum.STATUS_TOMBSTONE;
        }

        public bool IsPhantom()
        {
            return _account.Game.Character.LifeStatus == PlayerLifeStatusEnum.STATUS_PHANTOM;
        }

        public uint PlayerId()
        {
            return _account.Game.Character.Id;
        }

        public string GroupId()
        {
            return _account.GroupId;
        }

        public string Name()
        {
            return _account.Game.Character.Name;
        }

        public string GroupMng()
        {
            return _account.AccountConfig.Nickname;
        }

        public string IdMng()
        {
            return _account.AccountConfig.Identifiant;
        }

        public byte Level()
        {
            return _account.Game.Character.Level;
        }

        public bool Sex()
        {
            return _account.Game.Character.Sex;
        }

        public uint LifePoints()
        {
            return _account.Game.Character.Stats.LifePoints;
        }

        public uint MaxLifePoints()
        {
            return _account.Game.Character.Stats.MaxLifePoints;
        }

        public int LifePointsP()
        {
            return _account.Game.Character.Stats.LifePercent;
        }

        public int Experience()
        {
            return _account.Game.Character.Stats.ExperiencePercent;
        }

        public uint EnergyPoints()
        {
            return _account.Game.Character.Stats.EnergyPoints;
        }

        public uint MaxEnergyPoints()
        {
            return _account.Game.Character.Stats.MaxEnergyPoints;
        }

        public int EnergyPointsP()
        {
            return _account.Game.Character.Stats.EnergyPercent;
        }

        public int Kamas()
        {
            return _account.Game.Character.Inventory.Kamas;
        }

        public void Sit()
        {
            _account.Game.Character.Sit();
        }

        public bool FreeSoul()
        {
            return _account.Game.Character.FreeSoul();
        }

        #region IDisposable Support

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _account = null;

                disposedValue = true;
            }
        }

        ~CharacterAPI()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
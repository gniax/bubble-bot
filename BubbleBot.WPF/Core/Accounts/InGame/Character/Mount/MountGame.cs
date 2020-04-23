using System;
using BubbleBot.Protocol.Messages;

namespace BubbleBot.Core.Accounts.InGame.Character.Mount
{
    public class MountGame : IDisposable, IClearable
    {
        // Fields
        private Account _account;


        // Constructor
        public MountGame(Account account)
        {
            _account = account;
        }


        // Properties
        public bool HasMount { get; private set; }
        public bool IsRiding { get; private set; }
        public uint CurrentRatio { get; private set; }

        public void Clear()
        {
            HasMount = false;
            IsRiding = false;
        }

        public void Dispose()
        {
            _account = null;
        }


        public void ToggleRiding()
        {
            if (!HasMount)
                return;

            _account.Network.SendMessage(new MountToggleRidingRequestMessage());
        }

        public void SetRatio(uint ratio)
        {
            if (!HasMount)
                return;

            _account.Network.SendMessage(new MountSetXpRatioRequestMessage(ratio));
        }

        #region Updates

        public void Update(MountSetMessage message)
        {
            HasMount = true;
        }

        public void Update(MountRidingMessage message)
        {
            IsRiding = message.IsRiding;
        }

        public void Update(MountXpRatioMessage message)
        {
            CurrentRatio = message.Ratio;
        }

        #endregion
    }
}
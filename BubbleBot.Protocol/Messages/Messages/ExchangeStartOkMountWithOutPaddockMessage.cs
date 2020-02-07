using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ExchangeStartOkMountWithOutPaddockMessage : Message
	{

		// Properties
		public List<MountClientData> StabledMountsDescription { get; set; }


		// Constructors
		public ExchangeStartOkMountWithOutPaddockMessage() { }

		public ExchangeStartOkMountWithOutPaddockMessage(List<MountClientData> stabledMountsDescription = null)
		{
			StabledMountsDescription = stabledMountsDescription;
		}

	}
}

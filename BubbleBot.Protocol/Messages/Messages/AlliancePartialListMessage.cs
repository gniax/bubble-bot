using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AlliancePartialListMessage : AllianceListMessage
	{

		// Constructors
		public AlliancePartialListMessage() { }

		public AlliancePartialListMessage(List<AllianceFactSheetInformations> alliances = null)
		{
			Alliances = alliances;
		}

	}
}

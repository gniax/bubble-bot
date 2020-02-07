using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AllianceListMessage : Message
	{

		// Properties
		public List<AllianceFactSheetInformations> Alliances { get; set; }


		// Constructors
		public AllianceListMessage() { }

		public AllianceListMessage(List<AllianceFactSheetInformations> alliances = null)
		{
			Alliances = alliances;
		}

	}
}

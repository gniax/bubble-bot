using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Types
{
	public class BasicAllianceInformations : AbstractSocialGroupInfos
	{

		// Properties
		public uint AllianceId { get; set; }
		public string AllianceTag { get; set; }


		// Constructors
		public BasicAllianceInformations() { }

		public BasicAllianceInformations(uint allianceId = 0, string allianceTag = "")
		{
			AllianceId = allianceId;
			AllianceTag = allianceTag;
		}

	}
}

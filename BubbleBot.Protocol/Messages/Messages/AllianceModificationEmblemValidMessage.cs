using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class AllianceModificationEmblemValidMessage : Message
	{

		// Properties
		public GuildEmblem Alliancemblem { get; set; }


		// Constructors
		public AllianceModificationEmblemValidMessage() { }

		public AllianceModificationEmblemValidMessage(GuildEmblem alliancemblem = null)
		{
			Alliancemblem = alliancemblem;
		}

	}
}

using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class TaxCollectorDialogQuestionBasicMessage : Message
	{

		// Properties
		public BasicGuildInformations GuildInfo { get; set; }


		// Constructors
		public TaxCollectorDialogQuestionBasicMessage() { }

		public TaxCollectorDialogQuestionBasicMessage(BasicGuildInformations guildInfo = null)
		{
			GuildInfo = guildInfo;
		}

	}
}

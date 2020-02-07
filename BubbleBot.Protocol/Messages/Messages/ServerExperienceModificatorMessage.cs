using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class ServerExperienceModificatorMessage : Message
	{

		// Properties
		public uint ExperiencePercent { get; set; }


		// Constructors
		public ServerExperienceModificatorMessage() { }

		public ServerExperienceModificatorMessage(uint experiencePercent = 0)
		{
			ExperiencePercent = experiencePercent;
		}

	}
}

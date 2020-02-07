using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class PartInfoMessage : Message
	{

		// Properties
		public ContentPart Part { get; set; }
		public double InstallationPercent { get; set; }


		// Constructors
		public PartInfoMessage() { }

		public PartInfoMessage(ContentPart part = null, double installationPercent = 0)
		{
			Part = part;
			InstallationPercent = installationPercent;
		}

	}
}

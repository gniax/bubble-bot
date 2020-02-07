using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class JobDescriptionMessage : Message
	{

		// Properties
		public List<JobDescription> JobsDescription { get; set; }


		// Constructors
		public JobDescriptionMessage() { }

		public JobDescriptionMessage(List<JobDescription> jobsDescription = null)
		{
			JobsDescription = jobsDescription;
		}

	}
}

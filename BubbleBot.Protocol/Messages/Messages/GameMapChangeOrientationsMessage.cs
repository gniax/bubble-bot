using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameMapChangeOrientationsMessage : Message
	{

		// Properties
		public List<ActorOrientation> Orientations { get; set; }


		// Constructors
		public GameMapChangeOrientationsMessage() { }

		public GameMapChangeOrientationsMessage(List<ActorOrientation> orientations = null)
		{
			Orientations = orientations;
		}

	}
}

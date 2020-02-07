using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class GameMapChangeOrientationMessage : Message
	{

		// Properties
		public ActorOrientation Orientation { get; set; }


		// Constructors
		public GameMapChangeOrientationMessage() { }

		public GameMapChangeOrientationMessage(ActorOrientation orientation = null)
		{
			Orientation = orientation;
		}

	}
}

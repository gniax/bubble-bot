using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class UpdateMountBoostMessage : Message
	{

		// Properties
		public List<UpdateMountBoost> BoostToUpdateList { get; set; }
		public double RideId { get; set; }


		// Constructors
		public UpdateMountBoostMessage() { }

		public UpdateMountBoostMessage(double rideId = 0, List<UpdateMountBoost> boostToUpdateList = null)
		{
			RideId = rideId;
			BoostToUpdateList = boostToUpdateList;
		}

	}
}

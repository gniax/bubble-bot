using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class LivingObjectSkinJntMood : IData
	{

		// Properties
		[JsonProperty("skinId")]
		public int Id { get; set; }
		[JsonProperty("moods")]
		public List<List<int>> Moods { get; set; }


		//Constructor
		internal LivingObjectSkinJntMood() {}

	}
}

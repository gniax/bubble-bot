using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class Incarnation : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("lookMale")]
		public string LookMale { get; set; }
		[JsonProperty("lookFemale")]
		public string LookFemale { get; set; }


		//Constructor
		internal Incarnation() {}

	}
}

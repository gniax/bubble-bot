using Newtonsoft.Json;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Data
{
	public class TypeActions : IData
	{

		// Properties
		[JsonProperty("id")]
		public int Id { get; set; }
		[JsonProperty("elementName")]
		public string ElementName { get; set; }
		[JsonProperty("elementId")]
		public int ElementId { get; set; }


		//Constructor
		internal TypeActions() {}

	}
}

using System.Collections.Generic;
using BubbleBot.Protocol.Types;

namespace BubbleBot.Protocol.Messages
{
	public class NicknameChoiceRequestMessage : Message
	{

		// Properties
		public string Nickname { get; set; }


		// Constructors
		public NicknameChoiceRequestMessage() { }

		public NicknameChoiceRequestMessage(string nickname = "")
		{
			Nickname = nickname;
		}

	}
}

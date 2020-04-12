using BubbleBot.Protocol.Types;
using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class HouseToSellListMessage : Message
    {

        // Properties
        public List<HouseInformationsForSell> HouseList { get; set; }
        public uint PageIndex { get; set; }
        public uint TotalPage { get; set; }


        // Constructors
        public HouseToSellListMessage() { }

        public HouseToSellListMessage(uint pageIndex = 0, uint totalPage = 0, List<HouseInformationsForSell> houseList = null)
        {
            PageIndex = pageIndex;
            TotalPage = totalPage;
            HouseList = houseList;
        }

    }
}

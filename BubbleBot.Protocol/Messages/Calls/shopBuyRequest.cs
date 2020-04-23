using System.Collections.Generic;

namespace BubbleBot.Protocol.Messages
{
    public class shopBuyRequest : Message
    {

        // Properties
        public string Currency { get; set; }
        public long amountHard { get; set; }
        public long amountSoft { get; set; }

        public List<Purchase> purchase { get; set; }
        public bool isMysteryBox { get; set; }


        // Constructor
        public shopBuyRequest(string currency, long amounthard, long amountsoft, int quantity, int id, bool ismysterybox)
        {
            Currency = currency;
            amountHard = amounthard;
            amountSoft = amountsoft;

            isMysteryBox = ismysterybox;
            Purchase purchases = new Purchase(quantity, id);
            purchase = new List<Purchase>();
            purchase.Add(purchases);
        }

    }
    public class Purchase
    {
        public int Quantity { get; set; }
        public int Id { get; set; }
        public Purchase(int quantity, int id)
        {
            Quantity = quantity;
            Id = id;
        }
    }
}


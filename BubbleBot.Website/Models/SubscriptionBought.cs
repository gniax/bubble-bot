using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("subscriptionsbought")]
    public class SubscriptionBought
    {

        // Properties
        [Key]
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public SubscriptionPlan Subscription { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int Weeks { get; set; }


        // Constructors
        public SubscriptionBought() { }

        public SubscriptionBought(int subscriptionId, int userId, int weeks)
        {
            SubscriptionId = subscriptionId;
            UserId = userId;
            Date = DateTime.Now;
            Weeks = weeks;
        }

    }
}

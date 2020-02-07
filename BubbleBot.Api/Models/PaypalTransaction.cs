using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("paypaltransactions")]
    public class PaypalTransaction
    {

        // Properties
        [Key]
        public string Id { get; set; }
        public string PayerEmail { get; set; }
        public string PayerId { get; set; }
        public DateTime CreateTime { get; set; }

        public int PointsPlanId { get; set; }
        public PointsPlan PointsPlan { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }


        // Constructor
        public PaypalTransaction() { }

        public PaypalTransaction(string id, string payerEmail, string payerId, int pointsPlanId, int userId)
        {
            Id = id;
            PayerEmail = payerEmail;
            PayerId = payerId;
            CreateTime = DateTime.Now;
            PointsPlanId = pointsPlanId;
            UserId = userId;
        }

    }
}

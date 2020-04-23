using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("users")]
    public class User
    {
        // Properties
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Avatar { get; set; }
        public int PlanId { get; set; }
        public double Points { get; set; }
        public short UserGroup { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime? TouchEndDate { get; set; }
        public string ValidationToken { get; set; }
        public List<PaypalTransaction> PaypalTransactions { get; set; }
        public List<ExtensionBought> ExtensionsBought { get; set; }

        public string AvatarUrl => string.IsNullOrEmpty(Avatar) ? BubbleBot.Api.Program.Constants.WebsiteIpAddress + $"/uploads/avatars/default.jpg" : BubbleBot.Api.Program.Constants.WebsiteIpAddress + $"/uploads/avatars/{Avatar}";
        public bool IsSubscribedToTouch => TouchEndDate != null && DateTime.Now < TouchEndDate.Value;


        // Constructors
        public User() { }

        public User(string username, string email, string password, string salt, string avatar, int planId, double points, short userGroup, DateTime regDate, DateTime touchEndDate, string validationToken)
        {
            Username = username;
            Email = email;
            Password = password;
            Salt = salt;
            Avatar = avatar;
            PlanId = planId;
            Points = points;
            UserGroup = userGroup;
            RegDate = regDate;
            TouchEndDate = touchEndDate;
            ValidationToken = validationToken;
        }


        public Dictionary<int, DateTime> GetCurrentExtensions()
        {
            var extensions = new Dictionary<int, DateTime>();
            if (!IsSubscribedToTouch)
            {
                return extensions;
            }

            var now = DateTime.Now;
            foreach (var eb in ExtensionsBought)
            {
                if (eb.EndDate < now)
                    continue;

                var remainingTime = eb.EndDate - now;
                if (!extensions.ContainsKey(eb.Extension.Id))
                {
                    extensions.Add(eb.Extension.Id, DateTime.Now.Add(remainingTime));
                }
                else
                {
                    extensions[eb.Extension.Id] = extensions[eb.Extension.Id].Add(remainingTime);
                }
            }
            return extensions;
        }

    }
}

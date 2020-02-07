using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BubbleBot.Website.Models
{
    public class PanelDb : DbContext
    {

        // Properties
        public DbSet<SubscriptionPlan> SubscriptionsPlans { get; set; }
        public DbSet<PointsPlan> PointsPlans { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<PaypalTransaction> PaypalTransactions { get; set; }
        public DbSet<Extension> Extensions { get; set; }
        public DbSet<SubscriptionBought> SubscriptionsBought { get; set; }
        public DbSet<ExtensionBought> ExtensionsBought { get; set; }


        // Constructors
        public PanelDb() { }

        public PanelDb(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaypalTransaction>()
                .HasOne(pt => pt.PointsPlan)
                .WithMany(p => p.PaypalTransactions)
                .HasForeignKey(pt => pt.PointsPlanId);

            modelBuilder.Entity<PaypalTransaction>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.PaypalTransactions)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<ExtensionBought>()
                .HasOne(eb => eb.User)
                .WithMany(u => u.ExtensionsBought)
                .HasForeignKey(eb => eb.UserId);

            modelBuilder.Entity<ExtensionBought>()
                .HasOne(eb => eb.Extension)
                .WithMany(e => e.ExtensionsBought)
                .HasForeignKey(eb => eb.ExtensionId);
        }

        public User GetUser(string username)
            => Users.Include(u => u.PaypalTransactions)
                    .Include(u => u.ExtensionsBought)
                    .ThenInclude(eb => eb.Extension)
                    .FirstOrDefault(u => u.Username == username);
        public int UserExists(string username, string email)
        {
            short error = 0;
            User user = Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
                error += 1;

            user = Users.FirstOrDefault(u => u.Email == email);
            if (user != null)
                error += 2;
            return error;
            // 0 : user does not exist
            // 1 : username exists
            // 2 : email exists
            // 3 : both exists
        } 

        public SubscriptionPlan GetSubscriptionPlan(int id)
            => SubscriptionsPlans.FirstOrDefault(p => p.Id == id);

        public PointsPlan GetPointsPlan(int id)
            => PointsPlans.Include(p => p.PaypalTransactions).FirstOrDefault(p => p.Id == id);

        public Extension GetExtension(int id)
            => Extensions.FirstOrDefault(e => e.Id == id);

    }
}

//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;

//namespace FixItNow.Models
//{
//    public class ApplicationDbContext: IdentityDbContext<MyApplicationUser>
//    {
//        public DbSet<Credentials> Credentialss { get; set; }
//        public DbSet<User> Userss { get; set; }
//        public DbSet<Provider> Providers { get; set; }
//        public DbSet<Service> Services { get; set; }
//        public DbSet<Review> Reviews {  get; set; }
//        public DbSet<Booking> Bookings { get; set; }
//        public DbSet<Messages> Messagess { get; set; }
//        public DbSet<SavedServices> SavedServicess { get; set; }


//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=FixItNow;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
//        }
//    }
//}

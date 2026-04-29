
using FixItNow.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Data
{
    public class ApplicationDbContext : IdentityDbContext<MyApplicationUser>
    {
        public DbSet<Credentials> Credentialss { get; set; }
        public DbSet<User> Userss { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Messages> Messagess { get; set; }
        public DbSet<SavedServices> SavedServicess { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}

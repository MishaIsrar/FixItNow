using FixItNow.Data;
using FixItNow.Data.Migrations;

namespace FixItNow.Models.Repository
{
    public class AdminRepository
    {
        private readonly ApplicationDbContext c;
        public AdminRepository(ApplicationDbContext d)
        {
            c = d;
        }
        public List<User> getListOfUsers()
        {
          
                 var list= c.Userss.ToList();
                return list;
            
        }
        public List<Booking> getListOfBooking()
        {
           
                var list=c.Bookings.ToList();
                return list;
            
        }
        public List<Provider> getListOfProviders()
        {
           
                var list = c.Providers.Where(p=>p.status== "UnApproved").ToList();
                return list;
            
        }
        public List<Review> getListOfReviews()
        {
           
                var list = c.Reviews.ToList();
                return list;
            
        }

    }
}

using FixItNow.Data;
using FixItNow.Data.Migrations;
using FixItNow.Models;

public class ProviderRepository
{
    private readonly ApplicationDbContext c;
    public ProviderRepository(ApplicationDbContext d)
    {
        c = d;
    }
    public int getTotalProjects(int id)
    {
      
            return c.Bookings.Count(b => b.providerId == id);
        
    }

    public int getTotalClients(int id)
    {
        
            return c.Bookings
                .Where(b => b.providerId == id)
                .Select(b => b.customerId)
                .Distinct()
                .Count();
        
    }

    public int getPositiveReviews(int id)
    {
        
            var allServicesId = c.Services.Where(s => s.providerID == id).Select(s => s.id).ToList();
            if (!allServicesId.Any()) return 0;

            return c.Reviews
                .Where(r => allServicesId.Contains(r.serviceId) && r.rating >= 3)
                .Count();
        
    }

    public int getSatisfactionRate(int id)
    {
      
            var allServicesId = c.Services.Where(s => s.providerID == id).Select(s => s.id).ToList();
            if (!allServicesId.Any()) return 0;

            var totalReviews = c.Reviews.Count(r => allServicesId.Contains(r.serviceId));
            if (totalReviews == 0) return 0;

            int positiveReviews = getPositiveReviews(id);
            return (positiveReviews * 100) / totalReviews;
        
    }
    public int getCount(int id)
    {
        int cnt = 0;
        
            var totalBookings = c.Bookings
                .Where(b => b.serviceId == id)
                .Count();
            cnt = totalBookings;
        
        return cnt;
    }
    public float getRevenue(int id)
    {
        float t = 0f;
       
            var totalRevenue = c.Bookings
         .Where(b => b.serviceId == id)
         .Sum(b => Convert.ToInt32(b.pricing));
            t = totalRevenue;
        
        return t;
    }
}

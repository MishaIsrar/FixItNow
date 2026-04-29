using FixItNow.Data;
using FixItNow.Data.Migrations;

namespace FixItNow.Models.Repository
{
    public class ServicesRepository
    {
        private readonly ApplicationDbContext c;
        public ServicesRepository(ApplicationDbContext d)
        {
            c = d;
        }
        public void RemoveSavedService(int id)
        {
          
                var y=c.SavedServicess.First(ss=>ss.id==id);
                c.Remove(y);
                c.SaveChanges();
            
        }
        public void RemoveService(int id)
        {
           
                var y = c.Services.First(ss => ss.id == id);
                c.Remove(y);
                c.SaveChanges();
            
        }
        public void EditService(int id,string t, string d, float p)
        {
           
                var y = c.Services.First(ss => ss.id == id);
                y.title = t;
                y.description = d;
                y.pricing = p;
                c.SaveChanges();
            
        }
        public List<int> getListOfExist(int cId, List<Booking> b)
        {
            List<int> existing= new List<int>();
           
                foreach(var i in b)
                {
                    var r = c.Reviews.FirstOrDefault(r => r.customerId == cId && r.serviceId == i.serviceId);
                    if(r != null)
                    {
                        existing.Add(1);
                    }
                    else
                    {
                        existing.Add(0);
                    }
                }
            
            return existing;
        }
    }
}

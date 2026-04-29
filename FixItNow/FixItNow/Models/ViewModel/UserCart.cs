namespace FixItNow.Models.ViewModel
{
    public class UserCart
    {
        public List<Booking> Bookings { get; set; }
        public List<SavedServices> SavedServices { get; set; }

        public List<int> exists { get; set; }
    }
}

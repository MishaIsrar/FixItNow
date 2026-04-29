namespace FixItNow.Models.ViewModel
{
    public class ProviderPanel
    {
        public List<Booking> bookings { get; set; }
        public List<Service> services { get; set; }
        public List<Messages> messages { get; set; }
        public List<string> titles { get; set; }
        public List<int> countOfBookings { get; set; }
        public List<int> revenueOfServices { get; set; }
    }
}

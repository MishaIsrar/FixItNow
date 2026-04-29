using System.ComponentModel.DataAnnotations;

namespace FixItNow.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int serviceId { get; set; }
        public int customerId { get; set; }
        public int providerId {  get; set; }
        public string description {  get; set; }
        public string address { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string pricing { get; set; }
    }
}

namespace FixItNow.Models
{
    public class Review
    {
        public int id { get; set; }
        public int rating {  get; set; }
        public string name { get; set; }
        public int serviceId { get; set; }
        public string comments {  get; set; }
        public int customerId {  get; set; }

        public DateTime dateTime    { get; set; }
    }
}

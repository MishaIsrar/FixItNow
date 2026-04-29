namespace FixItNow.Models
{
    public class User
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber {  get; set; }
        public string address { get; set; }
        public string city { get; set; }    
        public string gender { get; set; }
        public string password {  get; set; }
        public DateTime joinedTime { get; set; }
    }
}

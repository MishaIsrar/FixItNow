using System.ComponentModel.DataAnnotations;

namespace FixItNow.Models
{
    public class Messages
    {
        [Key]
        public int Id { get; set; }
        public string title {  get; set; }
        public int providerId {  get; set; }
        public int customerId {  get; set; }
        public string message { get; set; }
        public string serviceName {  get; set; }
        public string senderName { get; set; }
        public int serviceId {  get; set; }
        public string DateTime {  get; set; }

   
    }
}

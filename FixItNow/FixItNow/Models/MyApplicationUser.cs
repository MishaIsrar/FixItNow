using Microsoft.AspNetCore.Identity;

namespace FixItNow.Models
{
    public class MyApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
    }
}

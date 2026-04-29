using System.ComponentModel.DataAnnotations;

namespace FixItNow.Models
{
    public class Provider
    {
       
        public int Id { get; set; } 

        public string FirstName { get; set; }

        
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

      
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

      
        public string Address { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

      
        public string CNIC { get; set; }

       
        public string ResumePath { get; set; }

        public string ProfilePicPath { get; set; } 

        public string Education { get; set; }
        public string status { get; set; }
        public string skill { get; set; }
        public string expereince {  get; set; }
        public string about_me { get; set; }
    }
}

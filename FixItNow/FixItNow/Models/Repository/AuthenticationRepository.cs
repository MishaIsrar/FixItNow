using FixItNow.Data;
using Microsoft.AspNetCore.Components.Web;

namespace FixItNow.Models.Repository
{
    public class AuthenticationRepository
    {
        private readonly ApplicationDbContext c;
        public AuthenticationRepository(ApplicationDbContext d)
        {
            c = d;
        }

        public bool isAlreadyExists(string email)
        {
            bool stat = false;
            var cred = c.Credentialss.FirstOrDefault(s => s.email == email);
            if (cred != null)
            {
                stat = true;
            }
            return stat;
        }
    }
}

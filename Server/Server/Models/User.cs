using Microsoft.AspNetCore.Identity;

namespace Server.Models
{
    public class User : IdentityUser
    {
        public bool Subscribed { get; set; }

        public User()
        {
            Subscribed = true;
        }
    }
}
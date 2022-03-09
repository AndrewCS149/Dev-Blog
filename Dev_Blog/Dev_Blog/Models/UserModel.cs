using Microsoft.AspNetCore.Identity;

namespace Dev_Blog.Models
{
    public class UserModel : IdentityUser
    {
        public bool Subscribed { get; set; }
    }
}
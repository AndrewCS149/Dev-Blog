using Microsoft.AspNetCore.Identity;

namespace DevBlog.Models
{
    public class UserModel : IdentityUser
    {
        public bool Subscribed { get; set; }
    }
}
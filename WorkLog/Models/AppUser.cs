using Microsoft.AspNetCore.Identity;

namespace WorkLog.Models
{
    public class AppUser: IdentityUser
    {
        public string Name
        {
            get;
            set;
        }
    }
}

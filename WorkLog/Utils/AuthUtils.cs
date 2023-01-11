using Microsoft.AspNetCore.Identity;

namespace WorkLog.Utils
{
    public class AuthUtils
    {
        private IHttpContextAccessor _httpContextAccessor;
        private UserManager<IdentityUser> _userManager;
        public AuthUtils(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<IdentityUser?> getCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null)
            {
                return null;
            }
            return await _userManager.GetUserAsync(user);

        }
    }
}

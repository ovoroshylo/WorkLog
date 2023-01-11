using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WorkLog.Data;
using WorkLog.Models;

namespace WorkLog.Controllers
{
    /*
        Only Manager can manage their own channel. 
    */
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ChannelContext _channelContext;
        public ManagerController(UserManager<IdentityUser> userManager, ChannelContext channelContext, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _channelContext = channelContext;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View("Create");
        }

        public async Task<IActionResult> List()
        {
            var user = await _userManager.GetUserAsync(User);
            List<Channel> Channels = _channelContext.getChannelByManagerEmail(user?.Email);
            ViewData["Channels"] = Channels;
            return View("List");
        }
    }
}

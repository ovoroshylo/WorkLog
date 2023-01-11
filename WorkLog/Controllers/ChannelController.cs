using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLog.Data;
using WorkLog.Models;

namespace WorkLog.Controllers
{
    [Authorize]
    public class ChannelController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ChannelContext _channelContext;

        public ChannelController(UserManager<IdentityUser> userManager, ChannelContext channelContext)
        {
            _userManager = userManager;
            _channelContext = channelContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        /*
            Return AddChannel Page. 
        */
        public async Task<IActionResult> AddChannel()
        {
            var user = await _userManager.GetUserAsync(User);

            /*
                Join channel and channelUsers filtered by user.Email 
            */
            var channelUserDTOs = from channelUser in _channelContext.ChannelUsers
                                  where channelUser.UserEmail == user!.Email
                                  join channel in _channelContext.Channels on channelUser.ChannelId equals channel.Id
                                  select new { channel, channelUser.State, channelUser.Id };


            /*
                Set ViewData["ChannelUsers] as List of ChannelUSerDTO 
            */
            var results = channelUserDTOs.ToList();
            /*
                Default Worklog questions. 
            */
            List<string> questions = new List<string>{
                "How do you feel",
                "What are 5 things I accomplished yesterday?",
                "What are 5 things I didn't completed yesterday?",
                "What are 5 things I can do differently today?",
                "What are 5 things I want to accomplish today?",
                "What are 5 things that might get in my way today?",
                "What is my commitment today?"
            };
            /*
                Add WorkLog channel as default. 
            */
            List<ChannelUserDTO> channelUserResults = new List<ChannelUserDTO>()
            {
                new ChannelUserDTO()
                {
                    Name = "Default Work Log",
                    Questions = String.Join(",#", questions),
                    Id = -1,
                    State = 1,
                    InvitationId = -1,
                }
            };

            foreach (var result in results)
            {
                /*
                    Parse to ChannelUserDTO 
                */
                channelUserResults.Add(new ChannelUserDTO()
                {
                    Name = result.channel.Name,
                    Manager_email = result.channel.Manager_email,
                    Questions = result.channel.Questions,
                    Id = result.channel.Id,
                    State = result.State,
                    InvitationId = result.Id
                });
            }

            ViewData["ChannelUsers"] = channelUserResults;
            if (ViewData["ChannelUsers"] == null)
            {
                ViewData["ChannelUsers"] = new List<ChannelUserDTO>();
            }
            return View("AddChannel");
        }
    }
}

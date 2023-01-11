using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLog.Data;
using WorkLog.Models;

namespace WorkLog.Controllers
{
    /*
        Process Channel Related API 
    */
    [Authorize]
    [Route("api/[controller]")]
    public class ChannelAPIController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ChannelContext _channelContext;

        public ChannelAPIController(UserManager<IdentityUser> userManager, ChannelContext channelContext)
        {
            _userManager = userManager;
            _channelContext = channelContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        /*
            Add new Channel
        */
        [Authorize(Roles = "Manager")]
        [Route("addChannel")]
        [HttpPost]
        public async Task<bool> AddChannel(Channel channel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                channel.Manager_email = user?.Email;
                _channelContext.Add(channel);
                await _channelContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /*
            Invite user to channel. 
        */
        [Route("inviteUser")]
        [Authorize(Roles = "Manager")]
        [HttpPost]
        /* 
        response
        -4: userEmail does not exist.
        -3: Model is not valid
        -2: Can't invite yourself.
        -1: channelId does not exist
        0: already inviteds
        1: success
        */
        public async Task<int> InviteUser(ChannelUsers channelUsers)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                /*
                    You can't invite yourself. 
                */
                if (user?.Email == channelUsers.UserEmail)
                    return -2;

                /*
                    User Email does not exist.
                */
                user = await _userManager.FindByEmailAsync(channelUsers.UserEmail);
                if (user == null)
                    return -4;

                /*
                    ChannelId does not exist. 
                */
                Channel invitingChannel = await _channelContext.Channels.FindAsync(channelUsers.ChannelId);
                if (invitingChannel == null)
                    return -1;

                /*
                    check if already invited to that channel 
                */
                int count = await _channelContext.ChannelUsers.Where(x => x.UserEmail == channelUsers.UserEmail && x.ChannelId == channelUsers.ChannelId).CountAsync();
                if (count > 0)
                    return 0;

                _channelContext.ChannelUsers.Add(channelUsers);
                await _channelContext.SaveChangesAsync();
                return 1;
            }
            return -3;
        }

        /*
            Accept invitation as user 
        */
        [Route("acceptInvite")]
        [HttpPost]
        public async Task<bool> AcceptInvite(long invitationId)
        {
            ChannelUsers channelUser = await _channelContext.ChannelUsers.FindAsync(invitationId);
            
            if (channelUser == null) return false;
            
            channelUser.State = 1;
            _channelContext.ChannelUsers.Update(channelUser);
            await _channelContext.SaveChangesAsync();
            return true;
        }

        /*
            Join channel(just set session attribute ChannelId) 
        */
        [Route("joinChannel")]
        [HttpPost]
        public void JoinChannel(long channelId)
        {
            /*
                set ChannelId session attribute
            */
            HttpContext.Session.SetInt32("ChannelId", (int)channelId);
        }

        /*
            Get all users of specific channel 
        */
        [Authorize(Roles = "Manager")]
        [Route("getUsers")]
        [HttpPost]
        public List<ChannelUsers> GetChannelUsers(long channelId)
        {
            return _channelContext.ChannelUsers.Where(e => e.ChannelId == channelId).ToList<ChannelUsers>();
        }
    }
}

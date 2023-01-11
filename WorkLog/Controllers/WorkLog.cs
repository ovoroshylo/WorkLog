using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using WorkLog.Data;
using WorkLog.Models;

namespace WorkLog.Controllers
{
    public class AnswerViewModel
    {
        public List<string>? Questions { get; set; }
    }

    [Authorize]
    public class WorkLog : Controller
    {
        private readonly WorkLogAnswerContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ChannelContext _channelContext;
        public WorkLog(WorkLogAnswerContext context, UserManager<IdentityUser> userManager, ChannelContext channelContext)
        {
            _context = context;
            _userManager = userManager;
            _channelContext = channelContext;
        }

        private List<string> defaultQuestions = new List<string>{
            "How do you feel",
            "What are 5 things I accomplished yesterday?",
            "What are 5 things I didn't completed yesterday?",
            "What are 5 things I can do differently today?",
            "What are 5 things I want to accomplish today?",
            "What are 5 things that might get in my way today?",
            "What is my commitment today?"
        };

        public IActionResult Index()
        {
            return View();
        }

        /*
            Return Answer Page   
        */
        public async Task<IActionResult> Answer()
        {
            var user = await _userManager.GetUserAsync(User);
            /*
                Get ChannelId from session. 
            */
            int channelId = (int)HttpContext.Session.GetInt32("ChannelId");

            /*
                Check if you've already answered the question
            */
            var inventories = _context.WorkLog.
                FromSqlRaw($"SELECT * FROM WorkLog WHERE CONVERT(Date, Date) = CONVERT(Date, GETDATE()) AND Email = '{user.Email}' AND ChannelId = {channelId}")
                .ToList();
            if (inventories.Count > 0)
                return await DailyInventory();

            List<string> questions;

            if (channelId == -1)
            {
                questions = defaultQuestions;
            }
            /*
                Get Questions from ChannelId 
            */
            else
            {
                Channel channel = await _channelContext.Channels.FindAsync((long)channelId);
                questions = channel.Questions.Split(",#").ToList();
                questions.Insert(0, "How do you feel?");
            }

            return View("Answer", new AnswerViewModel { Questions = questions });
        }

        public IActionResult Search()
        {
            return View("Search");
        }

        /*
            Return DailyInventory Page 
        */
        public async Task<IActionResult> DailyInventory()
        {
            var user = await _userManager.GetUserAsync(User);
            int channelId = (int)HttpContext.Session.GetInt32("ChannelId");

            List<WorkLogAnswerDTO> AnswerDTOS = _context.getAnswerDTOSOfDate(DateTime.Now, user.Email, channelId);

            //concat AllAnswers for wordCloud
            string AllAnswers = "";
            foreach (var AnswerDTO in AnswerDTOS)
            {
                foreach (var Answer in AnswerDTO.Answers)
                {
                    AllAnswers += " " + Answer;
                }
            }

            ViewData["AnswerDTOS"] = AnswerDTOS;

            List<string> questions;
            /*
                In case of Default WorkLog channel 
            */
            if (channelId == -1)
                questions = defaultQuestions;

            else
            {
                Channel channel = await _channelContext.Channels.FindAsync((long)channelId);
                questions = channel.Questions.Split(",#").ToList();
                questions.Insert(0, "How do you feel?");
            }

            ViewData["questions"] = questions;
            ViewData["AllAnswers"] = AllAnswers;

            return View("DailyInventory");
        }

        public async Task<IActionResult> LinkedList()
        {
            var user = await _userManager.GetUserAsync(User);
            int channelId = (int)HttpContext.Session.GetInt32("ChannelId");
            List<WorkLogAnswerDTO> AnswerDTOS = _context.getAnswerDTOSOfDate(DateTime.Now, user.Email, channelId);

            //concat AllAnswers for wordCloud
            string AllAnswers = "";
            foreach (var AnswerDTO in AnswerDTOS)
            {
                foreach (var Answer in AnswerDTO.Answers)
                {
                    AllAnswers += " " + Answer;
                }
            }

            ViewData["AnswerDTOS"] = AnswerDTOS;
            
            List<string> questions;
            /*
                In case of Default WorkLog channel 
            */
            if (channelId == -1)
                questions = defaultQuestions;

            else
            {
                Channel channel = await _channelContext.Channels.FindAsync((long)channelId);
                questions = channel.Questions.Split(",#").ToList();
                questions.Insert(0, "How do you feel?");
            }

            ViewData["questions"] = questions;
            ViewData["AllAnswers"] = AllAnswers;

            return View("LinkedList");
        }

        public IActionResult Compare()
        {
            return View("Compare");
        }

        public IActionResult Review()
        {
            return View("Review");
        }
    }
}

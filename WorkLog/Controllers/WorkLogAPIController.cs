using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Channels;
using WorkLog.Data;
using WorkLog.Models;
using WorkLog.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WorkLog.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class WorkLogAPIController : Controller
    {
        private readonly WorkLogAnswerContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public WorkLogAPIController(WorkLogAnswerContext context, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {
            return View("WorkLog/Index");
        }

        /*
            Add answers to the database. 
            Request Parameter: list of answers.
        */
        [Route("addAnswers")]
        [HttpPost]
        public async Task<IActionResult> AddAnswers(List<WorkLogAnswer> answers)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager?.GetUserAsync(User);
                answers.ForEach(workLog =>
                {
                    /*
                        Set Email as loggined user's Email and channelId as session's channelId 
                    */
                    workLog.Email = user?.Email;
                    workLog.ChannelId = (int)HttpContext.Session.GetInt32("ChannelId");
                    _context.Add(workLog);
                });
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NoContent();
        }

        /*
            concat all answers between startDate to endDate.  
        */
        [Route("getByDateRange")]
        [HttpPost]
        public async Task<string> GetByDateRange(string startDateStr, string endDateStr)
        {
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var user = await _userManager?.GetUserAsync(User);
            string concat_answers = _context.ConcatAllAnswersBetweenDateRange(startDate, endDate, user.Email, (int)HttpContext.Session.GetInt32("ChannelId"));

            return concat_answers;
        }

        /*
            flag = 0: "Past Week"
            flag = 1: "Past Month"
            flag = 2: "Past 3 Months"
            flag = 3: "Past Year"
            flag = 4: "All Time"

            Build Word cloud by flag.
        */
        [Route("searchWorkLogs")]
        [HttpPost]
        public async Task<List<WorkLogSearchDTO>> SearchWorkLogs(int flag)
        {
            var user = await _userManager?.GetUserAsync(User);
            DateTime CurrentDate = DateTime.Now;
            int i;
            List<WorkLogSearchDTO> results = new List<WorkLogSearchDTO>();
            int channelId = (int)HttpContext.Session.GetInt32("ChannelId");

            switch (flag)
            {
                //past week
                case 0:
                    for (i = 0; i < 7; i++)
                    {
                        DateTime pastDate = CurrentDate.AddDays(-6 + i);
                        string concat_answers = _context.ConcatAllAnswersBetweenDateRange(pastDate, pastDate, user.Email, channelId);
                        List<int> Feeling = _context.GetFeelingCounts(pastDate, pastDate, user.Email);
                        results.Add(new WorkLogSearchDTO()
                        {
                            Date = pastDate.ToString("dddd MM/dd/yy"),
                            Answer_Concat = concat_answers,
                            Feeling = Feeling
                        });
                    }
                    return results;
                //past month
                case 1:
                    for (i = 0; i < 4; i++)
                    {
                        DateTime pastBeginDate = CurrentDate.AddDays(-7 * (4 - i));
                        DateTime pastEndDate = pastBeginDate.AddDays(7);
                        string concat_answers = _context.ConcatAllAnswersBetweenDateRange(pastBeginDate, pastEndDate, user.Email, channelId);
                        List<int> Feeling = _context.GetFeelingCounts(pastBeginDate, pastEndDate, user.Email);
                        results.Add(new WorkLogSearchDTO()
                        {
                            Date = pastBeginDate.ToString("MM/dd/yy") + " - " + pastEndDate.ToString("MM/dd/yy"),
                            Answer_Concat = concat_answers,
                            Feeling = Feeling
                        });
                    }
                    return results;
                // past 3 month
                case 2:
                    for (i = 0; i < 3; i++)
                    {
                        DateTime pastBeginDate = CurrentDate.AddMonths(-(3 - i));
                        DateTime pastEndDate = pastBeginDate.AddMonths(1);
                        string concat_answers = _context.ConcatAllAnswersBetweenDateRange(pastBeginDate, pastEndDate, user.Email, channelId);
                        List<int> Feeling = _context.GetFeelingCounts(pastBeginDate, pastEndDate, user.Email);
                        results.Add(new WorkLogSearchDTO()
                        {
                            Date = pastBeginDate.ToString("MMMM dd") + " - " + pastEndDate.ToString("MMMM dd"),
                            Answer_Concat = concat_answers,
                            Feeling = Feeling
                        });
                    }
                    return results;
                //past year
                case 3:
                    for (i = 0; i < 12; i++)
                    {
                        DateTime pastBeginDate = CurrentDate.AddMonths(-(12 - i));
                        DateTime pastEndDate = pastBeginDate.AddMonths(1);
                        string concat_answers = _context.ConcatAllAnswersBetweenDateRange(pastBeginDate, pastEndDate, user.Email, channelId);
                        List<int> Feeling = _context.GetFeelingCounts(pastBeginDate, pastEndDate, user.Email);
                        results.Add(new WorkLogSearchDTO()
                        {
                            Date = pastEndDate.ToString("MMMM yyyy"),
                            Answer_Concat = concat_answers,
                            Feeling = Feeling
                        });
                    }
                    return results;
                //all time
                case 4:
                    results.Add(new WorkLogSearchDTO()
                    {
                        Date = "All Time",
                        Answer_Concat = _context.ConcatAllAnswersBetweenDateRange(DateTime.MinValue, DateTime.MaxValue, user.Email, channelId),
                        Feeling = _context.GetFeelingCounts(DateTime.MinValue, DateTime.MaxValue, user.Email)
                    });
                    return results;
                default:
                    return null;
            }
            //return NoContent();
        }

        [Route("getDateList")]
        [HttpPost]
        public async Task<List<string>> getDateList(int start, int count)
        {
            var user = await _userManager?.GetUserAsync(User);

            int channelId = (int)HttpContext.Session.GetInt32("ChannelId");
            //convert DateTime to string
            List<string> results = new List<string>();
            List<DateTime> dateLists = _context.GetDateList(start, 1, user.Email, channelId);
            foreach (DateTime date in dateLists)
            {
                results.Add(date.ToString("MM/dd/yyyy"));
            }
            return results;
        }
    }
}

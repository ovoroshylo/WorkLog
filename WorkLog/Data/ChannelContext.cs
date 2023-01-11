using Microsoft.EntityFrameworkCore;
using WorkLog.Models;

namespace WorkLog.Data
{
    public class ChannelContext: DbContext
    {
        public ChannelContext(DbContextOptions<ChannelContext> options) : base(options)
        {
        }
        /*
            Channel DB
        */
        public DbSet<Channel> Channels { get; set; }
        /*
            ChannelUser DB 
        */
        public DbSet<ChannelUsers> ChannelUsers { get; set; }

        /*
            Get Channel List given managerEmail 
        */
        public List<Channel> getChannelByManagerEmail(string managerEmail)
        {
            try
            {
                return Channels.Where(s => s.Manager_email == managerEmail).ToList();
            }
            catch (Exception ex)
            {
                return new List<Channel>();
            }
        }
    }
}

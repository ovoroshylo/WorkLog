using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WorkLog.Data
{
    //User Db Context
    public class AppDBContext: IdentityDbContext
    {
        private readonly DbContextOptions<AppDBContext> _options;
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            _options = options;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}
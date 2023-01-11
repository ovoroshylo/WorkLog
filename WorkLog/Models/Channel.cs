using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WorkLog.Models
{
    public class Channel
    {
        public long Id { get; set;  }

        [Required]
        [EmailAddress]
        public string? Manager_email { get; set; } = "a@b.com";

        [Required]
        public string? Name { get; set; } = "";

        public string Questions { get; set; } = "";

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

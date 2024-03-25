using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Comments")]
    public class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        // int? is nullable int
        // Key
        public int? StockId { get; set; }
        // Link to the stock
        // Navigation property
        public Stock? Stock { get; set; }

        // One-To-One
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
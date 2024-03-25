using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Comment
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;

        // int? is nullable int
        // Key
        public int? StockId { get; set; }

        // Don't need Navigation property for Dto
        // because that going to inject a whole entire object
        // within comment Dto, because it will make comment Dto look bad
        // once it return, it will also return stock

        // Link to the stock
        // Navigation property
        //public Stock? Stock { get; set; }
    }
}
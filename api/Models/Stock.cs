using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Stocks")]
    public class Stock
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        // make sure it is a decimal(18, 2) for money (18 digits, 2 after decimal)
        // Purchase
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Purchase { get; set; }
        // make sure it is a decimal(18, 2) for money (18 digits, 2 after decimal)
        // Last Div
        [Column(TypeName = "decimal(18, 2)")]
        public decimal LastDiv { get; set; }
        public string Industry { get; set; } = string.Empty;
        public long MarketCap { get; set; }

        // One to Many Relationship
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
    }
}
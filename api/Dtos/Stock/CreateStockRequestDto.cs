using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock
{
    public class CreateStockRequestDto
    {
        // Data validation
        [Required]
        [MaxLength(10, ErrorMessage = "Symbol cannot be over 10 characters long")]    
        public string Symbol { get; set; } = string.Empty;
        // Data validation
        [Required]
        [MaxLength(10, ErrorMessage = "Company Name cannot be over 10 characters long")]    
        public string CompanyName { get; set; } = string.Empty;
        // Data validation
        [Required]
        [Range(1, 1000000000)]    
        public decimal Purchase { get; set; }
        // Data validation
        [Required]
        [Range(0.001, 100)]    
        public decimal LastDiv { get; set; }
        // Data validation
        [Required]
        [MaxLength(10, ErrorMessage = "Industry cannot be over 10 characters long")]    
        public string Industry { get; set; } = string.Empty;
        // Data validation
        [Required]
        [Range(1, 5000000000)]    
        public long MarketCap { get; set; }
    }
}
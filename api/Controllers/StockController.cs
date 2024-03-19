using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
        // Controller base
        [Route("api/stock")]
        [ApiController]
    public class StockController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public StockController(ApplicationDBContext context)
        {
            _context = context;
        }

        // READ method
        [HttpGet]
        public IActionResult GetAll()
        {
            var stocks = _context.Stocks.ToList()
            .Select(s => s.ToStockDto());

            return Ok(stocks);
        }

        // READ method
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            // search by id
            var stock = _context.Stocks.Find(id);

            // if stock is null
            if (stock == null)
            {
                // return not found
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }
    }
}
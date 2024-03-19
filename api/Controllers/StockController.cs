using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.Stock;
using Microsoft.EntityFrameworkCore;

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

        // GET(READ) method
        [HttpGet]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> GetAll()
        {
            // await keyword is used to wait for the task to finish
            //.ToListAsync() is used to convert the IQueryable to a list
            var stocks = await _context.Stocks.ToListAsync();
            // convert to DTO
            var stockDto = stocks.Select(s => s.ToStockDto());

            return Ok(stocks);
        }

        // GET(READ) method
        [HttpGet("{id}")]
        /// async Task<> means that this method is asynchronous
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // search by id
            // await keyword is used to wait for the task to finish
            // FindAsync() is used to search by id
            var stock = await _context.Stocks.FindAsync(id);

            // if stock is null
            if (stock == null)
            {
                // return not found
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        // POST(CREATE) method
        [HttpPost]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            var stockModel = stockDto.ToStockFromCreateDTO();
            // await keyword is used to wait for the task to finish
            // AddAsync() is used to add to the database
            await _context.Stocks.AddAsync(stockModel);
            // save changes
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        // PUT(UPDATE) method
        [HttpPut]
        [Route("{id}")]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            // await keyword is used to wait for the task to finish
            // FirstOrDefaultAsync() is used to search by id
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            if (stockModel == null)
            {
                return NotFound();
            }

            stockModel.Symbol = updateDto.Symbol;
            stockModel.CompanyName = updateDto.CompanyName;
            stockModel.Purchase = updateDto.Purchase;
            stockModel.LastDiv = updateDto.LastDiv;
            stockModel.Industry = updateDto.Industry;
            stockModel.MarketCap = updateDto.MarketCap;

            // save changes
            await _context.SaveChangesAsync();

            return Ok(stockModel.ToStockDto());
        }

        // DELETE method
        [HttpDelete]
        [Route("{id}")]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // await keyword is used to wait for the task to finish
            // FindAsync() is used to search by id
            var stockModel = await _context.Stocks.FindAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            // remove does not have async
            _context.Stocks.Remove(stockModel);
            // save changes
            await _context.SaveChangesAsync();

            // return NoContent() because we are deleting a single resource
            return NoContent();
        }
    }
}
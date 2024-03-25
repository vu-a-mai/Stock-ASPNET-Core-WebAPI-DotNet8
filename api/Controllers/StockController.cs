using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using api.Dtos.Stock;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;
using api.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers
{
        // Controller base
        [Route("api/stock")]
        [ApiController]
    public class StockController: ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;
        public StockController(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
            _context = context;
        }

        // GET(READ) method
        [HttpGet]
        [Authorize]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // await keyword is used to wait for the task to finish
            // _stockRepo.GetAllAsync() is used to get all the stocks
            var stocks = await _stockRepo.GetAllAsync(query);
            // convert to DTO
            var stockDto = stocks.Select(s => s.ToStockDto()).ToList();

            return Ok(stockDto);
        }

        // GET(READ) method
        [HttpGet("{id:int}")]
        /// async Task<> means that this method is asynchronous
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // search by id
            // await keyword is used to wait for the task to finish
            // FindAsync() is used to search by id
            var stock = await _stockRepo.GetByIdAsync(id);

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
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stockModel = stockDto.ToStockFromCreateDTO();
            // await keyword is used to wait for the task to finish
            await _stockRepo.CreateAsync(stockModel);

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        // PUT(UPDATE) method
        [HttpPut]
        [Route("{id:int}")]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // await keyword is used to wait for the task to finish
            // FirstOrDefaultAsync() is used to search by id
            var stockModel = await _stockRepo.UpdateAsync(id, updateDto);

            if (stockModel == null)
            {
                return NotFound();
            }

            return Ok(stockModel.ToStockDto());
        }

        // DELETE method
        [HttpDelete]
        [Route("{id:int}")]
        // async Task<> means that this method is asynchronous
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // await keyword is used to wait for the task to finish
            // FindAsync() is used to search by id
            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            // return NoContent() because we are deleting a single resource
            return NoContent();
        }
    }
}
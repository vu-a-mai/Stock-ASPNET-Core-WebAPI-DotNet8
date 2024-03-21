using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        // GET All Method
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // get all comments
            var comments = await _commentRepo.GetAllAsync();
            // convert to DTO
            var commentDto = comments.Select(c => c.ToCommentDto());

            // return the comments
            return Ok(commentDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // get the comment by id
            var comment = await _commentRepo.GetByIdAsync(id);

            // check if the comment exists
            // if it doesn't exist, return a 404 not found
            // if it does exist, continue on
            if (comment == null)
            {
                return NotFound();
            }
            
            // return the comment
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto)
        {
            // check if the stock exists
            // if it doesn't exist, return a 400 bad request
            // if it does exist, continue on
            if(!await _stockRepo.StockExists(stockId))
            {
                return BadRequest("Stock does not exist");
            }

            // used the mapper to map the comment DTO to the comment model
            // this will create a new comment model with the values from the DTO and set the stockId to the stockId passed in
            var commentModel = commentDto.ToCommentFromCreate(stockId);
            
            // then we call the CreateAsync method on the comment repository to create the comment and return the comment as a DTO
            await _commentRepo.CreateAsync(commentModel);

            // CreatedAtAction is used to return a 201 status code and the location of the resource in the Location header
            // new { id = commentModel } is used to pass in the id of the comment to the route
            // this will be used in the GetById method to get the comment by id
            // the commentModel is passed in as the second parameter because it is the first parameter of the method that is being called
            // this is because the method being called is CreateAsync and the first parameter is the comment model and the second parameter is the stockId
            // this is why we are passing in the commentModel as the second parameter
            return CreatedAtAction(nameof(GetById), new { id = commentModel }, commentModel.ToCommentDto());
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Extensions;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFMPService _fmpService;

        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo, UserManager<AppUser> userManager, IFMPService fmpService)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
            _fmpService = fmpService;
        }

        // GET All Method
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] CommentQueryObject queryObject)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get all comments
            var comments = await _commentRepo.GetAllAsync(queryObject);
            // convert to DTO
            var commentDto = comments.Select(c => c.ToCommentDto());

            // return the comments
            return Ok(commentDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        // [HttpPost("{stockId:int}")]
        // public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto)
        // {
        //     // Perform Dtos validation
        //     if(!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     // check if the stock exists
        //     // if it doesn't exist, return a 400 bad request
        //     // if it does exist, continue on
        //     if(!await _stockRepo.StockExists(stockId))
        //     {
        //         return BadRequest("Stock does not exist");
        //     }

        //     // get the username from the user
        //     var username = User.GetUserName();
        //     var appUser = await _userManager.FindByNameAsync(username);

        //     // used the mapper to map the comment DTO to the comment model
        //     // this will create a new comment model with the values from the DTO and set the stockId to the stockId passed in
        //     var commentModel = commentDto.ToCommentFromCreate(stockId);

        //     // set the AppUserId to the id of the user that is logged in
        //     commentModel.AppUserId = appUser.Id;
            
        //     // then we call the CreateAsync method on the comment repository to create the comment and return the comment as a DTO
        //     await _commentRepo.CreateAsync(commentModel);

        //     // CreatedAtAction is used to return a 201 status code and the location of the resource in the Location header
        //     // new { id = commentModel } is used to pass in the id of the comment to the route
        //     // this will be used in the GetById method to get the comment by id
        //     // the commentModel is passed in as the second parameter because it is the first parameter of the method that is being called
        //     // this is because the method being called is CreateAsync and the first parameter is the comment model and the second parameter is the stockId
        //     // this is why we are passing in the commentModel as the second parameter
        //     return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        // }

        [HttpPost("{symbol:alpha}")]
        public async Task<IActionResult> Create([FromRoute] string symbol, CreateCommentDto commentDto)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _stockRepo.GetStockBySymbolAsync(symbol);

            if(stock == null)
            {
                stock = await _fmpService.FindStockBySymbolAsync(symbol);
                if(stock == null)
                {
                    return BadRequest("Stock does not exist");
                }
                else
                {
                    await _stockRepo.CreateAsync(stock);
                }
            }

            // get the username from the user
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);

            // used the mapper to map the comment DTO to the comment model
            // this will create a new comment model with the values from the DTO and set the stockId to the stockId passed in
            var commentModel = commentDto.ToCommentFromCreate(stock.Id);

            // set the AppUserId to the id of the user that is logged in
            commentModel.AppUserId = appUser.Id;
            
            // then we call the CreateAsync method on the comment repository to create the comment and return the comment as a DTO
            await _commentRepo.CreateAsync(commentModel);

            // CreatedAtAction is used to return a 201 status code and the location of the resource in the Location header
            // new { id = commentModel } is used to pass in the id of the comment to the route
            // this will be used in the GetById method to get the comment by id
            // the commentModel is passed in as the second parameter because it is the first parameter of the method that is being called
            // this is because the method being called is CreateAsync and the first parameter is the comment model and the second parameter is the stockId
            // this is why we are passing in the commentModel as the second parameter
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }
        
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateDto)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // get the comment by id
            var comment = await _commentRepo.UpdateAsync(id, updateDto.ToCommentFromUpdate());

            // check if the comment exists
            // if it doesn't exist, return a 404 not found
            // if it does exist, continue on
            if (comment == null)
            {
                return NotFound("Comment not found");
            }

            // return the comment
            return Ok(comment.ToCommentDto());

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // Perform Dtos validation
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var commentModel = await _commentRepo.DeleteAsync(id);

            if(commentModel == null)
            {
                return NotFound("Comment does not exist");
            }

            return Ok(commentModel);
        }
    }
}
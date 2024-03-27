using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IFMPService _fmpService;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepo, IPortfolioRepository portfolioRepo, IFMPService fmpService)
        {
            _userManager = userManager;
            _stockRepo = stockRepo;
            _portfolioRepo = portfolioRepo;
            _fmpService = fmpService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPorfolio(appUser);

            return Ok(userPortfolio);
        }

        // [HttpPost]
        // [Authorize]
        // public async Task<IActionResult> AddStockToPortfolio(string symbol)
        // {
        //     var username = User.GetUserName();
        //     var appUser = await _userManager.FindByNameAsync(username);
        //     var stock = await _stockRepo.GetStockBySymbolAsync(symbol);

        //     if(stock == null)
        //     {
        //         return BadRequest("Stock not found");
        //     }

        //     var userPortfolio = await _portfolioRepo.GetUserPorfolio(appUser);
        //     if(userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower()))
        //     {
        //         return BadRequest("Stock already in portfolio. Cannot add same stock to portfolio.");
        //     }

        //     var portfolioModel = new Portfolio
        //     {
        //         AppUserId = appUser.Id,
        //         StockId = stock.Id
        //     };

        //     await _portfolioRepo.CreateAsync(portfolioModel);

        //     if(portfolioModel == null)
        //     {
        //         return StatusCode(500, "Could not add stock to portfolio");
        //     }
        //     else
        //     {
        //         return Created();
        //     }
        // }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddStockToPortfolio(string symbol)
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
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

            if(stock == null)
            {
                return BadRequest("Stock not found");
            }

            var userPortfolio = await _portfolioRepo.GetUserPorfolio(appUser);
            if(userPortfolio.Any(s => s.Symbol.ToLower() == symbol.ToLower()))
            {
                return BadRequest("Stock already in portfolio. Cannot add same stock to portfolio.");
            }

            var portfolioModel = new Portfolio
            {
                AppUserId = appUser.Id,
                StockId = stock.Id
            };

            await _portfolioRepo.CreateAsync(portfolioModel);

            if(portfolioModel == null)
            {
                return StatusCode(500, "Could not add stock to portfolio");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteStockFromPortfolio(string symbol)
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);

            var userPortfolio = await _portfolioRepo.GetUserPorfolio(appUser);
            var filteredPortfolio = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower());

            if(filteredPortfolio.Count() == 1)
            {
                await _portfolioRepo.DeletePortfolio(appUser, symbol);
            }
            else
            {
                return BadRequest("Stock not found in portfolio");
            }

            return Ok();
        }
    }
}
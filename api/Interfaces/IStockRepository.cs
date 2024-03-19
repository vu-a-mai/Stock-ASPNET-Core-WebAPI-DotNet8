using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    // interface for stock repository
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync();
    }
}

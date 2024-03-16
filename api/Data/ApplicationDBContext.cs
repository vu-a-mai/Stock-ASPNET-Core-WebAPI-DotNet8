using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// add api.Models to the using list
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    // This class is used by the application to access and seed data for a new instance of the application
    public class ApplicationDBContext : DbContext
    {
        // type "ctor" to quickly create a constuctor
        // base option is to pass in the options for the dbcontext
        public ApplicationDBContext(DbContextOptions dbContextOptions) 
        : base(dbContextOptions)
        {
            
        }
        
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CzadRuletAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace CzadRuletAPI.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}

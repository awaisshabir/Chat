using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options):base(options)
        {

        }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Order> Posts { get; set; }
    }
}

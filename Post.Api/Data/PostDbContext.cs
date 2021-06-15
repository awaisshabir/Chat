using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Post.Api.Data
{
    public class PostDbContext : DbContext
    {
        public PostDbContext(DbContextOptions<PostDbContext> options):base(options)
        {

        }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Post> Posts { get; set; }
    }
}

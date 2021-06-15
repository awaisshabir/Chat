using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Api.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Post.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDbContext Context;

        public PostController(PostDbContext context)
        {
            this.Context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<Data.Entities.Post>> GetPostAsync()
        {
            return await Context.Posts.Include(x=>x.User).ToListAsync();

        }
        [HttpGet("{id}")]
        public async Task<Data.Entities.Post> GetPostAsync(int id)
        {
            return await Context.Posts.Include(x=>x.User).SingleOrDefaultAsync(x => x.PostId == id);
        }
        [HttpPost]
        public async Task<ActionResult<Data.Entities.User>> PostAsunc(Data.Entities.Post post)
        {
            await Context.Posts.AddAsync(post);
            await Context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPostAsync),new {id = post.PostId },post);
        }
    }
}

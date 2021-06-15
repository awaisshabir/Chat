using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Order.Api.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly OrderDbContext Context;

        public UserController(OrderDbContext context)
        {
            this.Context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<Data.Entities.User>> GetUserAsync()
        {
            return await Context.Users.ToListAsync();

        }
    }
}

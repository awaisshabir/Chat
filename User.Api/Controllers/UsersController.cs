using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Api.Data;

namespace User.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext dbContext;

        public UsersController(UserDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IEnumerable<User.Api.Data.Entities.User>> GetUsersAsync()
        {
            return await dbContext.Users.ToListAsync();
           
        }
        [HttpGet("{id}")]
        public async Task<User.Api.Data.Entities.User> GetUserAsync(int id)
        {
            return await dbContext.Users.SingleOrDefaultAsync(x => x.Id == id);
        }
         [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAsync(int id, User.Api.Data.Entities.User user)
        {
            dbContext.Entry(user).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            var integrationEventData = JsonConvert.SerializeObject(new { 
                id = user.Id,
                name = user.Name
            });
            Publish("user.update",integrationEventData);
            return NoContent();

        }
        [HttpPost]
        public async Task<IActionResult> PostUserAsync(User.Api.Data.Entities.User user)
        {
           await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
            var integrationEventData = JsonConvert.SerializeObject(new
            {
                id = user.Id,
                name = user.Name
            });
            Publish("user.add", integrationEventData);
            return CreatedAtAction(nameof(GetUserAsync),new {id = user.Id },user);

        }
        private static void Publish(string routingKey, string data)
        {
            try
            {
                // Method 1
                var factory = new ConnectionFactory() {
                    HostName = "10.201.205.51",
                    UserName = "admin",
                    Password = "Cloud@123",
                    Port = AmqpTcpEndpoint.UseDefaultPort
                };
                // Method 2
                //var factory = new ConnectionFactory()
                //{
                //    Uri = new Uri("amqp://guest:guest@localhost:5672")
                //};
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                var body = Encoding.UTF8.GetBytes(data);
                channel.BasicPublish(exchange: "cyclus.user", routingKey: routingKey, basicProperties: null, body: body);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

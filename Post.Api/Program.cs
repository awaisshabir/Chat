using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Post.Api.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Listener();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    private static void Listener()
        {

            //var factory = new ConnectionFactory()
            //{
            //    Uri = new Uri("amqp://admin:Cloud@123@10.201.205.51:5672")
            //};
            var factory = new ConnectionFactory()
            {
                HostName = "10.201.205.51",
                UserName = "admin",
                Password = "Cloud@123",
                Port = AmqpTcpEndpoint.UseDefaultPort
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            channel.BasicConsume(queue: "cyclus.user",
                autoAck: true,
                consumer: consumer);
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var contextOptions = new DbContextOptionsBuilder<Post.Api.Data.PostDbContext>()
                .UseSqlite(@"Data Source=post.db").Options;
            var dbContext = new PostDbContext(contextOptions);
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var data = JObject.Parse(message);
            var type = e.RoutingKey;
            if (type == "user.add")
            {
                dbContext.Users.Add(new Data.Entities.User()
                {
                     UserId = data["id"].Value<int>(),
                      UserName = data["name"].Value<string>()
                });
                dbContext.SaveChanges();
            }
            if (type == "user.update")
            {
                var user = dbContext.Users.FirstOrDefault(a => a.UserId == data["id"].Value<int>());
                user.UserName = data["name"].Value<string>();
                dbContext.SaveChanges();
            }
            if (type == "user.delete")
            {
                var user = dbContext.Users.FirstOrDefault(a => a.UserId == data["id"].Value<int>());
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
            }
        }
    }
}

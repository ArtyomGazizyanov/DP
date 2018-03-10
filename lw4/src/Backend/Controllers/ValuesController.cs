using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Backend.Dto;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDatabase _redisDb;
        private readonly IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
             _configuration = configuration;             
             ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
             _redisDb = redis.GetDatabase();
        }        

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = _redisDb.StringGet(id);
            
            return  value;
        }

         // GET api/values/<id>
        [HttpGet("rank/{id?}")]
        public IActionResult GetRank(string id)
        {
            string value = null;
            int coutOfTries = 10;
            while(coutOfTries != 0)
            {
                value = _redisDb.StringGet(id);
                if(value != null)
                { 
                    break;                    
                }
                else
                {
                    System.Threading.Thread.Sleep(200);
                    coutOfTries -= 1;
                }
            }

            if(value != null)
            {
                return Ok(value);
            }
            else
            {
                 return NotFound();
            }
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody] DataTransferDto dataTransfer)
        {
            if(dataTransfer == null)
            {
                return null;
            }

            var id = Guid.NewGuid().ToString();

            _redisDb.StringSet(id, dataTransfer.Data);
            Publish(id);

            return id;
        }

        private void Publish(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "backend-api",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                        routingKey: "backend-api",
                                        basicProperties: null,
                                        body: body);

                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
       }
    }    
}

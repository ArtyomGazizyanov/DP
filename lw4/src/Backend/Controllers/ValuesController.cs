using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Backend.Dto;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ;
using Redis;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RedisHelper _redisHelper;

        public ValuesController(IConfiguration configuration)
        {
             _configuration = configuration;             
             _redisHelper = new RedisHelper();
        }        

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string value = _redisHelper.Database.StringGet(id);
            
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
                value = _redisHelper.Database.StringGet(id);
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

            _redisHelper.Database.StringSet(id, dataTransfer.Data);         
            var rabbitMq = new RabbitMq();
            rabbitMq.ExchangeDeclare("backend-api", ExchangeType.Fanout);     
			rabbitMq.PublishToExchange("backend-api", id);  

            return id;
        }        
    }    
}

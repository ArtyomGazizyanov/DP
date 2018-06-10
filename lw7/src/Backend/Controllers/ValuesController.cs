using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Backend.Dto;
using Microsoft.Extensions.Caching.Distributed;
using RabbitMQ;
using Redis;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RedisHelper _redisHelper;
        private readonly string _rankPrefix = "rank_";
        
        public ValuesController(IConfiguration configuration)
        {
             _configuration = configuration;             
             _redisHelper = RedisHelper.Instance;
        }        

        // GET api/values/<id>
        [HttpGet("{id}")]
        public string Get(string id)
        {
            var databaseId = _redisHelper.CalculateDatabase(id);
            _redisHelper.SetDatabase(databaseId);            
            string value = _redisHelper.Database.StringGet(id);
            
            return  value;
        }

         // GET api/values/<id>
        [HttpGet("rank/{id?}")]
        public IActionResult GetRank(string id)
        {
            string value = null;
            int coutOfTries = 10;
            var databaseId = _redisHelper.CalculateDatabase(id);
            _redisHelper.SetDatabase(databaseId);            
            while(coutOfTries != 0)
            {
                
                value = _redisHelper.Database.StringGet($"{_rankPrefix}{id}");
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

         // GET api/values/<id>
        [HttpGet("statistics")]
        public IActionResult GetStatistics()
        {
            RedisHelper.Instance.SetDatabase(-1);
            var textNum = Convert.ToString(RedisHelper.Instance.Database.StringGet("textNum"));
            var highRankPart = Convert.ToString(RedisHelper.Instance.Database.StringGet("highRankPart"));
            var avgRank = Convert.ToString(RedisHelper.Instance.Database.StringGet("avgRank"));

            Console.WriteLine($"textNUm: {textNum}, highRankPart: {highRankPart}, avgRank: {avgRank}");
            StatisticsDto statisticsDto = new StatisticsDto{
                TextNum = textNum,
                HighRankPart = highRankPart,
                AvgRank = avgRank
            };
            var serializedObject = JsonConvert.SerializeObject(statisticsDto);
            Console.WriteLine($"{serializedObject}");

            return Ok(serializedObject);
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
            var databaseId = _redisHelper.CalculateDatabase(id);            
            Console.WriteLine($" [x] For {id} database : {databaseId}");    
            _redisHelper.SetDatabase(databaseId);
            _redisHelper.Database.StringSet(id, dataTransfer.Data);

            var rabbitMq = new RabbitMq();
            rabbitMq.ExchangeDeclare("backend-api", ExchangeType.Fanout);     
			rabbitMq.PublishToExchange("backend-api", id);  

            return id;
        }        
    }    
}

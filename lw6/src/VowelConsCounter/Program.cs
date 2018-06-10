using Redis;
using RabbitMQ;
using System;
using System.Text;
using VowelConsCounter;

namespace VowelConsCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("VowelConsCounter has started...");
            var rabbitMq = new RabbitMq();
            
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare("text-rank-tasks", ExchangeType.Direct);
            rabbitMq.ExchangeDeclare("vowel-cons-counter", ExchangeType.Direct);
            rabbitMq.BindQueueToExchange("text-rank-tasks");
            rabbitMq.ConsumeQueue(message =>
            {
                string redisValue = String.Empty;
                var databaseId = RedisHelper.Instance.CalculateDatabase(message);
                RedisHelper.Instance.SetDatabase(databaseId);       
                redisValue = RedisHelper.Instance.Database.StringGet(message);
                var vowelConsNum = TextRankCalc.Calc(redisValue);
                
                rabbitMq.PublishToExchange("vowel-cons-counter", $"{message}|{vowelConsNum.Consonants}|{vowelConsNum.Vowels}");

                Console.WriteLine(" [x] Received from redis {0} with key: {1} equals {2}", redisValue, message, $"{vowelConsNum.Consonants}|{vowelConsNum.Vowels}");    
            });

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                                                      
        }
    }
}

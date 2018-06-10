using Redis;
using RabbitMQ;
using System;
using System.Text;
using VowelConsRater;

namespace VowelConsRater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("VowelConsRater has started...");            
            var rabbitMq = new RabbitMq();
            string rankPrefix = "rank_";
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare("vowel-cons-counter", ExchangeType.Direct);
            rabbitMq.ExchangeDeclare("text-rank-calc", ExchangeType.Fanout);
            rabbitMq.BindQueueToExchange("vowel-cons-counter");
            rabbitMq.ConsumeQueue(message =>
            {
                var info = message.Split('|');
                var contextId = info[0];
                var volwes = info[1];
                var cons = info[2];
                
                var rank = TextRankCalc.Calc(Decimal.Parse(volwes), Decimal.Parse(cons));
                var databaseId = RedisHelper.Instance.CalculateDatabase(contextId);
                RedisHelper.Instance.SetDatabase(databaseId);       
                RedisHelper.Instance.Database.StringSet($"{rankPrefix}{contextId}", $"{rank.ToString()}");
                rabbitMq.PublishToExchange("text-rank-calc", $"{contextId}|{rank}");
                Console.WriteLine(" [x] Received from redis {0} with key: {1} equals {2}", message, rank);    
            });

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                                                      
        }
    }
}

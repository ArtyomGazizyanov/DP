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
            RedisHelper redis = new RedisHelper();
            var rabbitMq = new RabbitMq();
            string rankPrefix = "rank_";
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare("vowel-cons-counter", ExchangeType.Direct);
            rabbitMq.BindQueueToExchange("vowel-cons-counter");
            rabbitMq.ConsumeQueue(message =>
            {
                var info = message.Split('|');
                var contextId = info[0];
                var volwes = info[1];
                var cons = info[2];
                
                var rank = TextRankCalc.Calc(Decimal.Parse(volwes), Decimal.Parse(cons));
                         
                redis.Database.StringSet($"{rankPrefix}{contextId}", $"{rank.ToString()}");

                Console.WriteLine(" [x] Received from redis {0} with key: {1} equals {2}", contextId, rank);    
            });

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                                                      
        }
    }
}

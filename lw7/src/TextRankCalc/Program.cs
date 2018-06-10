using Redis;
using RabbitMQ;
using System;
using System.Text;

namespace TextRankCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TextRankCalck has started...");
            RedisHelper redis = RedisHelper.Instance;
            var rabbitMq = new RabbitMq();
            
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare("backend-api", ExchangeType.Fanout);
            rabbitMq.ExchangeDeclare("text-rank-tasks", ExchangeType.Direct);
            rabbitMq.BindQueueToExchange("backend-api");
            rabbitMq.ConsumeQueue(message =>
            {
                rabbitMq.PublishToExchange("text-rank-tasks", message);                
            });                    

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                                                      
        }
    }
}

using Redis;
using RabbitMQ;
using System;
using System.Text;

namespace TextListener
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Text Listener...");

            RedisHelper redishelper = RedisHelper.Instance;
            
            var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare("backend-api", ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange("backend-api");
			rabbitMq.ConsumeQueue(message =>
			{
                var databaseId = RedisHelper.Instance.CalculateDatabase(message);
                RedisHelper.Instance.SetDatabase(databaseId);            
                string redisValue = String.Empty;

                redisValue = redishelper.Database.StringGet(message);                                        
                Console.WriteLine(" [x] Received from redis {0} with key: {1}", redisValue, message);    
            });

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();           
        }
    }
}

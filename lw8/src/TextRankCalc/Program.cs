using Redis;
using RabbitMQ;
using System;
using System.Text;

namespace TextRankCalc
{   
    public static class Status
    {        
        public const string Rejected = "Rejected";
        public const string Accepted = "Accepted";
        public const string Completed = "Completed";
        public const string Processing = "Processing";
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TextRankCalck has started...");
            RedisHelper redis = RedisHelper.Instance;
            var rabbitMq = new RabbitMq();
            
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare("processing-limiter", ExchangeType.Fanout);
            rabbitMq.ExchangeDeclare("text-rank-tasks", ExchangeType.Direct);
            rabbitMq.BindQueueToExchange("processing-limiter");
            rabbitMq.ConsumeQueue(message =>
            {					
                Console.WriteLine($"Acepted message {message}");

                string[] splittedMessage = message.Split('|');
				string textId = splittedMessage[0];
				bool doesTextAllowed = splittedMessage[1] == "True";
                if (!doesTextAllowed)
				{
					Console.WriteLine($"{message} not allowed");
					return;
				}
                RedisHelper.Instance.SetDatabase(RedisHelper.Instance.CalculateDatabase(textId));
                if(RedisHelper.Instance.Database.StringGet($"status_{textId}") != Status.Accepted)
                {
                    Console.WriteLine($"'status_{textId}: {Status.Processing}' to redis database({RedisHelper.Instance.Database.Database})");
                    RedisHelper.Instance.Database.StringSet($"status_{textId}", Status.Processing);
                }

                rabbitMq.PublishToExchange("text-rank-tasks", textId);                
            });                    

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                                                      
        }
    }
}

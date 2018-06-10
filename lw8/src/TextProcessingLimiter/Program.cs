using System;
using Redis;
using RabbitMQ;

namespace TextProcessingLimiter
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
        private static int _succeededTextCount = 0;
        private static int _limit = 3;
        static void Main(string[] args)
        {
		    string statusOfText = "status_";
            RedisHelper redishelper = RedisHelper.Instance;

            var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare("backend-api", ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange("backend-api");
			rabbitMq.ConsumeQueue(message =>
			{               
                Console.WriteLine($"New message from text-rank-calc: \"{message}\"");
				redishelper.SetDatabase(redishelper.CalculateDatabase(message));

				bool status = _succeededTextCount < _limit;

                if (status)
				{
					++_succeededTextCount;
					Console.WriteLine($"'{statusOfText}{message}: {Status.Accepted}', redishelper.Database: {redishelper.Database.Database}");
					redishelper.Database.StringSet($"{statusOfText}{message}", Status.Accepted);
				}
				else
				{
					Console.WriteLine($"'{statusOfText}{message}: {Status.Rejected}'redishelper.Database: {redishelper.Database.Database}");
					redishelper.Database.StringSet($"{statusOfText}{message}", Status.Rejected);
				}
                var stringToPublish = $"{message}|" +
					(status ? "True" : "False");
				Console.WriteLine($"{stringToPublish} to 'processing-limiter' exchange");
				rabbitMq.PublishToExchange("processing-limiter", stringToPublish);
            });
            
            rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare("text-success-marker", ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange("text-success-marker");
			rabbitMq.ConsumeQueue(message =>
			{
				Console.WriteLine($"New message from 'text-success-marker': \"{message}\"");

				string[] data = message.Split('|');
				bool isTextSucceeded = data[1] == "True";
				if (!isTextSucceeded)
				{
					Console.WriteLine("Succeeded text count reset");
					_succeededTextCount -= 1;
				}

				Console.WriteLine("----------");
			});

			Console.WriteLine("TextProcessingLimiter has started");
			Console.WriteLine("Press [enter] to exit.");
			Console.ReadLine();

        }
    }
}

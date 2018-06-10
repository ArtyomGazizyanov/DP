using Redis;
using RabbitMQ;
using System;
using System.Text;

namespace TextSuccessMarker
{
    class Program
    {
        private const string _listeningExchangeName = "text-rank-calc";
        private const string _publishExchangeName = "text-success-marker";

        private const double _successLowerBound = 0.5;

        static void Main(string[] args)
        {
            string rankString = "rank_";
            RedisHelper redishelper = RedisHelper.Instance;
            var rabbitMq = new RabbitMq();
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare(_listeningExchangeName, ExchangeType.Fanout);
            rabbitMq.BindQueueToExchange(_listeningExchangeName);
            rabbitMq.ConsumeQueue(message =>
            {
                Console.WriteLine($"New message from {_listeningExchangeName}: \"{message}\"");
                var info = message.Split('|');                
                var textId = info[0];
                var rank = Double.Parse(info[1]);
                redishelper.SetDatabase(redishelper.CalculateDatabase(textId));
                
                Console.WriteLine($"'{rankString}{textId}: {rank}'");

                var stringToPublish = $"{textId}|" +
                    (rank > _successLowerBound
                        ? "True"
                        : "False");

                Console.WriteLine($"{stringToPublish} to {_publishExchangeName} exchange");
                rabbitMq.PublishToExchange(_publishExchangeName, stringToPublish);

                Console.WriteLine("----------");
            });

            Console.WriteLine($"TextSuccessMarker has started. Success lower bound is {_successLowerBound}");
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}

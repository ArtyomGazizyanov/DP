using Redis;
using RabbitMQ;
using System;
using System.Text;
using VowelConsRater;

namespace TextStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TextStatistics has started...");            
            var rabbitMq = new RabbitMq();
            
            rabbitMq.QueueDeclare();
            rabbitMq.ExchangeDeclare("text-success-marker", ExchangeType.Fanout);
            rabbitMq.BindQueueToExchange("text-success-marker");
            rabbitMq.ConsumeQueue(message =>
            {
                Console.WriteLine($" [x] message: {message}");    
                var info = message.Split('|');
                var contextId = info[0];
                var isSuccess = info[1];
                Console.WriteLine($" [x] contextId: {contextId}, isSuccess: {isSuccess}");    

                RedisHelper.Instance.Increment("textNum");

                if(isSuccess == "True")
                {                 
                    RedisHelper.Instance.Increment("highRankPart");
                }
                var rank = Convert.ToDecimal(RedisHelper.Instance.Database.StringGet($"rank_{contextId}"));
                
                RedisHelper.Instance.Increment("ranksSum", rank);
                var textNum = Convert.ToDecimal(RedisHelper.Instance.Database.StringGet("textNum"));
                var ranksSum = Convert.ToDecimal(RedisHelper.Instance.Database.StringGet("ranksSum"));
                RedisHelper.Instance.Database.StringSet("avgRank", $"{ranksSum/textNum}");
                
                Console.WriteLine($" [x] TextNum: {textNum}, ranksSum: {ranksSum}, rankSum: {ranksSum/textNum}");
            });

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                                                      
        }
    }
}

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
            rabbitMq.ExchangeDeclare("text-rank-calc", ExchangeType.Fanout);
            rabbitMq.BindQueueToExchange("text-rank-calc");
            rabbitMq.ConsumeQueue(message =>
            {
                var info = message.Split('|');
                var contextId = info[0];
                var rank = Convert.ToDecimal(info[1]);
                RedisHelper.Instance.Increment("textNum");

                if(rank > (decimal)0.5)
                {
                    RedisHelper.Instance.Increment("highRankPart");
                }
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

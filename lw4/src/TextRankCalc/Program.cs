using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using StackExchange.Redis;

namespace TextRankCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var channel = RabbitMQHelper.GetModel())
            {
                RabbitMQHelper.DeclareQueue("backend-api", channel);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    string redisValue = String.Empty;
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
                    IDatabase redisDb = redis.GetDatabase();
                    redisValue = redisDb.StringGet(message);
                    string rank = TextRankCalc.Calc(redisValue).ToString();
                    
                    redisDb.StringSet(message, rank);
                                            
                    Console.WriteLine(" [x] Received from redis {0} with key: {1} equals {2}", redisValue, message, rank);    
                };                    
                RabbitMQHelper.ConsumeQueue("backend-api", consumer, channel);

                Console.WriteLine(" Press [enter] to exit.");                
                Console.ReadKey();                              
            }            
        }
        /*static void Main(string[] args)
        {
            using(var channel = RabbitMQHelper.GetModel())
            {
                RabbitMQHelper.DeclareQueue("backend-api", channel);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    string redisValue = String.Empty;
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
                    IDatabase redisDb = redis.GetDatabase();
                   // redisValue = redisDb.StringGet(message);
                    int rank =0;// TextRankCalc.Calc(redisValue);
                    
                    //redisDb.StringSet(message, rank);
                                            
                    Console.WriteLine(" [x] Calculated rank of {0} with key: {1} equals {2}", redisValue, message, rank);    
                };                    
                RabbitMQHelper.ConsumeQueue("backend-api", consumer, channel);                                             
            }   
            Console.WriteLine(" Press [enter] to exit.");                
            //Console.ReadKey(); 
        }*/
    }
}

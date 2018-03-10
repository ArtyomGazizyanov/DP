using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using StackExchange.Redis;

namespace TextListener
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
                                            
                    Console.WriteLine(" [x] Received from redis {0} with key: {1}", redisValue, message);    
                };                    
                RabbitMQHelper.ConsumeQueue("backend-api", consumer, channel);

                Console.WriteLine(" Press [enter] to exit.");                
                Console.ReadKey();                              
            }            
        }
    }
}

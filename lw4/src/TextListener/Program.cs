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
            RedisHelper redishelper = new RedisHelper();
            
            var rabbitMq = new RabbitMq();
			rabbitMq.QueueDeclare();
			rabbitMq.ExchangeDeclare("backend-api", ExchangeType.Fanout);
			rabbitMq.BindQueueToExchange("backend-api");
			rabbitMq.ConsumeQueue(message =>
			{
                string redisValue = String.Empty;
                redisValue = redishelper.Database.StringGet(message);                                        
                Console.WriteLine(" [x] Received from redis {0} with key: {1}", redisValue, message);    
            });

            Console.WriteLine(" Press [enter] to exit.");                
            Console.ReadKey();                              
/*            using(var channel = RabbitMQ.RabbitMq.GetModel())
            {
                RabbitMQHelper.DeclareExchangeQueue("backend-api", channel);
                string queueName = channel.QueueDeclare().QueueName;

                RabbitMQHelper.BindQueueToExchange("backend-api", channel, queueName);
                var consumer = RabbitMQHelper.GetConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    
                };

                RabbitMQHelper.ConsumeQueue("backend-api", consumer, channel);

            }            */
        }
    }
}

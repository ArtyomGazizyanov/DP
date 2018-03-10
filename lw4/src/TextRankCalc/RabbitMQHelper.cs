using StackExchange.Redis;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TextRankCalc
{
    class RabbitMQHelper
    {
        static private ConnectionFactory ConnectionFactory {get; set;} = GetConnectionFactory();        
        private static string _defaultConnectionString = "localhost";
        private static string _connectionString {get; set;} = _defaultConnectionString;
        private IModel _channel {get; set;}
        
        public RabbitMQHelper(string connectionString)
        {
            _connectionString = connectionString ?? _defaultConnectionString;
            _channel = GetModel();
        }

        public static ConnectionFactory GetConnectionFactory()
        {
            return new ConnectionFactory() { HostName = "localhost" };
        }   

        public static IConnection GetConnection()
        {
            return ConnectionFactory.CreateConnection();
        }

        public static IModel GetModel()
        {
            return GetConnection().CreateModel();
        }

        public static void DeclareQueue(string queueName, IModel channel)
        {
            channel.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
        }

        public static void ConsumeQueue(string queueName, EventingBasicConsumer data, IModel channel)
        {
            channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: data);
        }

    }
}
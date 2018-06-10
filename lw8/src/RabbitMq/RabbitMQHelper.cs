using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ
{
/*    public class RabbitMQHelper
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

        public static EventingBasicConsumer GetConsumer(IModel channel)
        {
            return  new EventingBasicConsumer(channel);
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

        public static void DeclareExchangeQueue(string queueName, IModel channel)
        {            
            channel.ExchangeDeclare(exchange: queueName, type: "fanout");
        }
        

        public static void Push(string queueName, IModel channel, byte[] body)
        {            
            channel.BasicPublish(exchange: queueName,
                                    routingKey: "",
                                    basicProperties: null,
                                    body: body);
        }

        public static void ConsumeQueue(string queueName, EventingBasicConsumer data, IModel channel)
        {
            channel.BasicConsume(queue: queueName,
                                    autoAck: true,
                                    consumer: data);
        }

        public static void BindQueueToExchange(string exchangeName,  IModel channel, string queueName)
		{
			channel.QueueBind(
				queue: queueName,
				exchange: exchangeName,
				routingKey: "");
		}
    }*/
    public static class ExchangeType
	{
		public const string Direct = RabbitMQ.Client.ExchangeType.Direct;
		public const string Fanout = RabbitMQ.Client.ExchangeType.Fanout;
	}

	public class RabbitMq
	{
		private static readonly ConnectionFactory ConnectionFactory =
			new ConnectionFactory { HostName = "localhost" };

		private readonly IModel _channel = ConnectionFactory.CreateConnection().CreateModel();

		public string QueueName { get; private set; }

		public void QueueDeclare(string queueName = "")
		{
			QueueName = _channel.QueueDeclare(
				queue: queueName,
				exclusive: false,
				autoDelete: true,
				arguments: null).QueueName;
		}

		public void ConsumeQueue(Action<string> onReceive)
		{
            if (onReceive == null)
            {
                throw new System.ArgumentNullException(nameof(onReceive));
            }

            var consumer = new EventingBasicConsumer(_channel);

			consumer.Received += (model, ea) =>
			{
				byte[] body = ea.Body;
				string message = Encoding.UTF8.GetString(body);
				onReceive(message);
			};

			_channel.BasicConsume(
				queue: QueueName,
				autoAck: true,
				consumer: consumer
			);
		}

		public void ExchangeDeclare(string exchangeName, string exchangeType)
		{
			_channel.ExchangeDeclare(
				exchange: exchangeName,
				type: exchangeType,
				autoDelete: true);
		}

		public void PublishToExchange(string exchangeName, string message)
		{
			_channel.BasicPublish(
				exchange: exchangeName,
				routingKey: "",
				basicProperties: null,
				body: Encoding.UTF8.GetBytes(message));
		}

		public void BindQueueToExchange(string exchangeName)
		{
			_channel.QueueBind(
				queue: QueueName,
				exchange: exchangeName,
				routingKey: "");
		}
	}
}
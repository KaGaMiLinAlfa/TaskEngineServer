using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public static class MQHelper
{
    private readonly string host;
    private readonly string username;
    private readonly string password;
    private readonly string vhost;

    private static IConnection _connection;
    private static object _lockobj = new object();

    public static void ConnectionInit()
    {
        if (_connection != null)
            return;

        lock (_lockobj)
        {
            if (_connection != null)
                return;


        }

    }

    public static void Publish(string exchange, string routingKey, string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = host,
            UserName = username,
            Password = password
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false);

            // 声明并创建队列
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queueName, exchange, routingKey, null);

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange, routingKey, null, body);
        }
    }

    public static void Consume(string queue, string exchange, string routingKey, Action<string> messageHandler)
    {
        var factory = new ConnectionFactory()
        {
            HostName = host,
            UserName = username,
            Password = password
        };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false);
            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(queue, exchange, routingKey, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messageHandler(message);
                channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue, autoAck: false, consumer: consumer);

            Console.WriteLine("Waiting for messages. Press any key to exit.");
            Console.ReadKey();
        }
    }
}

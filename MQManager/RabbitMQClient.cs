using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using System.Data;
using System.Collections.Generic;
using System.Threading.Channels;

public static class RabbitMQClient
{
    //test
    private static string host = "10.0.3.20";
    private static int port = 5672;
    private static string username = "guest";
    private static string password = "guest";
    private static string vhost = "center";
    private static string _machineName;

    //dev
    //private static string host;
    //private static string username;
    //private static string password;
    //private static string vhost;

    private static IConnection _connection;
    public static IConnection _Connection
    {
        get
        {
            if (_connection == null)
                ConnectionInit();

            return _connection;
        }
    }

    private static object _lockobj = new object();

    //已处理的QueueName
    private static HashSet<string> _queueNames = new HashSet<string>();

    public static void ConnectionInit()
    {
        if (_connection != null)
            return;

        lock (_lockobj)
        {
            if (_connection != null)
                return;

            //GetMachineName
            _machineName = $"{Environment.MachineName}_{Environment.OSVersion}_{Environment.UserDomainName}_{Guid.MD5(6)}";


            //CreateConnection
            var factory = new ConnectionFactory()
            {
                HostName = host,
                Port = port,
                UserName = username,
                Password = password,

                VirtualHost = vhost,
                AutomaticRecoveryEnabled = true,
                ConsumerDispatchConcurrency = 1
            };

            _connection = factory.CreateConnection();
        }

    }








    private static IModel channel;
    private static IBasicProperties props;
    private static object channellock = new object();

    public static void Publish(string queueName, string message) => Publish("kagamitest", queueName, message);
    public static void Publish(string exchange, string queueName, string message)
    {

        if (channel == null)
            channel = _Connection.CreateModel();
        channel.ConfirmSelect();
        if (props == null)
        {
            props = channel.CreateBasicProperties();
            props.Persistent = true;
        }
    
        channel.BasicReturn += (sender, ea) =>
        {
            // 如果消息不能被路由，这里将会被触发
            var body = ea.Body.ToArray();
            var message2 = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Returned message: {message2}, ReplyText: {ea.ReplyText}");
        };
        //channel.CallbackException += Channel_CallbackException;

        //try
        //{
        //channel.ExchangeDeclarePassive(exchange);
        //}
        //catch (Exception)
        //{
        //channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false);

        //}


        //channel.TxSelect();


        //// 声明并创建队列

        //channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        //channel.QueueBind(queueName, exchange, "", null);

        //lock (channellock)
        //{
        //channel.BasicPublish(exchange, "", true, props, message.GetBytes());
        //}
        //channel.TxCommit();

        if (!channel.WaitForConfirms())
        {
            Console.WriteLine("Message could not be confirmed");
        }



    }

    private static void Channel_CallbackException(object sender, CallbackExceptionEventArgs e)
    {

    }

    public static void Consume(string queueName, Action<string> messageHandler) => Consume(queueName, "DefExchange", "DefRout", messageHandler);
    public static void Consume(string queue, string exchange, string routingKey, Action<string> messageHandler)
    {
        var channel = _Connection.CreateModel();

        channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

        channel.ExchangeDeclare(exchange, ExchangeType.Direct, durable: true, autoDelete: false);

        channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue, exchange, "", null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            //var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            //messageHandler(message);
            channel.BasicAck(ea.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(queue,
            autoAck: false,
            consumerTag: _machineName,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer);
    }


    public static string Guid => System.Guid.NewGuid().ToString().Replace("-", "");


    public static string MD5(this string md5Str, int length)
    {
        if (string.IsNullOrEmpty(md5Str))
            return string.Empty;

        var md5 = System.Security.Cryptography.MD5.Create();
        var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(md5Str));
        var sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
            sb.Append(bytes[i].ToString("x2"));

        return sb.ToString().Substring(0, length);
    }

    public static string SerializeObject(this object obj) => JsonConvert.SerializeObject(obj);

    public static byte[] GetBytes(this string str) => Encoding.UTF8.GetBytes(str);


}

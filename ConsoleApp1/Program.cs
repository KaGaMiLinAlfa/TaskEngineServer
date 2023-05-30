using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class Program
{
    public static void Main(string[] args)
    {

        //Task.Run(() =>
        //{
        //    while (true)
        RabbitMQClient.Publish("kagami.test", "test");
        //});

        //RabbitMQClient.Consume("kagami.test", "kagamitest", "", msg =>
        //{
        //    zxc(msg);

        //    //Console.WriteLine($"qwe {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
        //});


        Console.WriteLine("zxc");
        Console.ReadLine();
    }


    public static void zxc(string args)
    {

    }
}

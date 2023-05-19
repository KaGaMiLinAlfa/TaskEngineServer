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


public class Program
{
    public static void Main(string[] args)
    {


    }

}
public class model1
{
    public List<model2> models { get; set; }
    public List<object> objs { get; set; }
}

public class model2
{

}
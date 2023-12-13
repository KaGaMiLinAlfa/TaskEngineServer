
using FreeSql.DataAnnotations;
using LogManager;
using Org.BouncyCastle.Asn1.X509;
using SDK.MiDuo.CodeService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using static Google.Protobuf.WellKnownTypes.Field.Types;
using System.Security.Policy;
using System.Threading;
using System.Xml.Linq;
using Task.Other;

namespace test
{
    public static class test
    {
        public static DateTime ToDateTime(this object value, DateTime defaultValue)
        {
            try
            {
                return (value == null || value == DBNull.Value) ? defaultValue : (value.GetType().Equals(typeof(DateTime)) ? Convert.ToDateTime(value) : DateTime.Parse(value.ToString()));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
    internal class Program
    {
        static string connectionString = "server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;";

        //server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;
        static void Main(string[] args)
        {
            //SharpCompress.Common.ArchiveEncoding ArchiveEncoding = new SharpCompress.Common.ArchiveEncoding();
            //new CodeHandleService().Run();

            //DateTime startTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00").ToDateTime(DateTime.MinValue);



            //Console.WriteLine("over");

            //AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            //AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            //var assembly2 = Assembly.LoadFrom("D:\\MiDuo\\Code\\TaskEngineServer\\ClassLibrary2\\bin\\Release\\netcoreapp3.1\\publish\\System.Text.Encoding.CodePages.dll");
            //var assembly = Assembly.LoadFrom("D:\\MiDuo\\Code\\TaskEngineServer\\ClassLibrary2\\bin\\Release\\netcoreapp3.1\\publish\\ClassLibrary2.dll");
            //var type = assembly.GetType("ClassLibrary2.Class1");
            //object obj = Activator.CreateInstance(type);
            //MethodInfo method = type.GetMethod("Run");
            //method.Invoke(obj, null);

            new TimingRequestTask().Run();


        }
        private static Assembly ResolveAssembly(object sender, ResolveEventArgs e)
        {
            //The name would contain versioning and other information. Let's say you want to load by name.
            string dllName = e.Name.Split(new[] { ',' })[0] + ".dll";

            var asd = Path.Combine("D:\\MiDuo\\Code\\TaskEngineServer\\ClassLibrary2\\bin\\Release\\netcoreapp3.1\\publish", dllName);

            return Assembly.LoadFrom(Path.Combine("D:\\MiDuo\\Code\\TaskEngineServer\\ClassLibrary2\\bin\\Release\\netcoreapp3.1\\publish", dllName));
        }

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            //根据加载失败类型的名字找到其所属程序集并返回
            if (args.Name.Split(",")[0] == "MessageDisplay.MessageDisplayHelper")
            {
                //我们自定义的程序集加载逻辑知道MessageDisplay.MessageDisplayHelper类属于MessageDisplay程序集，而MessageDisplay程序集在C:\AssemblyResolverConsle\Reference\MessageDisplay.dll这个路径下，所以这里加载这个路径下的dll文件作为TypeResolve事件处理函数的返回值
                return Assembly.LoadFile(@"C:\AssemblyResolverConsle\Reference\MessageDisplay.dll");
            }

            //如果TypeResolve事件的处理函数返回null，说明TypeResolve事件的处理函数也不知道加载失败的类型属于哪个程序集
            return null;
        }


        static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.MySql, connectionString)
            .UseAutoSyncStructure(true) //自动同步实体结构到数据库
            .Build(); //请务必定义成 Singleton 单例模式
    }

    class TaskInfo
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public int Id { get; set; }

        /// <summary>
        /// Task名称
        /// </summary>
        [Required]
        public string TaskName { get; set; }


    }
}

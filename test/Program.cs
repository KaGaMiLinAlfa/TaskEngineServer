
using FreeSql.DataAnnotations;
using LogManager;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace test
{
    internal class Program
    {
        static string connectionString = "server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;";

        //server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("MyVariable", "MyValue", EnvironmentVariableTarget.Process);


            string value = Environment.GetEnvironmentVariable("MyVariable", EnvironmentVariableTarget.Process);



            Console.WriteLine("over");
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

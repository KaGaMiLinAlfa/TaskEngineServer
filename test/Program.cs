﻿
using FreeSql.DataAnnotations;
using LogManager;
using SDK.MiDuo.CodeService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using Task.CodeHandle;
using Task.CodeHandle.EntityModel;

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

            DateTime startTime = DateTime.Now.ToString("yyyy-MM-dd 00:00:00").ToDateTime(DateTime.MinValue);

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

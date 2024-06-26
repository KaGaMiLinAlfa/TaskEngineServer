﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Node.Helper
{
    public class DB
    {
        public const string connectionString = "server=172.17.0.1;port=3306;database=TaskDB;uid=root;pwd=123123;";
        public const string connectionString2 = "server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;";

        public static IFreeSql FSql = new FreeSql.FreeSqlBuilder()
            .UseConnectionString(FreeSql.DataType.MySql, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? connectionString2 : connectionString)
            //.UseAutoSyncStructure(true) //自动同步实体结构到数据库
            .Build(); //请务必定义成 Singleton 单例模式
    }
}
using FreeSql.DataAnnotations;
using LogManager;
using Newtonsoft.Json;
using System;

namespace LogManager
{

    public enum LogLevel
    {
        Debug,
        Info,
        Warn,
        Error,
    }

    internal class TaskLog
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long Id { get; set; }
        public int TaskId { get; set; }
        public int NodeId { get; set; }
        public DateTime LogTime { get; set; }
        public int Level { get; set; }
        public string Message { get; set; }
    }
    public class LogHelper
    {
        static string connectionString = "server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;";

        static IFreeSql fsql = new FreeSql.FreeSqlBuilder()
    .UseConnectionString(FreeSql.DataType.MySql, connectionString)
    //.UseAutoSyncStructure(true) //自动同步实体结构到数据库
    .Build(); //请务必定义成 Singleton 单例模式

        public static void SaveLog(LogLevel logLevel, string msg, params object[] objs)
        {
            var msgObj = new TaskLog
            {
                Level = (int)logLevel,
                Message = msg,
                //Objects = objs

                NodeId = 0,
                TaskId = 0,
            };

            var zxc = fsql.Insert(msgObj).ExecuteAffrows();

            //if (fsql.Insert<TaskLog>().ExecuteAffrows() > 0)
            //    return;

            //throw new Exception($"录入日志失败");

            //RabbitMQClient.Publish("LogQueueName", JsonConvert.SerializeObject(msgObj));
        }

        public static void Debug(string msg, params object[] objs) => SaveLog(LogLevel.Debug, msg, objs);
        public static void Info(string msg, params object[] objs) => SaveLog(LogLevel.Info, msg, objs);
        public static void Warn(string msg, params object[] objs) => SaveLog(LogLevel.Warn, msg, objs);
        public static void Error(string msg, params object[] objs) => SaveLog(LogLevel.Error, msg, objs);
    }

    public class LogMQModel
    {
        public string Message { get; set; }
        public object[] Objects { get; set; }
        public LogLevel LogLevel { get; internal set; }
    }
}

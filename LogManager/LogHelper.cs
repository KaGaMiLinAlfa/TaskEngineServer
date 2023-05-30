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
    public class LogHelper
    {
        public static void SaveLog(LogLevel logLevel, string msg, params object[] objs)
        {
            var msgObj = new LogMQModel
            {
                LogLevel = logLevel,
                Message = msg,
                Objects = objs
            };

            RabbitMQClient.Publish("LogQueueName", JsonConvert.SerializeObject(msgObj));
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

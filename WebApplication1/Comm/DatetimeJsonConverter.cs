using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;

namespace Worker2.Comm
{
    /// <summary>
    /// Newtonsoft.Json自定义时间格式转换
    /// </summary>
    public class DatetimeJsonConverter : DateTimeConverterBase
    {
        //读取对象，DateTimeFormat为空时就不按DateTimeFormat的格式强制序列化，"2021-08-29"这种只有日期的字符串也能序列化成功
        private static IsoDateTimeConverter isoDateTimeConverterRaad = new IsoDateTimeConverter() { };
        //写对象，输出时间格式为"yyyy-MM-dd HH:mm:ss"
        private static IsoDateTimeConverter isoDateTimeConverterWrite = new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

        /// <summary>
        /// 重写输入时的时间序列化格式
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            object result = null;
            try
            {
                result = isoDateTimeConverterRaad.ReadJson(reader, objectType, existingValue, serializer);
            }
            catch (Exception)
            {
            }
            return result;
        }
        /// <summary>
        /// 重写输出时的时间格式
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            isoDateTimeConverterWrite.WriteJson(writer, value, serializer);
        }

    }
}

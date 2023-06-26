using System;
using System.ComponentModel;
using System.Linq;

namespace Worker2.Comm
{
    public static class Expansion
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return value.ToString();

            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            if (attributes != null && attributes.Any())
                return attributes.First().Description;

            return value.ToString();
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Domain.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attribute.Length > 0 ? ((DescriptionAttribute)attribute[0]).Description : value.ToString();
        }
    }
}
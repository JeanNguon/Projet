using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Proteca.Silverlight.Enums
{
    public static class EnumExtension
    {
        /// <summary>
        /// Will get the string value for a given enums value, this will
        /// only work if you assign the StringValue attribute to
        /// the items in your enum.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }


        public static Enum FindByStringValue(this Type enumType, string strValue)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            FieldInfo[] fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

            IEnumerable<FieldInfo> FIs = fields.Where(f => f.GetCustomAttributes(typeof(StringValueAttribute), false).Length > 0 && ((StringValueAttribute)f.GetCustomAttributes(typeof(StringValueAttribute), false)[0]).StringValue.ToLower() == strValue.ToLower());
            if (FIs.Count() > 0)
            {
                return (Enum)FIs.First().GetValue(enumType);
            }

            return null;
        }
    }
}

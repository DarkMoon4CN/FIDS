using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
namespace System.Data
{
    /// <summary>
    /// 匿名对象序列化为XML
    /// </summary>
    public static class XmlTools
    {
        //识别需要序列化的类型
        private static readonly Type[] WriteTypes = new[] {
        typeof(string), typeof(DateTime), typeof(Enum),
        typeof(decimal?), typeof(Guid),typeof(int?)
    };
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || WriteTypes.Contains(type);
        }
        public static XElement ToXml(this object input)
        {
            return input.ToXml(null);
        }
        public static XElement ToXml(this object input, string element)
        {
            if (input == null)
                return null;
            if (string.IsNullOrEmpty(element))
                element = "object";
            element = XmlConvert.EncodeName(element);
            var ret = new XElement(element);
            if (input != null)
            {
                var type = input.GetType();
                var props = type.GetProperties();
                var elements = from prop in props
                               let name = XmlConvert.EncodeName(prop.Name)
                               let val = prop.GetValue(input, null)
                               let value = prop.PropertyType.IsSimpleType()
                                    ? new XElement(name, val)
                                    : val.ToXml(name)
                               where value != null
                               select value;
                ret.Add(elements);
            }
            return ret;
        }
    }
}
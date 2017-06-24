using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace System.Data
{
    public static class DataExtension
    {
        public static bool IsNotNULL(this object obj)
        {
            return (DBNull.Value != obj) && obj != null;
        }

        public static short ToShort(this object obj)
        {
            if (obj.IsNotNULL())
            {
                return short.Parse(obj.ToString());
            }
            return 0;
        }
        public static int ToInt(this object obj)
        {
            if (obj.IsNotNULL())
            {
                return int.Parse(obj.ToString());
            }
            return 0;
        }
        public static Guid ToGUID(this object obj)
        {
            if (obj.IsNotNULL())
            {
                return Guid.Parse(obj.ToString());
            }
            return Guid.Empty;
        }

        public static DateTime ToDateTime(this object obj)
        {
            if (obj.IsNotNULL())
            {
                return DateTime.Parse(obj.ToString());
            }
            return DateTime.Parse("1900-01-01");
        }
        public static decimal ToDecimal(this object obj)
        {
            if (obj.IsNotNULL())
                return decimal.Parse(obj.ToString());
            return 0;
        }
        public static string SerializeJSON(this object obj)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (obj.IsNotNULL())
            {
                var str = js.Serialize(obj);
                str = Regex.Replace(str, @"\\/Date\((\d+)\)\\/", match =>
                {
                    DateTime dt = new DateTime(1970, 1, 1);
                    dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                    dt = dt.ToLocalTime();
                    return dt.ToString("yyyy-MM-dd HH:mm:ss");
                });
                return str;
            }
            return "1900-01-01";
        }
        public static T DeserializeJSON<T>(this string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(json);
        }

        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        public static DateTime TimeStampToTime(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime); return dtStart.Add(toNow);
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        public static int TimeToTimeStamp(this DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }


        /// <summary>
        /// 从 实体转化为XML字符串
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="obj">数据</param>
        /// <returns></returns>
        public static string SerializeXML<T>(this object obj)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(stream, obj);
                stream.Position = 0;
                StreamReader sr = new StreamReader(stream);
                string str = sr.ReadToEnd();
                return str;
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 从 XML字符串转化为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeserializeXML<T>(this string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Info(ex.Message);
                return default(T);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Xml;
using System.Threading;
using System.Xml.Linq;
using System.Linq;
using System.Text;
namespace CATC.FIDS.Utils
{
    public class Settings
    {

        private static Settings _instance = new Settings();

        /// <summary>
        /// Get setting
        /// </summary>
        public static Settings Instance
        {
            get { return _instance ?? (_instance = new Settings()); }
        }
        public T GetSetting<T>(String key, T defaultValue)
        {
            if (ConfigurationManager.AppSettings[key] == null)
                return defaultValue;
            if (typeof(T) == typeof(String))
                return (T)(object)ConfigurationManager.AppSettings[key];
            if (typeof(T) == typeof(long))
                return (T)(object)(Int64.Parse(ConfigurationManager.AppSettings[key]));
            if (typeof(T) == typeof(int))
                return (T)(object)(Int32.Parse(ConfigurationManager.AppSettings[key]));
            if (typeof(T) == typeof(bool))
                return (T)(object)(bool.Parse(ConfigurationManager.AppSettings[key]));
            return (T)(object)ConfigurationManager.AppSettings[key];
        }


        public string SiteDomain
        {
            get
            {
                // if it's called from eg a Task
                if (System.Web.HttpContext.Current == null)
                    return GetSetting("SiteDomain", "/");

                string url = HttpContext.Current.Request.Headers["Host"];

                return "http://" + url;
            }
        }

        public string GetErrorMsg(string code)
        {
            lock (this)
            {
                var p = DataRoot + "/Static/files/Errors.xml";
                var xml = new XmlHelper();
                xml.Load(p);
                var lst = xml.SelectNodes("Error");
                if (lst != null && lst.Count > 0)
                {
                    foreach (XmlElement item in from XmlElement item in lst where item != null where item.Attributes["ErrorCode"].Value.ToLowerInvariant() == code.ToLowerInvariant() select item)
                    {
                        return item.Attributes["ErrorInfo"].Value;
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 返回Weg.Config配置文件值
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public  string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }
        public string DataRoot
        {
            get
            {
                var st = ResolvePath(GetSetting("DataRoot", ""));
                if (string.IsNullOrEmpty(st))
                {
                    st = AppDomain.CurrentDomain.BaseDirectory;
                }
                return st;
            }
        }

        private string ResolvePath(string p)
        {
            if (p.ToLower().StartsWith(@"%dataroot%\"))
                p = Path.Combine(DataRoot, p.Substring(11));
            if (p.ToLower().StartsWith(@"%dataroot%"))
                p = Path.Combine(DataRoot, p.Substring(10));
            var extension = Path.GetExtension(p);
            if (!string.IsNullOrEmpty(extension) && !extension.ToLower().EndsWith("dat"))
            {
                var dir = new DirectoryInfo(p);
                if (!dir.Exists) dir.Create();
            }
            else if (!string.IsNullOrEmpty(p) && string.IsNullOrEmpty(extension))
            {
                var dir = new DirectoryInfo(p);
                if (!dir.Exists) dir.Create();
            }
            return p;
        }

        public String LogPath
        {
            get
            {
                return ResolvePath(GetSetting("LogPath", @"%dataroot%\Log"));

            }
        }


        public string ProjectDatabase
        {
            get
            {

                return ConfigurationManager.ConnectionStrings["ProjectDatabase"].ConnectionString;

            }
        }

        /// <summary>
        /// 返回两个日期之间的时间间隔
        /// </summary>
        /// <param name="Interval"> 返回类型 秒 ,分,小时,天等</param>
        /// <param name="StartDate">起始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <returns>返回两个日期之间的时间间隔</returns>

        public long DateDiff(string Interval, DateTime StartDate, DateTime EndDate)
        {
            long lngDateDiffValue = 0;


            TimeSpan TS = new TimeSpan(EndDate.Ticks - StartDate.Ticks);
            switch (Interval)
            {
                case "Second":
                    lngDateDiffValue = (long)TS.TotalSeconds;
                    break;
                case "Minute":
                    lngDateDiffValue = (long)TS.TotalMinutes;
                    break;
                case "Hour":
                    lngDateDiffValue = (long)TS.TotalHours;
                    break;
                case "Day":
                    lngDateDiffValue = (long)TS.Days;
                    break;
                case "Week":
                    lngDateDiffValue = (long)(TS.Days / 7);
                    break;
                case "Month":
                    //lngDateDiffValue = (long)(TS.Days / 30);
                    //应取两个时间的月份之差(季度和年同理) 
                    lngDateDiffValue = (long)(EndDate.Year - StartDate.Year) * 12 + (EndDate.Month - StartDate.Month);
                    break;
                case "Quarter":
                    //lngDateDiffValue = (long)((TS.Days / 30) / 3);
                    lngDateDiffValue = (long)(EndDate.Year - StartDate.Year) * 4 + (Quarter(EndDate) - Quarter(StartDate));
                    break;
                case "Year":
                    //lngDateDiffValue = (long)(TS.Days / 365);
                    lngDateDiffValue = (long)(EndDate.Year - StartDate.Year);
                    break;
            }
            return (lngDateDiffValue);
        }

        /// <summary>
        /// 以万为单位显示 t 为真四舍五入 t=false 不进行四舍五入
        /// </summary>
        /// <param name="number">金额</param>
        /// <param name="dec">小数位数</param>
        /// <param name="t">是否四舍五入</param>
        /// <returns></returns>
        public string GetWebConvertdisp(decimal number, int dec, bool t)
        {
            string str = "";
            decimal deci = 10000M;
            if (number < deci)
            {

                if (dec == 0)
                {
                    str = number.ToString("0");
                }
                else
                {
                    if (t == true)
                    {
                        str = GetDecimal(number, dec, true).ToString();
                    }
                    else
                    {
                        str = GetDecimal(number, dec, false).ToString();
                    }

                }
            }
            else if (number >= deci)
            {

                decimal df = number / deci;

                //   string strDecPart = "";                    // 存放小数部分的处理结果 
                // 存放整数部分的处理结果 
                string[] tmp = null;
                string strDigital = df.ToString();

                tmp = strDigital.Split(cDelim, 2); // 将数据分为整数和小数部分 

                if (tmp.Length > 1) //分解出了小数
                {
                    // strDecPart = ConvertDecimal(tmp[1]);


                    if (decimal.Parse(tmp[1]) > 0)
                    {
                        str = df.ToString("0.00") + "万";
                    }
                    else
                    {
                        str = df.ToString("0") + "万";
                    }

                }
                else
                {
                    str = df.ToString("0") + "万";
                }
            }



            return str;


        }



        #region GetDecimal
        private char[] cDelim = { '.' }; //小数分隔标识
        /// <summary>
        /// 获取几位小数点 bool t为真则四舍五入返回小数点位数,t为假则不进行四舍五入返回小数位数,默认为真
        /// </summary>
        /// <param name="rmb">The RMB.</param>
        /// <param name="n">保留小数点位数</param>
        /// <param name="t">if set to <c>true</c> [t].</param>
        /// <returns>返回字符串</returns>
        public decimal GetDecimal(decimal rmb = 0.00m, int n = 2, bool t = true)
        {
            decimal dec = 0.00M;

            if (t)
            {
                dec = Math.Round(rmb, n, MidpointRounding.AwayFromZero);
            }
            else
            {
                string[] tmp = null;
                string strDigital = rmb.ToString();
                tmp = strDigital.Split(cDelim, 2); // 将数据分为整数和小数部分 
                if (tmp.Length > 1) //分解出了小数
                {
                    if (tmp[1].Length <= n)
                    {
                        dec = rmb;
                    }
                    else
                    {
                        string dec1 = tmp[1].Substring(0, n);
                        strDigital = tmp[0] + "." + dec1;
                        dec = decimal.Parse(strDigital);
                    }

                }
                else
                {
                    dec = rmb;
                }
            }
            return dec;
        }
        #endregion



        /// <summary>
        /// 取得某个日期是本年度的第几个季度.
        /// </summary>
        /// <param name="tDate">The t date.</param>
        /// <returns>System.Int32.</returns>
        public int Quarter(DateTime tDate)
        {
            switch (tDate.Month)
            {
                case 1:
                case 2:
                case 3:
                    return 1;

                case 4:
                case 5:
                case 6:
                    return 2;

                case 7:
                case 8:
                case 9:
                    return 3;

                default:
                    return 4;
            }

        }


        public bool EnableCache { get { return GetSetting("EnableCache", false); } }
        public bool EnableLevel1Cache { get { return GetSetting("EnableLevel1Cache", false); } }
        public bool EnableLevel2Cache { get { return GetSetting("EnableLevel2Cache", false); } }


        public string Random { get { return GetSetting("Random", DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)); } }


        public DateTime LowCache
        {
            get { return GetSetting("LowCache", DateTime.Now.AddMinutes(2)); }
        }
        public DateTime LowerCache
        {
            get { return GetSetting("LowerCache", DateTime.Now.AddMinutes(20)); }
        }
        public DateTime NormalCache
        {
            get { return GetSetting("NormalCache", DateTime.Now.AddHours(2)); }
        }
        public DateTime HighCache
        {
            get { return GetSetting("HighCache", DateTime.Now.AddDays(1)); }
        }
        public DateTime HigherCache
        {
            get { return GetSetting("HigherCache", DateTime.Now.AddDays(7)); }
        }
        public DateTime HighestCache
        {
            get { return GetSetting("HigherCache", DateTime.Now.AddDays(30)); }
        }

        public string Guid
        {
            get { return System.Guid.NewGuid().ToString("N"); }
        }

        public string ClientIp2
        {
            get
            {
                string Ip = string.Empty;

                Ip = HttpContext.Current.Request.Headers["Cdn-Src-Ip"];
                if (string.IsNullOrEmpty(Ip))
                {


                    if (HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_FORWARDED_FOR"] == null)
                    {
                        if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
                            Ip = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"].ToString();
                        else
                            if (HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] != null)
                            Ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                        else
                            Ip = null;
                    }
                    else
                        Ip = HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_FORWARDED_FOR"];

                }
                return Ip;
            }
        }
        public string ClientIp
        {
            get
            {
                object clientIp;
                if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                {
                    clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                }
                else
                {
                    clientIp = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                if (null == clientIp)
                {
                    clientIp = HttpContext.Current.Request.UserHostAddress;
                }
                if (clientIp != null)
                    return clientIp.ToString();
                return null;
            }
        }


        public TimeSpan NoAbsoluteExpirationTimespan { get { return GetSetting("NoAbsoluteExpirationTimespan", TimeSpan.FromDays(1)); } }
        public DateTime NoSlidingExpirationTimespan { get { return GetSetting("NoSlidingExpirationTimespan", DateTime.Now.AddMinutes(20)); } }


      

      

       



        public string OrderCode
        {

            get
            {
                return
                    DateTime.Now.ToString("yyyyMMddHHmmssffff") + RndNum(2);

            }
        }

        public string TaskSettingFolder { get { return ResolvePath(GetSetting("TaskSettingFolder", @"%dataroot%\Log\Settings")); } }

        /// <summary>
        /// 生成随机的字母
        /// </summary>
        /// <param name="VcodeNum">生成字母的个数</param>
        /// <returns>string</returns>
        public string RndNum(int VcodeNum)
        {
            string Vchar = "0,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了
            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数
            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);
                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }

        /// <summary>
        /// 生成随机的字母
        /// </summary>
        /// <param name="VcodeNum">生成字母的个数</param>
        /// <returns>string</returns>
        public string RndNumChar(int VcodeNum)
        {
            //string Vchar = "0,1,2,3,4,5,6,7,8,9";
            // string Vchar = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,1,2,3,4,5,6,7,8,9,0";
            string Vchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,P,Q,R,S,T,U,V,W,X,Y,Z,1,2,3,4,5,6,7,8,9";
            string[] VcArray = Vchar.Split(',');
            string VNum = ""; //由于字符串很短，就不用StringBuilder了
            int temp = -1; //记录上次随机数值，尽量避免生产几个一样的随机数
            //采用一个简单的算法以保证生成随机数的不同
            Random rand = new Random();
            for (int i = 1; i < VcodeNum + 1; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(VcArray.Length);
                if (temp != -1 && temp == t)
                {
                    return RndNum(VcodeNum);
                }
                temp = t;
                VNum += VcArray[t];
            }
            return VNum;
        }




        /// <summary>
        /// 修改配置文件.
        /// </summary> 
        /// <param name="path"></param>
        /// <param name="nodes"></param>
        /// <param name="prmKey"></param>
        /// <param name="prmValue"></param>
        /// <param name="newAttrInfo">添加的属性值</param>
        public void ModifyConfig(string path = "", string nodes = "", string prmKey = "", string prmValue = "", IEnumerable<KeyValue> newAttrInfo = null)
        {

            lock (this)
            {
                var p = DataRoot + path;
                var doc = new XmlDocument();
                doc.Load(p);
                XmlNodeList nodeList = doc.SelectNodes(nodes);
                foreach (XmlNode node in nodeList)
                {
                    var v = node.Attributes[prmKey].Value;
                    if (v == prmValue)
                    {
                        if (newAttrInfo != null)
                        {
                            foreach (var o in newAttrInfo)
                            {
                                node.Attributes.Append(CreateAttribute(node, o.Key, o.Value));
                            }
                        }
                    }

                }
                doc.Save(p);
            }
        }


        public XmlAttribute CreateAttribute(XmlNode node, string attributeName, string value)
        {
            try
            {
                XmlDocument doc = node.OwnerDocument;
                XmlAttribute attr = null;
                attr = doc.CreateAttribute(attributeName);
                attr.Value = value;
                node.Attributes.SetNamedItem(attr);
                return attr;
            }
            catch (Exception err)
            {
                string desc = err.Message;
                return null;
            }
        }

        public string RemoveScript(string htmlstring)
        {
            //删除脚本   
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);


            htmlstring = htmlstring.Replace("<form", "<textarea style='width:900px;height:200px;'><form");
            htmlstring = htmlstring.Replace("</form>", "</form></textarea>");

            return htmlstring;
        }

        public string RemoveHtml(string htmlstring)
        {
            //删除脚本   
            htmlstring = Regex.Replace(htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML   
            htmlstring = Regex.Replace(htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            htmlstring = Regex.Replace(htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlstring = Regex.Replace(htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);

            htmlstring = htmlstring.Replace("<", "");
            htmlstring = htmlstring.Replace(">", "");
            htmlstring = htmlstring.Replace("\r\n", "");
            var htmlEncode = HttpContext.Current.Server.HtmlEncode(htmlstring);
            if (htmlEncode != null)
                htmlstring = htmlEncode.Trim();

            return htmlstring;
        }

        public string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }

        public string GetCallbackUrl(string path)
        {
            return Settings.Instance.SiteDomain + path;
        }
        public string QRCodeLink
        {
            get
            {
                return GetSetting("QRCodeLink", "");
            }
        }
       
        /// <summary>
        /// 由于win10系统获取到的时间会带上星期几 上午下午，用此方式重新定义一下格式便于测试
        /// </summary>
        public void SetSYSDateTimeFormat()
        {
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;
        }
       

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Base64Encoder(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            string orgStr = Convert.ToBase64String(bytes);
            return orgStr;
        }

        /// <summary>
        /// 获取xml配置文件数据. demo:  <SiteVersion Value="20150317134602" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">xml 路径 demo D:\\Log.xml </param>
        /// <param name="rootAttrName">User</param>
        /// <param name="targetAttrKey">目标属性名称 结合属性值判断唯一：UserName</param>
        /// <param name="targetAttrValue">属性值，判断唯一的：chuanglitou</param>
        /// <param name="getAttrKey">想要获取的属性名称： Password</param>
        /// <returns>20150317134602</returns>
        public string GetConfig(string path = "", string rootAttrName = "", string targetAttrKey = "", string targetAttrValue = "", string getAttrKey = "")
        {
            lock (this)
            {
                var p = DataRoot + path;
                var xml = new XmlHelper();
                xml.Load(p);
                var lst = xml.SelectNodes(rootAttrName);
                if (lst != null && lst.Count > 0)
                {
                    foreach (XmlElement item in from XmlElement item in lst where item != null where item.Attributes[targetAttrKey].Value.ToLowerInvariant() == targetAttrValue.ToLowerInvariant() select item)
                    {
                        return item.Attributes[getAttrKey].Value;
                    }
                }
            }
            return "";
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
}

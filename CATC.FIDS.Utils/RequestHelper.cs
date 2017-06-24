using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Utils
{
    public class RequestHelper
    {
        public static string SendHttpRequest(string requestURI, string requestMethod, string json)
        {
            //json格式请求数据
            string requestData = json;
            //拼接URL
            string serviceUrl = requestURI;//string.Format("{0}/{1}", requestURI, requestMethod);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            //post请求
            myRequest.Method = requestMethod;
            //utf-8编码
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(requestData);

            myRequest.ContentLength = buf.Length;
            myRequest.Timeout = 5000;
            //指定为json否则会出错
            myRequest.ContentType = "application/json";
            myRequest.MaximumAutomaticRedirections = 1;
            myRequest.AllowAutoRedirect = true;
            Stream newStream = myRequest.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();

            //获得接口返回值
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string ReqResult = reader.ReadToEnd();
            reader.Close();
            myResponse.Close();
            return ReqResult;
        }
    }
}

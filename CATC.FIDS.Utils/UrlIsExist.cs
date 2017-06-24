using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;

namespace CATC.FIDS.Utils
{
    public class UrlIsExist
    {
        public static bool IsUrlExist(string Url)
        {
            bool IsExist = false;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(Url));
                request.Method = "POST";
                request.Timeout = 15000;
                request.ContentLength = 0;
                ServicePointManager.Expect100Continue = false;

                ((HttpWebResponse)request.GetResponse()).Close();
                IsExist = true;
            }
            catch (WebException exception)
            {
                //if (exception.Status != WebExceptionStatus.ProtocolError)
                //{
                //    return num;
                //}
                //if (exception.Message.IndexOf("500 ") > 0)
                //{
                //    return 500;
                //}
                //if (exception.Message.IndexOf("401 ") > 0)
                //{
                //    return 401;
                //}
                //if (exception.Message.IndexOf("404") > 0)
                //{
                //    num = 404;
                //}
                IsExist = false;
            }
            catch (Exception ex)
            {
                IsExist = false;
            }

            return IsExist;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Utils
{
    public class JavaScriptHelper
    {
        #region 调用页面JS方法
        /// <summary>
        /// 方法用与配合页面 global.js 下的 Global.TransSferAlternately 方法使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <param name="otherContent"></param>
        /// <returns></returns>
        public static string Alternately(object id, string content, string otherContent)
        {
            if (!string.IsNullOrWhiteSpace(content) && !string.IsNullOrWhiteSpace(otherContent))
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                str.Append("<script>$(function(){");
                str.AppendFormat(" Global.TransSferAlternately('{0}','{1}','{2}'); ", id.ToString(), content, otherContent);
                str.Append("});</script>");
                return str.ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        ///  方法用与配合页面 Global.js 下的 Global.ServiceTime  方法使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeType"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ServiceTime(Guid id, int timeType, string time = null)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            str.AppendFormat(" Global.ServiceTime('{0}','{1}'); ", id, timeType);
            str.Append("});</script>");
            return str.ToString();
        }

        /// <summary>
        /// 方法用与配合页面 Global.js 下的 Global.BindElementEvent 方法使用
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="pclass">父类样式</param>
        /// <param name="tag">标记</param>
        /// <param name="isDraggable">是否可以移动</param>
        /// <param name="isMenu">是否有右键菜单</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static string BindElementEvent(Guid id, string pclass, int tag, int isDraggable, int isResizable, int isMenu)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            str.AppendFormat("Global.BindElementEvent('{0}','{1}','{2}','{3}','{4}','{5}');", id, pclass, tag, isDraggable, isResizable, isMenu);
            str.Append("});</script>");
            return str.ToString();
        }

        /// <summary>
        /// 方法用与配合页面 Global.js 下的 Global.BindElementEventV2 方法使用
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="eleName">可被jquery识别的父类节点,当 isDraggable、isResizable 为0时,此值无效</param>
        /// <param name="tag">标记</param>
        /// <param name="isDraggable">是否可以移动</param>
        /// <param name="isMenu">是否有右键菜单</param>
        /// <param name="arrayName">被哪个自定义ID集合push</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static string BindElementEventV2(Guid id, string eleName, int tag, int isDraggable, int isResizable, int isMenu, string arrayName = "header")
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            str.AppendFormat("Global.BindElementEventV2('{0}','{1}','{2}','{3}','{4}','{5}','{6}');", id, eleName, tag, isDraggable, isResizable, isMenu, arrayName);
            str.Append("});</script>");
            return str.ToString();
        }
        public static string Marquee(Guid id, int num)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            str.AppendFormat("Global.Marquee('{0}','{1}');", id, num);
            str.Append("});</script>");
            return str.ToString();
        }

        /// <summary>
        /// 方法用与配合页面 Global.js 下的 Global.BodyElementInit 方法使用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tag"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static string BodyElementInit(Guid id, int tag, string style)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            str.AppendFormat("Global.BodyElementInit('{0}','{1}','{2}');", id, tag, style);
            str.Append("});</script>");
            return str.ToString();
        }

        #endregion

        public static string SetTimeAxis(Guid id, string chineseString, string englishString)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            str.AppendFormat("Global.SetTimeAxis('{0}');", id+"_"+ chineseString + "_"+ englishString);
            str.Append("});</script>");
            return str.ToString();
        }


        /// <summary>
        /// 生成通用 调用页面上JavaScript的动态脚本
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public static string CallCurrentJavaScript(string methodName, List<string> parms)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append("<script>$(function(){");
            string parmsString = string.Empty;
            foreach (var item in parms)
            {
                parmsString += "'{"+item+"}',";
            }
            parmsString = parmsString.TrimEnd(',');
            str.Append(methodName+"("+parmsString+");");
            str.Append("});</script>");
            return str.ToString();
        }
    }
}

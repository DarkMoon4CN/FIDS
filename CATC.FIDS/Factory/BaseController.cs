using CATC.FIDS.Model;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace CATC.FIDS.Factory
{
    public class BaseController:Controller
    {
        internal CATC_FIDS_DBEntities ef = new CATC_FIDS_DBEntities();

        #region 由 实体 转成 xml 逻辑
        /// <summary>
        /// 用于组装移动的头标签 后转成的xml
        /// </summary>
        /// <param name="list">头部所有元素的集合</param>
        /// <returns></returns>
        internal XElement CreateHeaderXml(List<object> list)
        {
            XElement xheader = new XElement("Header");
            foreach (var item in list)
            {
                string nodeName = item.GetType().Name;
                xheader.Add(item.ToXml(nodeName));
            }
            return xheader;
        }

        /// <summary>
        /// 用于组装页面主体标签 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        internal XElement CreateBodyXml(List<object> list)
        {
            XElement xbody = new XElement("Body");
            foreach (var item in list)
            {
                string nodeName = item.GetType().Name;

                if (nodeName.Contains("TemplateTable"))
                {
                    XElement xtable = new XElement(nodeName);
                    var table = (TemplateTable)item;
                    //遍历table属性
                    System.Reflection.PropertyInfo[] properties = table.GetType().GetProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (properties[i].Name != "TDs")
                        {
                            xtable.Add(new XAttribute(properties[i].Name, properties[i].GetValue(table, null).ToString()));
                        }
                    }

                    foreach (var td in table.TDs)
                    {
                        string childNodeName = td.GetType().Name;
                        xtable.Add(td.ToXml(childNodeName));
                    }
                    xbody.Add(xtable);
                }
                else
                {
                    xbody.Add(item.ToXml(nodeName));
                }
            }
            return xbody;
        }


        /// <summary>
        /// 用于组装自定义节点的
        /// </summary>
        /// <param name="xName"></param>
        /// <param name="list"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        internal XElement CreateXml(string xName,List<object> list, string childName=null) {
            XElement obj = new XElement(xName);
            foreach (var item in list)
            {
                string nodeName =string.IsNullOrWhiteSpace(childName)?item.GetType().Name:childName;
                obj.Add(item.ToXml(nodeName));
            }
            return obj;
        }

        #endregion

        #region  由 xml 转换成 html元素 逻辑
        /// <summary>
        /// 解析页面中的数据 header xml逻辑
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="xml"></param>
        /// <param name="headerStr"></param>
        /// <param name="bodyStr"></param>
        /// <param name="isEditState">是否是编辑状态</param>
        internal void AnalysisHeaderXml(string actionKey, string xml, out string headerStr, bool isEditState = false)
        {
            Utils.XmlHelper xh = new XmlHelper();
            headerStr = string.Empty;

            xh.LoadXml(xml);
            var header = xh.SelectSingleNode("Header").ChildNodes;
            foreach (XmlElement item in header)
            {
                headerStr += CreateDefaultHtml(actionKey,item, isEditState);
            }
            
        }

        /// <summary>
        /// 解析页面中的 body xml 逻辑
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="xml"></param>
        /// <param name="bodyStr"></param>
        /// <param name="isEditState"></param>
        internal void AnalysisBodyXml(string actionKey, string xml, out string bodyStr, bool isEditState = false)
        {
            Utils.XmlHelper xh = new XmlHelper();
            bodyStr = string.Empty;
            xh.LoadXml(xml);
            var body = xh.SelectSingleNode("Body").ChildNodes;
            if (body != null && body.Count != 0)
            {
                //设置table初始化xml
                foreach (XmlElement item in body)
                {
                    bodyStr += CreateDefaultHtml(actionKey, item, isEditState);
                }
            }
        }

        /// <summary>
       ///  解析页面 footer xml 逻辑
       /// </summary>
       /// <param name="actionKey"></param>
       /// <param name="xml"></param>
       /// <param name="footerStr"></param>
       /// <param name="isEditState"></param>
        internal void AnalysisFooterXml(string actionKey,string xml,out string footerStr,bool isEditState = false)
        {
            Utils.XmlHelper xh = new XmlHelper();
            footerStr = string.Empty;

            xh.LoadXml(xml);
            var footer = xh.SelectSingleNode("Footer").ChildNodes;
            foreach (XmlElement item in footer)
            {
                footerStr += CreateDefaultHtml(actionKey, item, isEditState);
            }
        }

        /// <summary>
        /// 解析页面中的 table xml 逻辑,此元素只能做成 body xml 中唯一的
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="xml"></param>
        /// <param name="columns"></param>
        /// <param name="tableStr"></param>
        /// <param name="tds"></param>
        /// <param name="multipleType">功能类型，0 默认 1页面上有多个列头</param>
        internal void  AnalysisTableXml(string actionKey, string xml, List<DailySchedule_ExtFileds> columns,out string tableStr,out List<TemplateTD> tds,int multipleType = 0)
        {
            XmlHelper xh = new XmlHelper();
            tds = new List<TemplateTD>();
            tableStr = string.Empty;
            xh.LoadXml(xml);
            var body = xh.SelectSingleNode("Body").ChildNodes;
               
            if (body != null && body.Count != 0)
            {
                //设置table初始化xml
                foreach (XmlElement item in body)
                {
                    tableStr += CreateTableHtml(actionKey, item, columns, out tds,false,multipleType);
                }
            }
        }

        /// <summary>
        ///  解析页面中的 config  xml 配置 例 背景色等
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="xml"></param>
        /// <param name="configStr"></param>
        /// <param name="isEditState"></param>
        internal void AnalysisConfigXml(string actionKey, string xml, out string configStr, bool isEditState = false)
        {
            configStr = string.Empty;
            Utils.XmlHelper xh = new XmlHelper();
            xh.LoadXml(xml);
            var footer = xh.SelectSingleNode("Config").ChildNodes;
            foreach (XmlElement item in footer)
            {
                configStr += CreateDefaultHtml(actionKey, item, isEditState);
            }
        }

        /// <summary>
        /// 解析页面中 other(其他的) xml 逻辑 
        /// </summary>
        /// <param name="actionKey"></param>
        /// <param name="xml"></param>
        /// <param name="otherStr"></param>
        /// <param name="isEditState"></param>
        internal void AnalysisOtherXml(string actionKey, string xml, out string otherStr, bool isEditState = false)
        {
            otherStr = string.Empty;
            Utils.XmlHelper xh = new XmlHelper();
            xh.LoadXml(xml);
            var footer = xh.SelectSingleNode("Other").ChildNodes;
            foreach (XmlElement item in footer)
            {
                otherStr += CreateDefaultHtml(actionKey, item, isEditState);
            }
        }
        #endregion

        internal string CreateDefaultHtml(string actionKey,XmlElement item,bool isEditState = false)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            if (item.Name.Contains("TemplateLabel"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateLabel>();
                int isEdit = 1;
                str.AppendFormat("<div ");
                str.AppendFormat(" id ='{0}' style='{1}' tag='{2}'",entity.ID,entity.Style,entity.Tag);
                if (entity.Class != null && entity.Class.Length > 0)
                {
                    str.AppendFormat(" class='{0}' ",entity.Class);
                }
                str.Append(" >");
                str.AppendFormat("<div id='msg_{0}'>",entity.ID);
                str.Append(entity.Content);

                if (entity.ID.ToString().Contains("000000")==true)
                {
                    int last = entity.ID.ToString().Substring(entity.ID.ToString().Length - 1, 1).ToInt();
                    isEdit = (int)TagEnum.NoEditLabel == last ? 0 : 1;
                }
                if (isEditState == true && entity.IsSETime==true)
                {
                    str.Append(":00:00 — 24:00");
                }
                else if (entity.IsSETime == true)
                {
                    str.Append(":{#stime} - {#etime}");
                }
                str.AppendFormat("</div>");
                str.Append("</div>");


                if (isEditState == true && entity.IsSETime == true)
                {
                    str.Append(JavaScriptHelper.Alternately("msg_" + entity.ID
                         , entity.Content + ":00:00 — 24:00", entity.OtherContent + ":00:00 — 24:00"));
                }
                else if (entity.IsSETime == true)
                {
                    str.Append(JavaScriptHelper.Alternately("msg_" + entity.ID
                                              , entity.Content + ":{#stime} - {#etime}"
                                              , entity.OtherContent + ":{#stime} - {#etime}"));
                    str.Append(JavaScriptHelper.SetTimeAxis(entity.ID, entity.Content, entity.OtherContent));
                }
                else
                {
                    str.Append(JavaScriptHelper.Alternately("msg_" + entity.ID, entity.Content, entity.OtherContent));
                }
                if (isEditState)
                {
                    if (actionKey == "Bulletin")
                    {
                        str.Append(JavaScriptHelper.BindElementEventV2(entity.ID, "body", entity.Tag, isEdit, 1, isEdit, "other"));
                    }
                    else
                    {
                        str.Append(JavaScriptHelper.BindElementEvent(entity.ID, "header", entity.Tag, isEdit, 0, isEdit));
                    }
                }
            }
            else if (item.Name.Contains("TemplateImage"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateImage>();
                str.AppendFormat("<div ");
                str.AppendFormat(" id ='{0}' style='{1}' tag='{2}'", entity.ID, entity.Style,entity.Tag);
                if (entity.Class != null && entity.Class.Length > 0)
                {
                    str.AppendFormat(" class='{0}' ", entity.Class);
                }
                str.AppendFormat(" >");

                str.AppendFormat("<img src='{0}' />",entity.Url);
                str.Append("</div>");
                if (isEditState)
                {
                    if (actionKey == "BaggageClaim" || actionKey == "Checkins" || actionKey == "BoardingGateInfo")
                    {
                        str.Append(JavaScriptHelper.BindElementEvent(entity.ID, "header", entity.Tag, 1, 1, 1));
                    }
                    else if (actionKey == "SecurityBulletin")
                    {
                        str.Append(JavaScriptHelper.BindElementEventV2(entity.ID, "body", entity.Tag, 0, 0, 0,"body"));
                    }
                    else
                    {
                        str.Append(JavaScriptHelper.BindElementEvent(entity.ID, "header", entity.Tag, 1, 0, 1));
                    }
                }
            }
            if (item.Name.Contains("TemplateTime"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateTime>();
                str.Append("<div ");
                str.AppendFormat(" id ='{0}' style='{1}' tag='{2}' ", entity.ID, entity.Style,entity.Tag);
                if (entity.Class != null && entity.Class.Length > 0)
                {
                    str.AppendFormat(" class='{0}' ", entity.Class);
                }
                str.AppendFormat(" >");
                str.Append("<div class='timeBox'>13:00 "+DateTime.Now.ToString("yyyy/MM/dd")+"</div>");
                str.Append("</div>");
                str.Append(JavaScriptHelper.ServiceTime(entity.ID, entity.TimeType));
                if (isEditState)
                {
                    str.Append(JavaScriptHelper.BindElementEvent(entity.ID, "header", entity.Tag, 1,0,1));
                }
            }
            if (item.Name.Contains("TemplateMessage"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateMessage>();
                str.Append("<div ");
                str.AppendFormat(" id ='{0}' style='{1}' tag='{2}' ", entity.ID, entity.Style, entity.Tag);
                if (entity.Class != null && entity.Class.Length > 0)
                {
                    str.AppendFormat(" class='{0}' ", entity.Class);
                }
                str.AppendFormat(" >");
                str.Append(entity.Content);
                str.Append("</div>");
                str.Append(JavaScriptHelper.Alternately(entity.ID, entity.Content, entity.OtherContent));//多语言切换
                str.Append(JavaScriptHelper.Marquee(entity.ID,80));
            }

            if (item.Name.Contains("TemplateVideo"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateVideo>();
                str.Append("<div ");
                str.AppendFormat(" id ='{0}' style='{1}' tag='{2}' ", entity.ID, entity.Style, entity.Tag);
                if (entity.Class != null && entity.Class.Length > 0)
                {
                    str.AppendFormat(" class='{0}' ", entity.Class);
                }
                str.AppendFormat(" >");
                str.AppendFormat("<video controls='controls' src='{0}' autoplay='autoplay' loop='loop' width='100%'></video>",entity.Urls);
                str.Append("</div>");
            }

            //config 的 TemplateBgColor 一般都是在页面html->body下的节点,所以不能生产节点
            if (item.Name.Contains("TemplateBgColor"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateBgColor>();
                str.Append(JavaScriptHelper.BodyElementInit(entity.ID, entity.Tag,entity.Style));
            }

            if (item.Name.Contains("TemplateContent"))
            {
                var entity = item.OuterXml.DeserializeXML<TemplateContent>();
                str.Append("<div ");
                str.AppendFormat(" id ='{0}' style='{1}' tag='{2}' ", entity.ID, entity.Style, entity.Tag);
                if (entity.Class != null && entity.Class.Length > 0)
                {
                    str.AppendFormat(" class='{0}' ", entity.Class);
                }
                str.AppendFormat(" >");

                if (entity.Type == 1)//文本类型
                {
                    str.Append(entity.Content);
                }
                else if (entity.Type == 2)
                {
                    //预留
                }
                else if (entity.Type == 3)//图片类型
                {
                    str.AppendFormat("<img src = '{0}'/>", entity.Content);
                }
                str.Append("</div>");
                //写入页面时,bodyIDs.Push(ID);
                str.Append(JavaScriptHelper.BindElementEventV2(entity.ID,string.Empty,entity.Tag,0,0,0,"body"));
            }
            return str.ToString();
        }

        /// <summary>
        /// 用于特殊处理table元素生成
        /// </summary>
        /// <param name="item"></param>
        /// <param name="columns">页面业务的所有列</param>
        /// <param name="tds">out 后的完整列</param>
        /// <param name="isEditState"></param>
        /// <param name="multipleType"></param>
        /// <returns>已被选中table列的html FristRowsStr</returns>
        internal string  CreateTableHtml(string actionKey, XmlElement item,List<DailySchedule_ExtFileds> columns, out List<TemplateTD> tds, bool isEditState = false, int multipleType=0)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            tds = new List<TemplateTD>();
            if (item.Name.Contains("TemplateTable"))
            {
                var xtds=item.ChildNodes;
                foreach (XmlElement td in xtds)
                {
                    tds.Add(td.OuterXml.DeserializeXML<TemplateTD>());
                }
                tds=tds.OrderBy(o => o.Index).ToList();
                str.Append(" <div class='mainCon-hd is-hdColor'>");
                foreach (var td in tds)
                {
                    var ds_ext=columns.Where(c => c.filedID == td.FiledID).FirstOrDefault();
                    var id = multipleType==0?td.ID: Guid.NewGuid();
                    str.AppendFormat("<div id='{0}' class='{1}' index='{2}'>{3}</div>",id, td.Class,td.Index,ds_ext.chineseName);
                    str.Append(JavaScriptHelper.Alternately(id, ds_ext.chineseName, ds_ext.englishName));//给td增加中英文互换
                }

                str.Append("</div>");
            }
            return str.ToString();
        }

        /// <summary>
        ///  根据jarr["tag"]值,给元素实体赋值
        /// </summary>
        /// <param name="eventItem">对应的元素存放记录</param>
        /// <param name="jarr">待保存的数据json实体</param>
        /// <returns></returns>
        internal dynamic CreateTemplate(EventItem eventItem,JObject jarr)
        {
            //运行时类型，所有字段没有IDE验证机制
            dynamic templateObj = null;
            int tag = jarr["tag"].ToInt();
            if (tag == (int)TagEnum.DefaultLabel)
            {
                templateObj = JsonConvert.DeserializeObject<TemplateLabel>(eventItem.dataValue);
            }
            else if (tag == (int)TagEnum.DefaultImage)
            {
                templateObj = JsonConvert.DeserializeObject<TemplateImage>(eventItem.dataValue);
            }
            else if (tag == (int)TagEnum.DefaultTime)
            {
                templateObj = JsonConvert.DeserializeObject<TemplateTime>(eventItem.dataValue);
            }
            else if (tag == (int)TagEnum.DefaultVideo)
            {
                templateObj = JsonConvert.DeserializeObject<TemplateVideo>(eventItem.dataValue);
            }
            else if (tag == (int)TagEnum.DefaultBgColor)
            {
                //注：全局背景设置一般只有一个背景色
                //遇到BgColor或者Config 命名时，都是指向一个业务
                templateObj = JsonConvert.DeserializeObject<TemplateBgColor>(eventItem.dataValue);
            }
            else if (tag == (int)TagEnum.DefaultContent)
            {
                templateObj = JsonConvert.DeserializeObject<TemplateContent>(eventItem.dataValue);
            }
            if (jarr.Property("style") != null)
            {
                templateObj.Style = jarr["style"].ToString();
            }
            if (jarr.Property("class") != null)
            {
                templateObj.Class = jarr["class"].ToString();
            }
            if (jarr.Property("tag") != null)
            {
                templateObj.Tag = jarr["tag"].ToInt();
            }
            return templateObj;
        }

        internal List<DailySchedule_ExtFileds> GetDepColumns()
        {
            List<int> fids = new List<int>()
            {
                1,//实际时间
                6,//经停/终点站
                8,//航空公司
                10,//航徽
                11,//航班号
                12,//计划时间
                13,//备注 or 状态
                14,//值机柜台
                15,//共享号
                16//登机口
            };
            var dse = ef.DailySchedule_ExtFileds.Where(p => fids.Contains(p.filedID)).ToList();
            return dse;
        }

        internal List<DailySchedule_ExtFileds> GetArrColumns()
        {
            List<int> fids = new List<int>()
            {
                6,//经停/终点站
                8,//航空公司
                10,//航徽
                11,//航班号
                13,//备注 or 状态
                15,//共享号
                17,//实际时间
                18,//计划时间
                19,//行李转盘
            };
            var dse = ef.DailySchedule_ExtFileds.Where(p => fids.Contains(p.filedID)).ToList();
            return dse;
        }

        internal List<DailySchedule_ExtFileds> GetBaggageClaimColumns()
        {
            List<int> fids = new List<int>()
            {
                6,//经停/终点站
                10,//航徽
                11,//航班号
                13,//备注 or 状态
            };
            var dse = ef.DailySchedule_ExtFileds.Where(p => fids.Contains(p.filedID)).ToList();
            return dse;
        }

        internal List<DailySchedule_ExtFileds> GetBSDepartureAndDepartureColumns()
        {
            List<int> fids = new List<int>()
            {
                1,//实际时间
                6,//经停/终点站
                8,//航空公司
                10,//航徽
                11,//航班号
                12,//计划时间
                13,//备注 or 状态
                14,//值机柜台
                15,//共享号
                16//登机口
            };
            var dse = ef.DailySchedule_ExtFileds.Where(p => fids.Contains(p.filedID)).ToList();
            return dse;
        }

        #region 模版初始化 xml

        #region 规范示例
        /// <summary>
        /// 普通公告
        /// </summary>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        internal string BulletinInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            List<object> otherList = new List<object>();

            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion

            #region header 
            TemplateImage airLogo = new TemplateImage();//左边图标
            airLogo.Url = "/Images/img-small-plane01.png";
            airLogo.ID = Guid.NewGuid();
            airLogo.Class = "labelBox ui-draggable";
            airLogo.Style = "min-width: 1.2rem; width: 3.2rem; line-height: 0; border: 0px none; opacity: 1; left: 16.014%; top: 0%;";
            airLogo.Tag = 2;
            headerList.Add(airLogo);
            #endregion

            #region body
            TemplateContent tc = new TemplateContent();//中间的内容
            tc.ID = Guid.NewGuid();
            tc.Type = 1;
            tc.Tag = (int)TagEnum.DefaultContent;
            tc.Content = "欢迎使用民航电信FIDS-公告信息，如需修改文字请双击此处！！";
            bodyList.Add(tc);
            #endregion

            #region other
            TemplateLabel maxLabel = new TemplateLabel();//大号的标题字
            maxLabel.ID = Guid.NewGuid();
            maxLabel.Tag = 1;
            maxLabel.Class = "labelBox ui-draggable ui-resizable";
            maxLabel.IsBold = false;
            maxLabel.FontSize = "70px";
            maxLabel.Style = "color: rgb(15, 134, 225); background-color: transparent; font-size: 70px; border: 0px none; opacity: 1; left: 32.4248%; top: -2.06061%;";
            maxLabel.Content = "呼伦贝尔东山国际机场";
            maxLabel.Color = "#0f86e1";
            maxLabel.OtherContent = string.Empty;
            maxLabel.BgColor = "#fff";
            maxLabel.Opacity = "0";
            otherList.Add(maxLabel);

            TemplateLabel minLabel = new TemplateLabel();//小号的标题字
            minLabel.ID = Guid.NewGuid();
            minLabel.Tag = 1;
            minLabel.Class = "labelBox ui-draggable ui-resizable";
            minLabel.IsBold = false;
            minLabel.FontSize = "32px";
            minLabel.Style = "color: rgb(15, 134, 225); background-color: transparent; position: absolute; width: 767px; height: 96px; font-size: 36px; border: 0px none; opacity: 1; left: 31.0619%; top: 4.72727%;";
            minLabel.Content = "Hunlun Buir Dongshan Interational Airport";
            minLabel.Color = "#0f86e1";
            minLabel.OtherContent = string.Empty;
            minLabel.BgColor = "#fff";
            minLabel.Opacity = "0";
            otherList.Add(minLabel);
            #endregion

            return CreateCurrentInit(actionKey, headerList, bodyList, configList, otherList);
        }
        #endregion

        internal string BaggageClaimInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
           
            #endregion
            #region header 
            TemplateImage logo = new TemplateImage();//logo
            logo.Url = "/Images/img-small-plane02.png";
            logo.ID = Guid.NewGuid();
            logo.Class = "labelBox ui-draggable";
            logo.Style = "left:0%;min-width: 1.2rem; width: 3.2rem; line-height: 0; border: 0px none;";
            logo.Tag = 2;
            headerList.Add(logo);
            TemplateImage bcLogo = new TemplateImage();//baggage 图标
            bcLogo.Url = "/Images/baggage01.png";
            bcLogo.ID = Guid.NewGuid();
            bcLogo.Class = "labelBox ui-draggable";
            bcLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; position: absolute; width: 167px; opacity: 1; left: 20.897%; top: 0%;";
            bcLogo.Tag = 2;
            headerList.Add(bcLogo);

            TemplateLabel midLabel = new TemplateLabel();//行李提取文字
            midLabel.Tag = 1;
            midLabel.ID = Guid.NewGuid();
            midLabel.Class = "labelBox ui-draggable";
            midLabel.IsBold = false;
            midLabel.IsSETime = false;
            midLabel.FontSize = "60px";
            midLabel.Color = "#0f86e1";
            midLabel.Style = "font-size: 60px; color: #0f86e1; background-color: rgba(153, 51, 102, 0); border: 0px none; font-weight: bold; opacity: 1; left: 35.709%; top: 0%;";
            midLabel.Content = " 行 李 提 取 ";
            midLabel.OtherContent = "Baggage Claim";
            midLabel.BgColor = "#fff";
            midLabel.Opacity = "50";
            headerList.Add(midLabel);
            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.IsBold = true;
            time.Style = "border: 0px none; opacity: 1; left: 65.25%; top: 0%;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);

            TemplateLabel rigLabel = new TemplateLabel();//右侧文字
            //设置一个特殊的Guid 来做一个固定值
            rigLabel.ID = new Guid("00000000-0000-0000-0000-00000000000"+ (int)TagEnum.NoEditLabel); 
            rigLabel.Tag = 1;
            rigLabel.Class = "labelBox ui-draggable";
            rigLabel.IsBold = true;
            rigLabel.FontSize = "100px";
            rigLabel.Style = "background-color: rgb(231, 240, 250);font-size: 100px; left: 83.305%; top: 0px; display: block; font-weight: bold;";
            rigLabel.Content = "0 1";
            rigLabel.Color = "#0f86e1";
            rigLabel.OtherContent = "";
            rigLabel.BgColor = "#fff";
            rigLabel.Opacity = "0";
            headerList.Add(rigLabel);

            #endregion
            #region  body
            TemplateTable table = new TemplateTable();//数据主体设置
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            table.TDs = new List<TemplateTD>()
                    {
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-4",Style="", FiledID=11,Index=2, Remarks="Flight"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-4",Style="", FiledID=6,Index=3, Remarks="To/Via"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-4",Style="", FiledID=13,Index=6, Remarks="Remarks"},
                    };
            bodyList.Add(table);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string CheckinsInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 
            TemplateImage bcLogo = new TemplateImage();//Checkins 图标
            bcLogo.Url = "/Images/img-plane.png";
            bcLogo.ID = Guid.NewGuid();
            bcLogo.Class = "labelBox ui-draggable";
            bcLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; position: absolute;height: 390px; width: 791px; opacity: 1; left: 58.319%; top: 0%;";
            bcLogo.Tag = 2;
            headerList.Add(bcLogo);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string BoardingGateInfoInitXML(string actionKey)
        {
           
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            
            #endregion
            #region header 
            TemplateImage airLogo = new TemplateImage();//左边图标
            airLogo.Url = "/Images/logo.png";
            airLogo.ID = Guid.NewGuid();
            airLogo.Class = "labelBox ui-draggable";
            airLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; position: absolute; height: 358px; width: 1124px; opacity: 1; left: 7.269%; top: 0%;";
            airLogo.Tag = 2;
            headerList.Add(airLogo);
            TemplateImage bcLogo = new TemplateImage();//右边图标
            bcLogo.Url = "/Images/img-plane.png";
            bcLogo.ID = Guid.NewGuid();
            bcLogo.Class = "labelBox ui-draggable";
            bcLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; height: 358px; position: absolute; width: 826px; opacity: 1; left: 54.06%; top: 0%;";
            bcLogo.Tag = 2;
            headerList.Add(bcLogo);

            #endregion

            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string BoardingGuideInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 
            TemplateImage airLogo = new TemplateImage();//左边图标
            airLogo.Url = "/Images/img-small-plane01.png";
            airLogo.ID = Guid.NewGuid();
            airLogo.Class = "labelBox ui-draggable";
            airLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; position: absolute; height: 163px; width: 373px; opacity: 1; left: 0%; top: 0%;";
            airLogo.Tag = 2;
            headerList.Add(airLogo);

            TemplateLabel midLabel = new TemplateLabel();//航班计划文字
            midLabel.Tag = 1;
            midLabel.ID = Guid.NewGuid();
            midLabel.Class = "labelBox ui-draggable";
            midLabel.IsBold = false;
            midLabel.IsSETime = false;
            midLabel.FontSize = "60px";
            midLabel.Color = "#0f86e1";
            midLabel.Style = "color: rgb(0, 102, 255); background-color: transparent; font-size: 60px; border: 0px none; font-weight: bold; opacity: 1; left: 31.175%; top: 0%;";
            midLabel.Content = "乘 客 登 机 引 导";
            midLabel.OtherContent = "Boarding Guide";
            midLabel.BgColor = "#fff";
            midLabel.Opacity = "0";
            headerList.Add(midLabel);

            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.Style = "border: 0px none; opacity: 1; left: 67.064%; top: 0%;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string BSDepartureAndDepartureInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion

            #region header 

            #endregion

            #region  body
            TemplateTable table = new TemplateTable();//数据主体设置
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            table.TDs = new List<TemplateTD>()
                    {
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=8,Index=1, Remarks="AirLine"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=11,Index=2, Remarks="Flight"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=6,Index=3, Remarks="To/Via"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="",FiledID=1,Index=4, Remarks="STD"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=14,Index=5, Remarks="Checkins"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=13,Index=6, Remarks="Remarks"},
                    };
            bodyList.Add(table);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string BSDepartureAndArrivalInitXML()
        {
            XElement xd = new XElement("BSDepartureAndArrival");
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            var list = new List<object>{
                new {Index=0,ActionKey="DepartureFlight" },
                new {Index=1,ActionKey="ArrivalFlight" },
            };
            var configXml = CreateXml("Config", list, "Table");
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            xd.Add(configXml);
            xd.Add(xheader);
            xd.Add(xbody);
            return xd.ToString();
        }
        internal string VNWBoardingGateInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 
            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.Style = "border: 0px none; opacity: 1; left: 48.4952%; top: 0%;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string BoardingGateHorizInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 
            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.Style = "border: 0px none; z-index: 1; opacity: 1; left: 79.1485%; top: 0.5%;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string SpecialServiceInitXML(string actionKey)
        {
           
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 

            TemplateImage airLogo = new TemplateImage();//左边图标
            airLogo.Url = "/Images/img-plane.png";
            airLogo.ID = Guid.NewGuid();
            airLogo.Class = "labelBox ui-draggable";
            airLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; position: absolute; height: 358px; width: 630px; opacity: 1; left: 62.5781%;";
            airLogo.Tag = 2;
            headerList.Add(airLogo);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string VipServiceInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            
            #endregion
            #region header 
            TemplateImage airLogo = new TemplateImage();//左边图标
            airLogo.Url = "/Images/img-plane.png";
            airLogo.ID = Guid.NewGuid();
            airLogo.Class = "labelBox ui-draggable";
            airLogo.Style = "min-width: 1.2rem; line-height: 0; border: 0px none; position: absolute; height: 358px; width: 630px; opacity: 1; left: 62.5781%;";
            airLogo.Tag = 2;
            headerList.Add(airLogo);

            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string SecurityBulletinInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            List<object> otherList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
         
            #endregion
            #region header 

            #endregion
            #region body
            TemplateImage tc = new TemplateImage();//中间的内容
            tc.Style = "line-height: 0; border: 0px none; left: 0%; top: 0%;";
            tc.ID = Guid.NewGuid();
            tc.Tag = (int)TagEnum.DefaultImage;
            tc.Url = "/Images/img-securityBulletin.png";
            bodyList.Add(tc);

            #endregion
            #region other 

            #endregion

            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        /// <summary>
        /// 离港编辑页所有元素初始化
        /// </summary>
        /// <returns></returns>
        internal string DepartureFlightInitXML(string actionKey)
        {
           
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion

            #region header 
            TemplateImage logo = new TemplateImage();//logo
            logo.Url = "/Images/img-small-plane01.png";
            logo.ID = Guid.NewGuid();
            logo.Class = "labelBox ui-draggable";
            logo.Style = "left:0%;";
            logo.Tag = 2;
            headerList.Add(logo);
            TemplateLabel midLabel = new TemplateLabel();//航班计划文字
            midLabel.Tag = 1;
            midLabel.ID = Guid.NewGuid();
            midLabel.Class = "labelBox ui-draggable";
            midLabel.IsBold = false;
            midLabel.IsSETime = true;
            midLabel.FontSize = "40px";
            midLabel.Color = "#0f86e1";
            midLabel.Style = "left: 21.863%; top: 0px; font-size: 40px; border: 0px none;";
            midLabel.Content = "本屏航班计划时间";
            midLabel.OtherContent = "Schedule Period Of Validity";
            midLabel.BgColor = "#fff";
            midLabel.Opacity = "50";
            headerList.Add(midLabel);
            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.Style = "background-color: rgb(255, 255, 255); left: 64.282%; top: 0px;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);

            TemplateLabel rigLabel = new TemplateLabel();//右侧文字
            rigLabel.ID = Guid.NewGuid();
            rigLabel.Tag = 1;
            rigLabel.Class = "labelBox ui-draggable";
            rigLabel.IsBold = false;
            rigLabel.FontSize = "48px";
            rigLabel.Style = "background-color: rgb(255, 255, 255); left: 83.305%; top: 0px; display: block;";
            rigLabel.Content = "离港航班";
            rigLabel.Color = "#0f86e1";
            rigLabel.OtherContent = "Departure";
            rigLabel.BgColor = "#fff";
            rigLabel.Opacity = "50";
            headerList.Add(rigLabel);

            #endregion

            #region  body
            TemplateTable table = new TemplateTable();//数据主体设置
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            table.TDs = new List<TemplateTD>()
                    {
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=8,Index=1, Remarks="AirLine"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=11,Index=2, Remarks="Flight"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=6,Index=3, Remarks="To/Via"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="",FiledID=1,Index=4, Remarks="STD"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=14,Index=5, Remarks="Checkins"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=13,Index=6, Remarks="Remarks"},
                    };
            bodyList.Add(table);
            #endregion

            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }
        internal string ArrivalFlightInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 
            TemplateImage logo = new TemplateImage();//logo
            logo.Url = "/Images/img-small-plane01.png";
            logo.ID = Guid.NewGuid();
            logo.Class = "labelBox ui-draggable";
            logo.Style = "left:0%;";
            logo.Tag = 2;
            headerList.Add(logo);
            TemplateLabel midLabel = new TemplateLabel();//航班计划文字
            midLabel.Tag = 1;
            midLabel.ID = Guid.NewGuid();
            midLabel.Class = "labelBox ui-draggable";
            midLabel.IsBold = false;
            midLabel.IsSETime = true;
            midLabel.FontSize = "40px";
            midLabel.Color = "#0f86e1";
            midLabel.Style = "left:21.863%; top: 0px; font-size: 40px; border: 0px none;";
            midLabel.Content = "本屏航班计划时间";
            midLabel.OtherContent = "Schedule Period Of Validity";
            midLabel.BgColor = "#fff";
            midLabel.Opacity = "50";
            headerList.Add(midLabel);
            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.Style = "background-color: rgb(255, 255, 255); left: 64.282%; top: 0px;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);

            TemplateLabel rigLabel = new TemplateLabel();//右侧文字
            rigLabel.ID = Guid.NewGuid();
            rigLabel.Tag = 1;
            rigLabel.Class = "labelBox ui-draggable";
            rigLabel.IsBold = false;
            rigLabel.FontSize = "48px";
            rigLabel.Style = "background-color: rgb(255, 255, 255); left: 83.305%; top: 0px; display: block;";
            rigLabel.Content = "到港航班";
            rigLabel.Color = "#0f86e1";
            rigLabel.OtherContent = "Arrival";
            rigLabel.BgColor = "#fff";
            rigLabel.Opacity = "50";
            headerList.Add(rigLabel);

            #endregion
            #region  body
            TemplateTable table = new TemplateTable();//数据主体设置
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            table.TDs = new List<TemplateTD>()
                    {
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=8,Index=1, Remarks="AirLine"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=11,Index=2, Remarks="Flight"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=6,Index=3, Remarks="To/Via"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="",FiledID=17,Index=4, Remarks="ATA"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=18,Index=5, Remarks="STA"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=19,Index=6, Remarks="Baggage"},
                    };
            bodyList.Add(table);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }

        internal string BSArrivalFlightInitXML(string actionKey)
        {
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();
            #region 全局背景
            TemplateBgColor bgcolor = new TemplateBgColor();
            bgcolor.BgType = 2;//图片类型
            bgcolor.BgImageUrl = "/Images/bg-1.jpg";
            bgcolor.ID = Guid.NewGuid();
            bgcolor.Tag = 9;
            bgcolor.Opacity = string.Empty;
            bgcolor.BgColor = string.Empty;
            configList.Add(bgcolor);
            #endregion
            #region header 
            TemplateImage logo = new TemplateImage();//logo
            logo.Url = "/Images/img-small-plane01.png";
            logo.ID = Guid.NewGuid();
            logo.Class = "labelBox ui-draggable";
            logo.Style = "left:0%;";
            logo.Tag = 2;
            headerList.Add(logo);
            TemplateLabel midLabel = new TemplateLabel();//航班计划文字
            midLabel.Tag = 1;
            midLabel.ID = Guid.NewGuid();
            midLabel.Class = "labelBox ui-draggable";
            midLabel.IsBold = false;
            midLabel.IsSETime = true;
            midLabel.FontSize = "40px";
            midLabel.Color = "#0f86e1";
            midLabel.Style = "left:21.863%; top: 0px; font-size: 40px; border: 0px none;";
            midLabel.Content = "本屏航班计划时间";
            midLabel.OtherContent = "Schedule Period Of Validity";
            midLabel.BgColor = "#fff";
            midLabel.Opacity = "50";
            headerList.Add(midLabel);
            TemplateTime time = new TemplateTime();//时间轴
            time.ID = Guid.NewGuid();
            time.Tag = 3;
            time.Color = "#0f86e1";
            time.FontSize = "48px";
            time.Style = "background-color: rgb(255, 255, 255); left: 64.282%; top: 0px;";
            time.Class = "labelBox ui-draggable";
            time.TimeType = 5;//默认形态
            headerList.Add(time);

            TemplateLabel rigLabel = new TemplateLabel();//右侧文字
            rigLabel.ID = Guid.NewGuid();
            rigLabel.Tag = 1;
            rigLabel.Class = "labelBox ui-draggable";
            rigLabel.IsBold = false;
            rigLabel.FontSize = "48px";
            rigLabel.Style = "background-color: rgb(255, 255, 255); left: 83.305%; top: 0px; display: block;";
            rigLabel.Content = "到港航班";
            rigLabel.Color = "#0f86e1";
            rigLabel.OtherContent = "Arrival";
            rigLabel.BgColor = "#fff";
            rigLabel.Opacity = "50";
            headerList.Add(rigLabel);

            #endregion
            #region  body
            TemplateTable table = new TemplateTable();//数据主体设置
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            table.TDs = new List<TemplateTD>()
                    {
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=8,Index=1, Remarks="AirLine"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=11,Index=2, Remarks="Flight"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=6,Index=3, Remarks="To/Via"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="",FiledID=17,Index=4, Remarks="ATA"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=18,Index=5, Remarks="STA"},
                        new TemplateTD() { ID=System.Guid.NewGuid(),Class="col-2",Style="", FiledID=13,Index=6, Remarks="Remarks"},
                    };
            bodyList.Add(table);
            #endregion
            return CreateCurrentInit(actionKey, headerList, bodyList, configList);
        }

        /// <summary>
        /// 模板初始化入口
        /// </summary>
        /// <param name="actionKey"></param>
        /// <returns></returns>
        internal string TemplateInitEntrance(string actionKey,ref string description)
        {
            string definition = string.Empty;
            switch (actionKey)
            {
                case "DepartureFlight": description = "离港航班"; definition =  DepartureFlightInitXML(actionKey); break;
                case "ArrivalFlight": description = "到港航班"; definition =ArrivalFlightInitXML(actionKey); break;
                case "BaggageClaim": description = "行李提取"; definition = BaggageClaimInitXML(actionKey); break;
                case "Checkins": description = "值机柜台"; definition = CheckinsInitXML(actionKey); break;
                case "BSDepartureAndDeparture": description = "离港大屏"; definition = BSDepartureAndDepartureInitXML(actionKey); break;
                case "BoardingGate": description = "竖版登机口(含气象)"; definition = string.Empty; break;
                case "BoardingGateInfo": description = "横版登机口(无气象)"; definition = BoardingGateInfoInitXML(actionKey); break;
                case "BoardingGuide": description = "登机引导"; definition = BoardingGuideInitXML(actionKey); break;
                case "Bulletin": description = "普通公告"; definition =  BulletinInitXML(actionKey); break;
                case "SecurityBulletin": description = "特殊公告"; definition = SecurityBulletinInitXML(actionKey); break;
                case "VNWBoardingGate": description = "竖版登机口(无气象)"; definition = VNWBoardingGateInitXML(actionKey); break;
                case "BoardingGateHoriz": description = "横版登机口(含气象)"; definition = BoardingGateHorizInitXML(actionKey); break;
                case "SpecialService": description = "特殊旅客"; definition = SpecialServiceInitXML(actionKey); break;
                case "VipService": description = "VIP旅客"; definition = VipServiceInitXML(actionKey); break;
                case "BSArrivalAndArrival": description = "进港大屏"; definition = BSArrivalFlightInitXML(actionKey); break;
                default: definition = string.Empty; break;
            }
            return definition;
        }

        internal string CreateCurrentInit(string actionKey=null
                      ,List<object> headerList=null, List<object> bodyList=null
                      ,List<object> configList=null, List<object> otherList=null)
        {
            headerList = headerList == null ? headerList = new List<object>() : headerList;
            bodyList = bodyList == null ? bodyList = new List<object>() : bodyList;
            configList = configList == null ? configList = new List<object>() : configList;
            otherList = otherList == null ? otherList = new List<object>() : otherList;

            XElement xd = new XElement(actionKey);
            #region 批量增加至元素事务表
            List<object> allList = new List<object>();
            allList.AddRange(headerList);
            allList.AddRange(bodyList);
            allList.AddRange(configList);
            allList.AddRange(otherList);

            List<EventItem> addEventItems = new List<EventItem>();
            //增加元素事务
            foreach (var item in allList)
            {
                EventItem events = new EventItem();
                events.creator = "系统初始化";
                events.dataID = ((TemplateBase)item).ID;
                events.creationtime = DateTime.Now;
                events.dataValue = item.SerializeJSON();
                addEventItems.Add(events);
            }
            ef.EventItem.AddRange(addEventItems);
            #endregion
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            var xother = CreateXml("Other", otherList);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            xd.Add(xother);
            return xd.ToString();
        }
        #endregion
    }
}
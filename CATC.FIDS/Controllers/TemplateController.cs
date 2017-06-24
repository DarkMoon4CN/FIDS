using CATC.FIDS.Model;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;
using CATC.FIDS.Factory;
using Webdiyer.WebControls.Mvc;
namespace CATC.FIDS.Controllers
{

    /// <summary>
    /// 搭建所有展示页面
    /// </summary>
    public partial class TemplateController : BaseController
    { 
        #region 离港信息
        /// <summary>
        /// 离港信息模板
        /// </summary>
        /// <returns></returns>
        public ActionResult Departureflight()
        {
            string actionKey = "DepartureFlight";
            //设置页面配置信息
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            
            //获取配置
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null)
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion
 
        #region 到港信息
        public ActionResult Arrivalflight()
        {
            string actionKey = "ArrivalFlight";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null)
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion

        #region 行李提取信息
        public ActionResult BaggageClaim()
        {
            string actionKey = "BaggageClaim";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null)
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion

        #region 值机柜台信息
        public ActionResult Checkins()
        {
            string actionKey = "Checkins";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null)
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }
        #endregion

        #region 离港航班大屏
        public ActionResult BSDepartureflight()
        {
            string actionKey = "BSDepartureFlight";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string bstr = string.Empty;
            if (template != null)
            {
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
                AnalysisBodyXml(actionKey, template.definition, out bstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.bstr = bstr;
            return View();
        }

        #endregion

        #region 进出港航班3个大屏

        /// <summary>
        /// 进出港航班大屏
        /// </summary>
        /// <returns></returns>
        public ActionResult BSDepartureAndArrival()
        {
            //string actionKey = "BSDepartureAndArrival";
            //var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            //string leftKey = string.Empty;
            //string rightKey = string.Empty;
            //if (template != null && string.IsNullOrWhiteSpace(template.definition)==false)
            //{
            //    XmlHelper xh = new XmlHelper();
            //    xh.LoadXml(template.definition);
            //    var config = xh.SelectSingleNode("Config").ChildNodes;
            //    if (config != null && config.Count != 0)
            //    {
            //        foreach (XmlElement table in config)
            //        {
            //            var pars = table.ChildNodes;
            //            var index = table.ChildNodes[0].InnerText.ToString();
            //            var ak = table.ChildNodes[1].InnerText.ToString();//模块的actionKey
            //            if (index == "0")
            //            {
            //                leftKey = ak;
            //            }
            //            if(index=="1")
            //            {
            //                rightKey = ak;
            //            }
            //        }
            //    }
            //}
            //ViewBag.leftKey = leftKey;
            //ViewBag.rightKey = rightKey;
            return View();
        }

        public ActionResult BSDepartureAndDeparture()
        {
            return View();
        }

        public ActionResult BSArrivalAndArrival()
        {
            return View();
        }

        #endregion

        #region 登机引导信息
        public ActionResult BoardingGuide()
        {
            string actionKey = "BoardingGuide";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null)
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion

        #region 普通公告
        public ActionResult Bulletin()
        {
            string actionKey = "Bulletin";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string cstr = string.Empty;
            string ostr = string.Empty;
            string bstr = string.Empty;
            if (template != null)
            {
                AnalysisOtherXml(actionKey, template.definition, out ostr, true);
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
                AnalysisBodyXml(actionKey, template.definition, out bstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            ViewBag.ostr = ostr;
            ViewBag.bstr = bstr;
            return View();
        }
        #endregion

        #region 安全公告
        public ActionResult SecurityBulletin()
        {
            string actionKey = "SecurityBulletin";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            string hstr = string.Empty;
            string cstr = string.Empty;
            string ostr = string.Empty;
            string bstr = string.Empty;
            if (template != null && !string.IsNullOrWhiteSpace(template.definition))
            {
                AnalysisOtherXml(actionKey, template.definition, out ostr, true);
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
                AnalysisBodyXml(actionKey, template.definition, out bstr, true);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            ViewBag.ostr = ostr;
            ViewBag.bstr = bstr;
            return View();
        }
        #endregion

        #region 特殊旅客页面
        public ActionResult SpecialService()
        {
            string actionKey = "SpecialService";
            string iata = DNTRequest.GetString("iata");//航空公司ID
                                                       
            var airEntity = ef.F_Airline.Where(p => p.Airline_IATA == iata).FirstOrDefault();

            //设置页面配置信息
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            //获取配置
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null && !string.IsNullOrWhiteSpace(template.definition))
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            //获取航空公司logo
            ViewBag.logo = airEntity != null ? airEntity.MaxLogo : "/Images/logo.png";
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }
        #endregion

        #region vip旅客页面

        public ActionResult VipService()
        {
            string actionKey = "VipService";
            string iata = DNTRequest.GetString("iata");//航空公司ID
            var airEntity = ef.F_Airline.Where(p => p.Airline_IATA == iata).FirstOrDefault();

            //设置页面配置信息
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            //获取配置
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null && !string.IsNullOrWhiteSpace(template.definition))
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr, true);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            //获取航空公司logo
            ViewBag.logo = airEntity != null ? airEntity.MaxLogo : "/Images/logo.png";
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion

        #region 模板保存动作
        /// <summary>
        ///  增加 或 修改 模板 json 之间 || 做分割
        /// </summary>
        /// <returns></returns>
        public JsonResult DoAddDepartureTemplate()
        {
            string actionKey = "DepartureFlight";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string[] headerSplit = header.Split(new char[2] { '|','|'});
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            bool allEdited = true;

            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }

            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        allEdited = false;
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem,jarr);
                    headerList.Add(TemplateObj);
                }
            }


            TemplateTable table = new TemplateTable();
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            List<TemplateTD> tds = new List<TemplateTD>();
            for (int i = 0; i < bodySplit.Length; i++)
            {
                var p = bodySplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    //里面保存的都是按顺序的table列ID
                    TemplateTD td = new TemplateTD();
                    td.Class = "col-2";
                    td.ID = Guid.NewGuid();
                    td.FiledID = p.ToInt();
                    td.Index = i + 1;
                    td.Style = string.Empty;
                    td.Tag = td.Index;
                    td.Remarks = "离港编辑TD";
                    tds.Add(td);
                }
            }
            table.TDs = tds;
            bodyList.Add(table);

            var result = new ResultDto<string>();
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            //离港xml
            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd"+actionKey+"Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "离港航班",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime=DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        public JsonResult DoAddBSDepartureTemplate()
        {
            string actionKey = "BSDepartureFlight";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            bool allEdited = true;
            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        allEdited = false;
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }

            for (int i = 0; i < bodySplit.Length; i++)
            {
                var p = bodySplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    bodyList.Add(TemplateObj);
                }
            }
            var result = new ResultDto<string>();
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            //离港xml
            XElement xd = new XElement(actionKey);
            xd.Add(xheader);
            xd.Add(xbody);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd" + actionKey + "Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "离港大屏",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        
        public JsonResult DoAddArrivalTemplate()
        {
            string actionKey = "ArrivalFlight";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            bool allEdited = true;

            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }

            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        allEdited = false;
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }

            TemplateTable table = new TemplateTable();
            table.ID = System.Guid.NewGuid();
            table.Border = 1;
            table.Class = "mainCon-hd is-hdColor";
            table.Style = "";
            List<TemplateTD> tds = new List<TemplateTD>();
            for (int i = 0; i < bodySplit.Length; i++)
            {
                var p = bodySplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    //里面保存的都是按顺序的table列ID
                    TemplateTD td = new TemplateTD();
                    td.Class = "col-2";
                    td.ID = Guid.NewGuid();
                    td.FiledID = p.ToInt();
                    td.Index = i + 1;
                    td.Style = string.Empty;
                    td.Tag = td.Index;
                    td.Remarks = "进港编辑TD";
                    tds.Add(td);
                }
            }
            table.TDs = tds;
            bodyList.Add(table);

            var result = new ResultDto<string>();
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }

            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            //到港xml
            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd"+actionKey+"Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "到港航班",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        public JsonResult DoAddBaggageClaimTemplate()
        {
            string actionKey = "BaggageClaim";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            bool allEdited = true;

            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }
            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        allEdited = false;
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }
            var result = new ResultDto<string>();
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd"+ actionKey + "Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "行李提取",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        public JsonResult DoAddCheckinsTemplate()
        {
            string actionKey = "Checkins";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            //bool allEdited = true;

            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }

            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }

            var result = new ResultDto<string>();
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd"+actionKey+"Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "值机柜台",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        public JsonResult DoAddBoardingGuideTemplate()
        {
            string actionKey = "BoardingGuide";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            bool allEdited = true;
            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }
            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        allEdited = false;
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }

            var result = new ResultDto<string>();
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);

            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd" + actionKey + "Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "登机引导",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        public JsonResult DoAddBulletinTemplate()
        {

            string actionKey = "Bulletin";
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string other = DNTRequest.GetString("other");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            string[] otherSplit = other.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            List<object> otherList = new List<object>();
            bool allEdited = true;
            #region 装载 config
            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }
            #endregion
            #region 装载 header
            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }
            #endregion
            #region 装载body
            for (int i = 0; i < bodySplit.Length; i++)
            {
                var p = bodySplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    bodyList.Add(TemplateObj);
                }
            }
            #endregion
            #region 装载 other
            for (int i = 0; i < otherSplit.Length; i++)
            {
                var p = otherSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    otherList.Add(TemplateObj);
                }
            }
            #endregion
            var result = new ResultDto<string>();
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            var xother = CreateXml("Other",otherList);

            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            xd.Add(xother);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd" + actionKey + "Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = "普通公告",
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);
        }

        /// <summary>
        /// 通用的模板保存
        /// </summary>
        /// <returns></returns>
        public JsonResult DoAddCurrentTemplate()
        {
            var result = new ResultDto<string>();
            string actionKey = DNTRequest.GetString("actionKey");
            if (string.IsNullOrWhiteSpace(actionKey))
            {
                result.Status = 0;
                result.Message = "模板名(actionKey)无法正常接收！！";
                return Json(result);
            }
            string header = DNTRequest.GetString("header");
            string body = DNTRequest.GetString("body");
            string config = DNTRequest.GetString("config");
            string other = DNTRequest.GetString("other");
            string[] headerSplit = header.Split(new char[2] { '|', '|' });
            string[] bodySplit = body.Split(new char[2] { '|', '|' });
            string[] otherSplit = other.Split(new char[2] { '|', '|' });
            List<object> headerList = new List<object>();
            List<object> bodyList = new List<object>();
            List<object> configList = new List<object>();//大部分用于背景
            List<object> otherList = new List<object>();
            bool allEdited = true;
            #region 装载 config
            if (!string.IsNullOrWhiteSpace(config))
            {
                var jarr = (JObject)JsonConvert.DeserializeObject(config);
                var flag = jarr["id"].ToGUID();
                var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                configList.Add(TemplateObj);
            }
            #endregion
            #region 装载 header
            for (int i = 0; i < headerSplit.Length; i++)
            {
                var p = headerSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    headerList.Add(TemplateObj);
                }
            }
            #endregion
            #region 装载body
            for (int i = 0; i < bodySplit.Length; i++)
            {
                var p = bodySplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    bodyList.Add(TemplateObj);
                }
            }
            #endregion
            #region 装载 other
            for (int i = 0; i < otherSplit.Length; i++)
            {
                var p = otherSplit[i];
                if (!string.IsNullOrWhiteSpace(p))
                {
                    var jarr = (JObject)JsonConvert.DeserializeObject(p);
                    var flag = jarr["id"].ToGUID();
                    var eventItem = ef.EventItem.Where(t => t.dataID == flag).FirstOrDefault();
                    if (eventItem == null)
                    {
                        //页面有未编辑的元素
                        break;
                    }
                    dynamic TemplateObj = base.CreateTemplate(eventItem, jarr);
                    otherList.Add(TemplateObj);
                }
            }
            #endregion
            if (allEdited == false)
            {
                //页面有未编辑的元素，不允许入库
                result.Status = 0;
                result.Message = "页面还有待编辑的元素,请编辑完毕后提交!";
                return Json(result);
            }
            var xconfig = CreateXml("Config", configList);
            var xheader = CreateHeaderXml(headerList);
            var xbody = CreateBodyXml(bodyList);
            var xother = CreateXml("Other", otherList);

            XElement xd = new XElement(actionKey);
            xd.Add(xconfig);
            xd.Add(xheader);
            xd.Add(xbody);
            xd.Add(xother);
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            if (template != null)
            {
                template.definition = xd.ToString();
                template.modifier = "DoAdd" + actionKey + "Template";
                template.modifytime = DateTime.Now;
                ef.SaveChanges();
            }
            else
            {
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = actionKey,
                    creator = actionKey,
                    definition = xd.ToString(),
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            result.Status = 1;
            result.Message = "is success";
            return Json(result);

        }
        #endregion

        #region 修改 label 元素逻辑
        /// <summary>
        ///  查看页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditLabel()
        {
            string guid= DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid=Guid.Parse(guid);
            var result= ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();
            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null:(JObject)JsonConvert.DeserializeObject(result.dataValue);

            if (member != null)
            {
                ViewBag.content = member["Content"].ToString();
                ViewBag.otherContent = member["OtherContent"].ToString();
                ViewBag.isBold = member["IsBold"].ToString();
                ViewBag.color = member["Color"].ToString();
                ViewBag.fontSize = member["FontSize"].ToString();
                ViewBag.isSETime = member["IsSETime"].ToString();
                ViewBag.bgColor = member["BgColor"].ToString();
                ViewBag.opacity = member["Opacity"].ToString();
            }
            else
            {
                ViewBag.content = string.Empty;
                ViewBag.otherContent = string.Empty;
                ViewBag.isBold = "false";
                ViewBag.color ="#000";
                ViewBag.bgColor = "#000";
                ViewBag.opacity = "50";
                ViewBag.fontSize = "48px";
                ViewBag.isSETime = "false";//是否显示数据轴
            }
            ViewBag.result = result;
            return View();
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <returns></returns>
        public JsonResult DoEditLabel()
        {
            string dataid= DNTRequest.GetString("dataid");
            string text = DNTRequest.GetString("text");
            string otherText = DNTRequest.GetString("othertext");
            string fontColor = DNTRequest.GetString("fontcolor");
            string fontSize = DNTRequest.GetString("fontsize");
            string isBold = DNTRequest.GetString("isBold");
            string isSETime = DNTRequest.GetString("isSETime");
            string bgColor = DNTRequest.GetString("bgcolor");
            string opacity = DNTRequest.GetString("opacity");
            var did = Guid.Parse(dataid);
            var entity = new EventItem();

            TemplateLabel label = new TemplateLabel();
            label.ID = did;
            label.Content = text;
            label.OtherContent = otherText;
            label.FontSize = fontSize;
            label.Color = fontColor;
            label.BgColor = bgColor;
            label.Opacity = opacity;
            label.IsBold = bool.Parse(isBold);
            label.IsSETime = bool.Parse(isSETime);
            var json = label.SerializeJSON() ;
            entity.creator = "DoEditLabel";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var  entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status=1,
                Message= "is success",
                Data= entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }

        #endregion

        #region 修改 Image 元素逻辑

        /// <summary>
        ///  查看页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditImage()
        {
            string guid = DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid = Guid.Parse(guid);
            var result = ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();

            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null : (JObject)JsonConvert.DeserializeObject(result.dataValue);
            if (member != null)
            {
                ViewBag.url = member["Url"].ToString();
            }
            else
            {
                ViewBag.url = string.Empty;
            }
            ViewBag.result = result;
            return View();
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <returns></returns>
        public JsonResult DoEditImage()
        {
            string dataid = DNTRequest.GetString("dataid");
            string url = DNTRequest.GetString("url");
            var did = Guid.Parse(dataid);
            TemplateImage image = new TemplateImage();
            image.ID = did;
            image.Url = url;
            var entity = new EventItem();
            var json = image.SerializeJSON();
            entity.creator = "DoEditImage";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }
        #endregion

        #region 修改 Time 元素逻辑
        public ActionResult EditTime()
        {
            string guid = DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid = Guid.Parse(guid);
            var result = ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();
            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null : (JObject)JsonConvert.DeserializeObject(result.dataValue);
            if (member != null)
            {
                
                ViewBag.timeType = member["TimeType"].ToString();
                ViewBag.isBold = member["IsBold"].ToString();
                ViewBag.color = member["Color"].ToString();
                ViewBag.fontSize = member["FontSize"].ToString();
            }
            else
            {
                ViewBag.timeType = "1";
                ViewBag.isBold = "false";
                ViewBag.color = "#000";
                ViewBag.fontSize = "36px";
            }
            ViewBag.result = result;
            ViewBag.timeFormat1 = DateTime.Now.ToString("yyyy年MM月dd日");
            ViewBag.timeFormat2= DateTime.Now.ToString("yyyy/MM/dd");
            return View();
        }
        public JsonResult DoEditTime()
        {
            string dataid = DNTRequest.GetString("dataid");
            string fontcolor = DNTRequest.GetString("fontcolor");
            string fontsize = DNTRequest.GetString("fontsize");
            string isBold = DNTRequest.GetString("isBold");
            string timeType = DNTRequest.GetString("timetype");
            var did = Guid.Parse(dataid);
            var entity = new EventItem();
            TemplateTime time = new TemplateTime();
            time.ID = did;
            time.FontSize = fontsize;
            time.Color = fontcolor;
            time.IsBold = bool.Parse(isBold);
            time.TimeType = timeType.ToInt();
            var json = time.SerializeJSON();
            entity.creator = "DoEditTime";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }
        #endregion

        #region 修改 Message 元素逻辑
        public ActionResult EditMessage()
        {
            string guid = DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid = Guid.Parse(guid);
            var result = ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();
            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null : (JObject)JsonConvert.DeserializeObject(result.dataValue);

            if (member != null)
            {
                ViewBag.content = member["Content"].ToString();
                ViewBag.otherContent = member["OtherContent"].ToString();
                ViewBag.color = member["Color"].ToString();
               
            }
            else
            {
                ViewBag.content = string.Empty;
                ViewBag.otherContent = string.Empty;
                ViewBag.color = "#fff";
            }
            ViewBag.result = result;
            return View();
        }

        public JsonResult DoEditMessage()
        {
            string dataid = DNTRequest.GetString("dataid");
            string text = DNTRequest.GetString("text");
            string otherText = DNTRequest.GetString("othertext");
            string fontColor = DNTRequest.GetString("fontcolor");
            var did = Guid.Parse(dataid);
            var entity = new EventItem();

            TemplateMessage message = new TemplateMessage();
            message.ID = did;
            message.Content = text;
            message.OtherContent = otherText;
            message.Color = fontColor;
            var json = message.SerializeJSON();
            entity.creator = "DoEditMessage";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }
        #endregion

        #region 修改 Video 元素逻辑
        public ActionResult EditVideo()
        {
            string guid = DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid = Guid.Parse(guid);
            var result = ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();
            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null : (JObject)JsonConvert.DeserializeObject(result.dataValue);
            
            if (member != null)
            {
                //视屏地址 数据用 用双竖线 做分割
                ViewBag.urls = member["Urls"].ToString();
              
            }
            else
            {
                ViewBag.urls=string.Empty;
            }
            ViewBag.result = result;
            return View();
        }


        public JsonResult DoEditVideo()
        {
            string dataid = DNTRequest.GetString("dataid");
            string url = DNTRequest.GetString("url");
            var did = Guid.Parse(dataid);
            TemplateVideo video = new TemplateVideo();
            video.ID = did;
            video.Urls = url;
            var entity = new EventItem();
            var json = video.SerializeJSON();
            entity.creator = "DoEditVideo";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }
        #endregion

        #region 修改 BgColor 元素逻辑
        /// <summary>
        /// 大部分在修改
        /// </summary>
        /// <returns></returns>
        public ActionResult EditBgcolor()
        {
            string guid = DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid = Guid.Parse(guid);
            var result = ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();
            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null : (JObject)JsonConvert.DeserializeObject(result.dataValue);

            if (member != null)
            {
             
                ViewBag.bgColor = member["BgColor"].ToString();
                ViewBag.opacity = member["Opacity"].ToString();
                ViewBag.bgImageUrl= member["BgImageUrl"].ToString();
                ViewBag.bgType = member["BgType"].ToString();
            }
            else
            {
                ViewBag.bgColor = "#000";
                ViewBag.opacity = "50";
                ViewBag.bgImageUrl = "/Images/bg-1.jpg";
                ViewBag.bgType =2;
            }
            ViewBag.result = result;
            return View();
        }

        public JsonResult DoEditBgcolor()
        {
            string dataid = DNTRequest.GetString("dataid");
            string bgType = DNTRequest.GetString("bgtype");
            string url = DNTRequest.GetString("url");
            string bgcolor = DNTRequest.GetString("bgcolor");
            string opacity = DNTRequest.GetString("opacity");
            var did = Guid.Parse(dataid);
            var entity = new EventItem();
            TemplateBgColor tc = new TemplateBgColor();
            tc.ID = did;
            tc.BgType=bgType.ToInt();
            tc.BgImageUrl = url;
            tc.BgColor = bgcolor;
            tc.Opacity = opacity;
            var json = tc.SerializeJSON();
            entity.creator = "EditBgcolor";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }

        #endregion

        #region 修改 Content 元素逻辑
        public ActionResult EditContent()
        {
            string guid = DNTRequest.GetString("id");
            string tag = DNTRequest.GetString("tag");
            if (string.IsNullOrWhiteSpace(guid))
            {
                guid = Guid.NewGuid().ToString();
            }
            var dataid = Guid.Parse(guid);
            var result = ef.EventItem.Where(p => p.dataID == dataid).FirstOrDefault();
            if (result == null)
            {
                result = new EventItem();
                result.dataID = dataid;
            }
            var member = string.IsNullOrWhiteSpace(result.dataValue) ? null : (JObject)JsonConvert.DeserializeObject(result.dataValue);
            if (member != null)
            {
                ViewBag.content = member["Content"].ToString();
            }
            else
            {
                ViewBag.content = string.Empty;
            }
            ViewBag.result = result;
            return View();
        }
        [ValidateInput(false)]
        public JsonResult DoEditContent()
        {
            string dataid = DNTRequest.GetString("dataid");
            string htmlStr = DNTRequest.GetString("htmlStr");
            var did = Guid.Parse(dataid);
            var entity = new EventItem();
            TemplateContent tc = new TemplateContent();
            tc.ID = did;
            tc.Type = 1;//默认文字
            tc.Content = htmlStr;
            var json = tc.SerializeJSON();
            entity.creator = "EditContent";
            entity.creationtime = DateTime.Now;
            entity.dataValue = json;
            entity.dataID = did;
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag == null)
            {
                //写入
                ef.EventItem.Add(entity);
            }
            else
            {
                //更新
                entFlag.creator = entity.creator;
                entFlag.creationtime = entity.creationtime;
                entFlag.dataValue = entity.dataValue;
                entFlag.dataID = entity.dataID;
            }
            ef.SaveChanges();
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = entity.dataValue  //此数据带入至父页面
            };
            return Json(result);
        }
        #endregion

        #region 移除元素
        /// <summary>
        /// 移除元素日志表
        /// </summary>
        /// <returns></returns>
        public JsonResult DoDelElement()
        {
            string dataid = DNTRequest.GetString("dataid");
            var did = Guid.Parse(dataid);
            var entFlag = ef.EventItem.Where(p => p.dataID == did).FirstOrDefault();
            if (entFlag != null)
            {
                ef.EventItem.Remove(entFlag);
                ef.SaveChanges();
            }
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = "is success",
                Data = null
            };
            return Json(result);
        }
        #endregion

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <returns></returns>
        public JsonResult Upload()
        {
            ResultDto<string> result = new ResultDto<string>();
            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            string savePath = Server.MapPath("/Upload");
            if (!Directory.Exists(savePath))
            {
                //需要注意的是，需要对这个物理路径有足够的权限，否则会报错 
                Directory.CreateDirectory(savePath);
            }
            if (hfc.Count > 0)
            {
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssf");
                fileName = fileName + hfc[0].FileName.Substring(hfc[0].FileName.LastIndexOf('.'));
                fileName = "/Upload/" + fileName;
                string PhysicalPath = Server.MapPath(fileName);
                hfc[0].SaveAs(PhysicalPath);
                result.Status = 1;
                result.Message = " is success";
                result.Data = fileName;
            }
            else
            {
                result.Status = 0;
                result.Message = " is error ";
                result.Data = null;
            }
            return Json(result);
        }

        /// <summary>
        /// 页面用来获取GUID
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGUID()
        {
            var result = new ResultDto<string>()
            {
                Status = 1,
                Message = " is success",
                Data = System.Guid.NewGuid().ToString()
            };
            return Json(result);
        }

        /// <summary>
        /// 重置用户设置
        /// </summary>
        /// <returns></returns>
        public JsonResult Reset()
        {
            string actionKey = DNTRequest.GetString("ak");
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            if (template != null)
            {
                string description = string.Empty;
                template.definition=TemplateInitEntrance(actionKey,ref description);
                ef.SaveChanges();
            }
            var result =new  ResultDto<string>();
            result.Status = 1;
            result.Message = "重置已完成,正在配置元素....";
            result.Data = "/home/index?ak="+actionKey;
            return Json(result);
        }
    }
}
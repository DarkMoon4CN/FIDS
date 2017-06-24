using CATC.FIDS.Model;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CATC.FIDS.Controllers
{
    public partial class TemplateController
    {
        #region 登机竖版含气象模板
        /// <summary>
        /// 登机竖版含气象模板
        /// </summary>
        /// <returns></returns>
        public ActionResult BoardingGate()
        {
            string actionKey = "BoardingGate";
            //设置页面配置信息
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();

            //获取配置
            string hstr = string.Empty;
            if (template != null && !string.IsNullOrWhiteSpace(template.definition))
            {
               AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            return View();
        }

        #endregion

        #region 登机口竖版无气象模板
        /// <summary>
        /// VNW: Vertical NO Weather
        /// </summary>
        public ActionResult VNWBoardingGate()
        {
            string actionKey = "VNWBoardingGate";
            string gid = DNTRequest.GetString("gid"); //登机口ID
            string iata = DNTRequest.GetString("iata");//航空公司ID
            int gateId = 1;
            if (!string.IsNullOrWhiteSpace(gid))
            {
                gateId = gid.ToInt();
            }
            //获取航空公司logo
            var airEntity = ef.F_Airline.Where(p => p.Airline_IATA == iata).FirstOrDefault();

            //获取登机口默认位置
            //var gateEntity=  wait

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
            ViewBag.logo = airEntity != null ? airEntity.MaxLogo : "/Images/logo.png";
            ViewBag.gateid = gateId > 10 ? gateId.ToString() : "0" + gateId;
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion

        #region 登机口横版无气象模板
        public ActionResult BoardingGateInfo()
        {
            string actionKey = "BoardingGateInfo";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();

            //获取配置
            string hstr = string.Empty;
            if (template != null)
            {
                AnalysisHeaderXml(actionKey, template.definition, out hstr, true);
            }
            ViewBag.hstr = hstr;
            return View();
        }

        #endregion

        #region 登机口横版含气象模板
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ActionResult BoardingGateHoriz()
        {
            string actionKey = "BoardingGateHoriz";
            string gid = DNTRequest.GetString("gid"); //登机口ID
            string iata = DNTRequest.GetString("iata");//航空公司ID
            int gateId = 1;
            if (!string.IsNullOrWhiteSpace(gid))
            {
                gateId = gid.ToInt();
            }
            //获取航空公司logo
            var airEntity = ef.F_Airline.Where(p => p.Airline_IATA == iata).FirstOrDefault();

            //获取登机口默认位置
            //var gateEntity=  wait

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
            ViewBag.logo = airEntity != null ? airEntity.MaxLogo : "/Images/logo.png";
            ViewBag.gateid = gateId > 10 ? gateId.ToString() : "0" + gateId;
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }
        #endregion

        #region 模板保存动作
        public JsonResult DoAddBoardingGateTemplate()
        {
            string actionKey = "BoardingGate";
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
                    description = "竖版登机口(含气象)",
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

        public JsonResult DoAddBoardingGateInfoTemplate()
        {
            string actionKey = "BoardingGateInfo";
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


            //TemplateTable table = new TemplateTable();
            //table.ID = System.Guid.NewGuid();
            //table.Border = 1;
            //table.Class = "mainCon-hd is-hdColor";
            //table.Style = "";
            //List<TemplateTD> tds = new List<TemplateTD>();
            //for (int i = 0; i < bodySplit.Length; i++)
            //{
            //    var p = bodySplit[i];
            //    if (!string.IsNullOrWhiteSpace(p))
            //    {
            //        //里面保存的都是按顺序的table列ID
            //        TemplateTD td = new TemplateTD();
            //        td.Class = "col-2";
            //        td.ID = Guid.NewGuid();
            //        td.FiledID = p.ToInt();
            //        td.Index = i + 1;
            //        td.Style = string.Empty;
            //        td.Tag = td.Index;
            //        td.Remarks = "登机口信息TD";
            //        tds.Add(td);
            //    }
            //}
            //table.TDs = tds;
            //bodyList.Add(table);

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
                    description = "横版登机口(无气象)",
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

        public JsonResult DoAddVNWBoardingGateTemplate()
        {
            string actionKey = "VNWBoardingGate";
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

        public JsonResult DoAddBoardingGateHorizTemplate()
        {
            string actionKey = "BoardingGateHoriz";
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
                    description = "横登机口(含气象)",
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
    }
}
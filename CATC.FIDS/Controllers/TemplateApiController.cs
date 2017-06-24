using CATC.FIDS.Factory;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Data;
using System.Net;
using System.Collections.Specialized;
using CATC.FIDS.Model;
using Newtonsoft.Json.Converters;

namespace CATC.FIDS.Controllers
{

    /// <summary>
    /// 此类用于模板模块对外开放的接口
    /// </summary>
    [ControllerAuth]
    public class TemplateApiController : Controller
    { 
        CATC_FIDS_DBEntities ef = new CATC_FIDS_DBEntities();

        /// <summary>
        /// 获取站点域名
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSiteDomain()
        {
            var result = new ResultDto<string>();
            result.Status = 1;
            result.Message = "is success";
            result.Data = Utils.Settings.Instance.GetSetting("SiteDomain");
            return Json(result,JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetWeatherDomain()
        {
            var result = new ResultDto<string>();
            result.Status = 1;
            result.Message = "is success";
            result.Data = Utils.Settings.Instance.GetSetting("WeatherDomain");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取模板设备配置信息
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetTemplateDisplayList(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<TemplateDisplayInfoView, bool>> where = PredicateExtensionses.True<TemplateDisplayInfoView>();
                string startId = DNTRequest.GetString("startId");
                if (!string.IsNullOrWhiteSpace(startId))
                {
                    int tid = startId.ToInt();
                    where = where.And(p => p.ID > tid);
                }
                string displayID = DNTRequest.GetString("displayID");
                if (!string.IsNullOrWhiteSpace(displayID))
                {
                    int did = displayID.ToInt();
                    where = where.And(p => p.DisplayID == did);
                }
                var list = ef.TemplateDisplayInfoView.Where(where).OrderByDescending(o => o.CreateTime).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.TemplateDisplayInfoView.Where(where).Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new {List=list,Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/GetTemplateDisplayList error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  获取模板列表
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetTemplateList(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<Template, bool>> where = PredicateExtensionses.True<Template>();
                string tid = DNTRequest.GetString("startId");
                if (!string.IsNullOrWhiteSpace(tid))
                {
                    where = where.And(p => p.templateID > tid.ToInt());
                }
                where = where.And(p => p.isValid == 1);
                List<Template> list = ef.Template.Where(where).OrderByDescending(o => o.templateID).Skip(startRow).Take(pageSize).ToList();
                //进行对值机组进行分配
                List<TemplateModels> models = new List<TemplateModels>();

                foreach (var item in list)
                {
                    TemplateModels model = new TemplateModels();
                    model.templateID = item.templateID;
                    model.code = item.code;
                    model.description = item.description;
                    if (item.code == "BoardingGateHoriz")
                    {
                        var gateList = ef.R_Facility.Where(p => p.FacilityType == 1 &&p.Status.Trim()=="N").ToList();
                        List<dynamic> gtList = new List<dynamic>();
                        foreach (var gate in gateList)
                        {
                            var gt = new
                            {
                                Display_Symbol = gate.Display_Symbol,
                                Facility_ID = gate.Facility_ID,
                                Description = gate.Description,
                            };
                            gtList.Add(gt);
                        }
                        model.value = gtList;
                    }
                    if (item.code == "Checkins")
                    {
                        var checkInsList = ef.R_Facility.Where(p => p.FacilityType == 2 && p.Status.Trim() == "N").ToList();
                        List<dynamic> ckList = new List<dynamic>();
                        foreach (var check in checkInsList)
                        {
                            var ck = new
                            {
                                Display_Symbol = check.Display_Symbol,
                                Facility_ID=check.Facility_ID,
                                Description=check.Description,
                            };
                            ckList.Add(ck);
                        }
                        model.value = ckList;
                    }
                   
                    if (item.code == "BaggageClaim")
                    {
                        var baggageclaimList = ef.R_Facility.Where(p => p.FacilityType == 3 && p.Status.Trim() == "N").ToList();
                        List<dynamic> bcList = new List<dynamic>();
                        foreach (var baggageclaim in baggageclaimList)
                        {
                            var bc = new
                            {
                                Display_Symbol = baggageclaim.Display_Symbol,
                                Facility_ID = baggageclaim.Facility_ID,
                                Description = baggageclaim.Description,
                            };
                            bcList.Add(bc);
                        }
                        model.value = bcList;
                    }
                    models.Add(model);
                }
                result.Status = 1;
                result.Message = "is success";
                result.Data = models;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/GetTemplateList error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取显示设备集合
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetDisplayList(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<R_DisplayInfo, bool>> where = PredicateExtensionses.True<R_DisplayInfo>();
                string did = DNTRequest.GetString("startId");
                if (!string.IsNullOrWhiteSpace(did))
                {
                    where = where.And(p => p.displayID > did.ToInt());
                }
                List<R_DisplayInfo> list = ef.R_DisplayInfo.Where(where).OrderByDescending(o => o.displayID).Skip(startRow).Take(pageSize).ToList();
                result.Status = 1;
                result.Message = "is success";
                result.Data = list;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/GetDisplayList error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取单条模板设备配置信息
        /// </summary>
        /// <returns></returns>
        public JsonResult GetTemplateDisplay()
        {
            var result = new ResultDto<TemplateDisplayInfoView>();
            try
            {
                Expression<Func<TemplateDisplayInfoView, bool>> where = PredicateExtensionses.True<TemplateDisplayInfoView>();
                string tdid = DNTRequest.GetString("ID");
  
                if (!string.IsNullOrWhiteSpace(tdid))
                {
                    int id = tdid.ToInt();
                    where = where.And(p => p.ID == id);
                }
                var model = ef.TemplateDisplayInfoView.Where(where).FirstOrDefault();
                result.Status = 1;
                result.Message = "is success";
                result.Data = model;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/GetTemplateDisplay error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 增加模板及设备信息
        /// </summary>
        /// <returns></returns>
        public JsonResult AddTemplateDisplay()
        {
            var result = new ResultDto<dynamic>();
            L_Operation_Log sysLog = new L_Operation_Log();
            sysLog.LogType = (int)OperationLogEnum.接口日志;
            sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
            sysLog.LogTime = DateTime.Now;
            try
            {
                string templateID = DNTRequest.GetString("templateID");
                string displayID = DNTRequest.GetString("displayID");
                string startTime = DNTRequest.GetString("startTime");
                string endTime = DNTRequest.GetString("endTime");
                string weeks = DNTRequest.GetString("weeks");
                string isCover = DNTRequest.GetString("isCover");
                string isAdvert = DNTRequest.GetString("isAdvert");
                string advertUrl = DNTRequest.GetString("advertUrl");
                string sort = DNTRequest.GetString("sort");
                string spaceStartTime = DNTRequest.GetString("spaceStartTime");
                string spaceEndTime = DNTRequest.GetString("spaceEndTime");
                string intervalSecond = DNTRequest.GetString("intervalSecond");
                string index= DNTRequest.GetString("index");
                string pageName = DNTRequest.GetString("pageName");
                string count = DNTRequest.GetString("count");
                string topScreenCode = DNTRequest.GetString("topScreenCode");
                string isSend= DNTRequest.GetString("isSend");
                string dataValue = DNTRequest.GetString("dataValue");

                Template_DisplayInfo_Rel tdInfo = new Template_DisplayInfo_Rel();
                tdInfo.TemplateID = templateID.ToInt();
                tdInfo.DisplayID = displayID.ToInt();
                tdInfo.StartTime = startTime.ToDateTime();
                tdInfo.EndTime = endTime.ToDateTime();
                tdInfo.Weeks = weeks;
                tdInfo.IsCover = isCover.ToShort();
                tdInfo.IsAdvert = isAdvert.ToShort();
                tdInfo.AdvertUrl = advertUrl;
                tdInfo.Sort = sort.ToShort();
                tdInfo.SpaceStartTime = spaceStartTime.ToDateTime();
                tdInfo.SpaceEndTime = spaceEndTime.ToDateTime();
                tdInfo.IntervalSecond = intervalSecond.ToInt();
                tdInfo.PageName = pageName;
                tdInfo.Index = index.ToInt();
                tdInfo.Count = count.ToInt();
                tdInfo.TopScreenCode = topScreenCode.ToInt();
                tdInfo.CreateTime = DateTime.Now;
                tdInfo.IsSend = isSend.ToInt();
                tdInfo.DataValue = dataValue;
                string id = DNTRequest.GetString("ID");
                if (!string.IsNullOrWhiteSpace(id))//更新
                {
                    int tdid = id.ToInt();
                    var model= ef.Template_DisplayInfo_Rel.Where(p => p.ID == tdid).FirstOrDefault();
                    if (model != null)
                    {
                        model.TemplateID = tdInfo.TemplateID;
                        model.DisplayID = tdInfo.DisplayID;
                        model.StartTime = tdInfo.StartTime;
                        model.EndTime = tdInfo.EndTime.AddDays(1).AddSeconds(-1);
                        model.Weeks = tdInfo.Weeks;
                        model.IsCover = tdInfo.IsCover;
                        model.IsAdvert = tdInfo.IsAdvert;
                        model.AdvertUrl = tdInfo.AdvertUrl;
                        model.Sort = tdInfo.Sort;
                        model.SpaceStartTime = tdInfo.SpaceStartTime;
                        model.SpaceEndTime = tdInfo.SpaceEndTime;
                        model.IntervalSecond = tdInfo.IntervalSecond;
                        model.PageName = tdInfo.PageName;
                        model.Index = tdInfo.Index;
                        model.Count = tdInfo.Count;
                        model.TopScreenCode = tdInfo.TopScreenCode;
                        model.IsSend = tdInfo.IsSend;
                        model.DataValue = tdInfo.DataValue;
                        if (model.IsSend > 0)
                        {
                            result = SendCommand(displayID.ToInt(), model);
                            if (result.Status == 0)
                            {
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                sysLog.ModuleType = (int)OperationLogModuleEnum.模板分配发布;
                            }
                        }
                        else
                        {
                            sysLog.ModuleType = (int)OperationLogModuleEnum.模板分配编辑;
                        }
                        sysLog.ModuleValue = id;
                        ef.L_Operation_Log.Add(sysLog);
                        ef.SaveChanges();
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此模板信息！";
                    }
                }
                else
                {
                    if (tdInfo.IsSend > 0)
                    {
                        result = SendCommand(displayID.ToInt(), tdInfo);
                        if (result.Status == 0)
                        {
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            sysLog.ModuleType = (int)OperationLogModuleEnum.模板分配发布;
                        }
                    }
                    else
                    {
                        sysLog.ModuleType = (int)OperationLogModuleEnum.模板分配新增;
                    }

                    ef.Template_DisplayInfo_Rel.Add(tdInfo);
                    ef.SaveChanges();

                    sysLog.ModuleValue = tdInfo.ID.ToString();
                    ef.L_Operation_Log.Add(sysLog);
                    ef.SaveChanges();
                }
                result.Status = 1;
                result.Message = "模板写入成功！";
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/AddTemplateDisplay error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaxIndex()
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<Template_DisplayInfo_Rel, bool>> where = PredicateExtensionses.True<Template_DisplayInfo_Rel>();
                string displayID = DNTRequest.GetString("displayID");
                int did = displayID.ToInt();
                where = where.And(p => p.DisplayID ==did);
                var list = ef.Template_DisplayInfo_Rel.Where(where).ToList();
                int index = 1;
                if (list != null && list.Count > 0)
                {
                   index=list.Max(m=>m.Index)+1;
                }
                result.Status = 1;
                result.Message = "is success";
                result.Data = index;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/GetMaxIndex error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回已被占用的Index 集合字符串 1,2,3,4,5
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUseIndexs()
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<Template_DisplayInfo_Rel, bool>> where = PredicateExtensionses.True<Template_DisplayInfo_Rel>();
                string displayID = DNTRequest.GetString("displayID");
                int did = displayID.ToInt();
                where = where.And(p => p.DisplayID == did);
                var list = ef.Template_DisplayInfo_Rel.Where(where).ToList();
                var indexs = list.Select(s=>s.Index).ToList();
                string indexsStr = string.Join(",",indexs);
                result.Status = 1;
                result.Message = "is success";
                result.Data = indexsStr;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/GetUseIndexs error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = string.Empty;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  删除模板设备配置信息   
        /// </summary>
        /// <returns></returns>
        public JsonResult DelTemplateDisplay()
        {

            var result = new ResultDto<dynamic>();
            try
            {
                string ids = DNTRequest.GetString("ids");
                string[] idSplit = ids.TrimEnd(',').Split(',');
                var tids = idSplit.Select<string, int>(s => Convert.ToInt32(s)).ToList();
                var existTDInfos = ef.Template_DisplayInfo_Rel.Where(p => tids.Contains(p.ID)).ToList();
                if (existTDInfos != null && existTDInfos.Count > 0)
                {
                    //清理
                    ef.Template_DisplayInfo_Rel.RemoveRange(existTDInfos);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "is success";

                    L_Operation_Log sysLog = new L_Operation_Log();
                    sysLog.LogTime = DateTime.Now;
                    sysLog.LogType = (int)OperationLogEnum.接口日志;
                    sysLog.ModuleType = (int)OperationLogModuleEnum.模板分配删除;
                    sysLog.ModuleValue = ids;
                    sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                    ef.L_Operation_Log.Add(sysLog);
                    ef.SaveChanges();
                }
                else
                {
                    result.Status = 0;
                    result.Message = "移除失败,无法查询到模板信息！";
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/TemplateApi/DelTemplateDisplay error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  由接口向航显设备下发指令动作（编辑页面时的单个发送逻辑）
        /// </summary>
        /// <param name="displayID">设备ID</param>
        /// <returns></returns>
        private ResultDto<dynamic> SendCommand(int displayID, Template_DisplayInfo_Rel tdinfo=null)
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            string json = string.Empty;
            string clientIP = string.Empty;
            try
            {
                List<SendTemplateDisplayInfoMdoels> models = new List<SendTemplateDisplayInfoMdoels>();
                //如果是行李提取、值机、登机口、需获取这几张子表的数据
                
                //wait。。。

                //查找设置设备 结束时间大于今天，开始时间小于今天 的模板
                var date=DateTime.Now;
                Expression<Func<Template_DisplayInfo_Rel, bool>> where = PredicateExtensionses.True<Template_DisplayInfo_Rel>();
                where = where.And(p =>p.DisplayID == displayID);
                where = where.And(p=>p.StartTime <= date);
                where = where.And(p =>p.EndTime >= date);

                List<Template_DisplayInfo_Rel> tdList = new List<Template_DisplayInfo_Rel>();
                if (tdinfo != null)
                {
                    where = where.And(p=>p.ID !=tdinfo.ID);
                    tdList.Add(tdinfo);
                }
                tdList.AddRange(ef.Template_DisplayInfo_Rel.Where(where).ToList());
                var tids = tdList.Select(s => s.TemplateID).ToList();
                var tList =ef.Template.Where(p=>tids.Contains(p.templateID)).ToList();
                var displayInfo = ef.R_DisplayInfo.Where(p=>p.displayID==displayID).FirstOrDefault();
                

                foreach (var item in tdList)
                {
                    SendTemplateDisplayInfoMdoels model = new SendTemplateDisplayInfoMdoels();
                    model.ID = item.ID;
                    model.TemplateID = item.TemplateID;
                    model.DisplayID = item.DisplayID;
                    model.StartTime = item.StartTime;
                    model.EndTime = item.EndTime;
                    model.Weeks = item.Weeks;
                    model.IsCover = item.IsCover;
                    model.IsAdvert = item.IsAdvert;
                    model.AdvertUrl = item.AdvertUrl;
                    model.Sort = item.Sort;
                    model.SpaceStartTime = item.SpaceStartTime;
                    model.SpaceEndTime = item.SpaceEndTime;
                    model.IntervalSecond = item.IntervalSecond;
                    model.PageName = item.PageName;
                    model.Index = item.Index;
                    model.Count = item.Count;
                    model.TopScreenCode = item.TopScreenCode;
                    model.TemplateName = tList.Where(p => p.templateID == model.TemplateID).FirstOrDefault().name;
                    model.DisplayName = displayInfo.displayName;
                    var dataValue = string.Empty;
                    if (string.IsNullOrWhiteSpace(item.DataValue))
                    {
                        dataValue = "?ds=" + item.DataValue;//所有带参数的数据全部使用 ds 作为参数
                    }
                    model.Url = Utils.Settings.Instance.GetSetting("SiteDomain")+"/Actual/"+model.TemplateName+dataValue;
                    models.Add(model);
                }

                //向航显设备发送指令动作
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                //系统自带的JavaScriptSerializer 解析对时间解析负值缺少8个时区
                var displayInfoJson = Newtonsoft.Json.JsonConvert.SerializeObject(models, timeFormat);
                ResultDto<string> cmd = new ResultDto<string>();
                cmd.Status = 1;
                cmd.Message = "template";//由客户端指定的字符串
                cmd.Data = displayInfoJson;
                json = new { Json = cmd.SerializeJSON() }.SerializeJSON();
                clientIP = displayInfo.ip;
                string resultStr = RequestHelper.SendHttpRequest("http://" + clientIP + ":8888/Client/ExecuteCommand", "POST", json);
                result.Status = 1;
                result.Message = "更新客户端设备已完成！";
                result.Data = resultStr;
            }
            catch (Exception ex)
            {
                result.Status = 0;
                result.Message = "航显设备无法连接：" + clientIP;
                result.Data = json;
                LoggerHelper.Error("/TemplateApi/SendCommand error :" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 由接口向航显设备下发指令动作（批量根据Template_DisplayInfo_Rel 进行数据发送）
        /// </summary>
        /// <returns></returns>
        public JsonResult SendCommand()
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            List<SendTemplateDisplayInfoMdoels> models = new List<SendTemplateDisplayInfoMdoels>();
            List<int> errorIDs = new List<int>();//发送失败的ID
            string errorMsg = string.Empty;
            string json = string.Empty;
            string clientIP = string.Empty;
            try
            {
                string ids = DNTRequest.GetString("ids");
                string sendType= DNTRequest.GetString("sendType");//发送类型： 0.单独发送，1.追加至发送队列 2.撤销某一组数据
                if (string.IsNullOrWhiteSpace(sendType))
                {
                    sendType ="1";
                }
                int st = sendType.ToInt();
                var date = DateTime.Now;
                string[] idSplit = ids.TrimEnd(',').Split(',');
                var tdids = idSplit.Select<string, int>(s => Convert.ToInt32(s)).ToList();
                Expression<Func<Template_DisplayInfo_Rel, bool>> where = PredicateExtensionses.True<Template_DisplayInfo_Rel>();
                where = where.And(p => tdids.Contains(p.ID));
                where = where.And(p => p.StartTime <= date);
                where = where.And(p => p.EndTime >= date);
                List<Template_DisplayInfo_Rel> tdList = ef.Template_DisplayInfo_Rel.Where(where).ToList();
                var dids = tdList.Select(s => s.DisplayID).ToList();
                var tids = tdList.Select(s => s.TemplateID).ToList();
                var tList = ef.Template.Where(p => tids.Contains(p.templateID)).ToList();
                var dList = ef.R_DisplayInfo.Where(p => dids.Contains(p.displayID)).ToList();
                if (st == 1 || st==2)
                {
                    //查询相关设备下的所有配置
                    //select_Template_DisplayInfo_Rel_List
                    Expression<Func<Template_DisplayInfo_Rel, bool>> sWhere = PredicateExtensionses.True<Template_DisplayInfo_Rel>();
                    sWhere = sWhere.And(p => dids.Contains(p.DisplayID));
                    sWhere = sWhere.And(p => p.IsSend == 1);
                    sWhere = sWhere.And(p => p.StartTime <= date);
                    sWhere = sWhere.And(p => p.EndTime >= date);
                    var selectTDRList = ef.Template_DisplayInfo_Rel.Where(sWhere).ToList();
                    if (st == 1)//增加至队列
                    {
                        foreach (var item in selectTDRList)
                        {

                            if (!tdids.Contains(item.ID))
                            {
                                tdList.Add(item);
                            }
                        }
                    }

                    if (st == 2)//移除队列
                    {
                        for (int j = selectTDRList.Count-1; j>=0; j--)
                        {
                             int id=selectTDRList[j].ID;
                            if (tdids.Contains(id))
                            {
                                selectTDRList.RemoveAt(j);
                            }
                        }
                        tdList = selectTDRList;
                    }
                    dids = tdList.Select(s => s.DisplayID).Distinct().ToList();
                    tids = tdList.Select(s => s.TemplateID).Distinct().ToList();
                    tList = ef.Template.Where(p => tids.Contains(p.templateID)).ToList();
                    dList = ef.R_DisplayInfo.Where(p => dids.Contains(p.displayID)).ToList();
                }
                foreach (var item in tdList)
                {
                    SendTemplateDisplayInfoMdoels model = new SendTemplateDisplayInfoMdoels();
                    model.ID = item.ID;
                    model.TemplateID = item.TemplateID;
                    model.DisplayID = item.DisplayID;
                    model.StartTime = item.StartTime;
                    model.EndTime = item.EndTime;
                    model.Weeks = item.Weeks;
                    model.IsCover = item.IsCover;
                    model.IsAdvert = item.IsAdvert;
                    model.AdvertUrl = item.AdvertUrl;
                    model.Sort = item.Sort;
                    model.SpaceStartTime = item.SpaceStartTime;
                    model.SpaceEndTime = item.SpaceEndTime;
                    model.IntervalSecond = item.IntervalSecond;
                    model.PageName = item.PageName;
                    model.Index = item.Index;
                    model.Count = item.Count;
                    model.TopScreenCode = item.TopScreenCode;

                    var flagTemplate= tList.Where(p => p.templateID == model.TemplateID).FirstOrDefault();
                    var flagDisplay = dList.Where(p => p.displayID == model.DisplayID).FirstOrDefault();
                    if (flagTemplate == null )
                    {
                        result.Status = 0;
                        result.Message = "无法找到模板ID:"+model.TemplateID;
                        result.Data = null;
                        return Json(result, JsonRequestBehavior.AllowGet); ;
                    }
                    if (flagDisplay == null)
                    {
                        result.Status = 0;
                        result.Message = "无法找到设备ID:" + model.TemplateID;
                        result.Data = null;
                        return Json(result, JsonRequestBehavior.AllowGet); ;
                    }
                    model.TemplateName = flagTemplate.name;
                    model.DisplayName = flagDisplay.displayName;
                    var dataValue = string.Empty;//值机柜台与登机口要使用
                    if (!string.IsNullOrWhiteSpace(item.DataValue))
                    {
                        dataValue = "?ds=" + item.DataValue;//所有带参数的数据全部使用 ds 作为参数
                    }
                    model.Url = Utils.Settings.Instance.GetSetting("SiteDomain") + "/Actual/" + model.TemplateName + dataValue;
                    models.Add(model);
                }
                var dict = new Dictionary<int, List<SendTemplateDisplayInfoMdoels>>();
                foreach (var g in models.GroupBy(x => x.DisplayID))
                {
                    dict.Add(g.Key, g.ToList());
                }
                foreach (var item  in dict)
                {
                    //批量向航显设备发送指令动作
                    IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                    timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    //系统自带的JavaScriptSerializer 解析对时间解析负值缺少8个时区
                    var displayInfoJson =Newtonsoft.Json.JsonConvert.SerializeObject(item.Value, timeFormat);
                    ResultDto<string> cmd = new ResultDto<string>();
                    cmd.Status = 1;
                    cmd.Message = "template";//由客户端指定的字符串
                    cmd.Data = displayInfoJson;
                    json = new { Json = cmd.SerializeJSON() }.SerializeJSON();
                    var did = item.Value.FirstOrDefault().DisplayID;
                    clientIP = dList.Where(p => p.displayID == did).FirstOrDefault().ip;
                    try
                    {
                        string resultStr = RequestHelper.SendHttpRequest("http://" + clientIP + ":8888/Client/ExecuteCommand", "POST", json);
                    }
                    catch
                    {
                        //请求失败后加入异常列表
                        errorIDs.Add(did);
                    }
                }
                
                foreach (var item in tdList)
                {
                    if (!errorIDs.Contains(item.DisplayID))
                    {
                        item.IsSend = 1;
                    }  
                }
                var errDisplayList = dList.Where(p => errorIDs.Contains(p.displayID)).ToList();
                if (errDisplayList != null && errDisplayList.Count > 0)
                {
                    errorMsg += "设备:";
                    for (int i = 0; i < errDisplayList.Count; i++)
                    {
                        errorMsg += i + 1 + "." + errDisplayList[i].displayName + errDisplayList[i].ip + ";";
                    }
                    errorMsg += "发送失败！";
                }

                if (st == 2)//撤销发送 全部修改成未发送
                {
                    var revokeList = ef.Template_DisplayInfo_Rel.Where(p => tdids.Contains(p.ID)).ToList();
                    foreach (var item in revokeList)
                    {
                        item.IsSend = 0;
                    }
                }
                ef.SaveChanges();
                result.Status = 1;
                result.Message =string.IsNullOrWhiteSpace(errorMsg)==false?errorMsg:"更新客户端设备已完成！";
                result.Data = errorIDs;
            }
            catch (Exception ex)
            {
                result.Status = 0;
                result.Message = "无法连接服务器！" ;
                result.Data = json;
                LoggerHelper.Error("/TemplateApi/SendCommand error :" + ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
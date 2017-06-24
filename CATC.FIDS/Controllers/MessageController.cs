using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CATC.FIDS.Factory;

namespace CATC.FIDS.Controllers
{
    /// <summary>
    /// 用于全局响应所有页面中的实时消息与数据
    /// </summary>
    public class MessageController : BaseController
    {
        public JsonResult GetEventMessage()
        {
            //根据actionKey来处理所有页面信息
            string actionKey = DNTRequest.GetString("ak");
            DateTime dt = DateTime.Now;

            //查询时间段内的的数据并级别排序后按时间排序
            var em = ef.EventMessage.Where(p => p.strartTime <= dt && p.endTime >= dt && p.actionKeys.Contains(actionKey))
                .OrderByDescending(o => o.level)
                .ThenByDescending(o1 => o1.createTime)
                .FirstOrDefault();
            ResultDto<EventMessage> result = new ResultDto<EventMessage>()
            {
                Status = 1,
                Message = "is success",
                Data = em
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public  JsonResult GetWeatherTextInfo()
        {
            var text = string.Empty;
            try
            {
                string icao = Utils.Settings.Instance.GetSetting("ICAO");
                string url = Utils.Settings.Instance.GetSetting("WeatherWSDomain");
                object[] args = new object[1];
                args.SetValue(icao, 0);//第一个参数
                text = WebServiceHelper.InvokeWebService(url, "WebService_Phone", "GetAirportInfoByCCCC_NewObtaining_json", args).ToString();
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/Message/GetWeatherTextInfo error:" + ex.Message);
            }

            return Json(text, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeatherImageInfo()
        {
            try
            {
                string icao = Utils.Settings.Instance.GetSetting("ICAO");
                string url = Utils.Settings.Instance.GetSetting("WeatherWSUrl");//气象服务地址
                string domain = Utils.Settings.Instance.GetSetting("WeatherWSDomain");//气象服务域名
                object[] args = new object[3];
                args.SetValue("Star_Red1", 0);
                args.SetValue(50, 1);
                args.SetValue(0, 2);//1的数据有很多，0只有一条
                string text = WebServiceHelper.InvokeWebService(url, "WebService_Phone", "GetStarPic_OneOrFive", args).ToString();
                dynamic obj = text.DeserializeJSON<object>();
                if (obj != null)
                {
                    obj[0]["CONTENT"] = domain + obj[0]["CONTENT"];
                }
                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/Message/GetWeatherImageInfo error:" + ex.Message);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取离港所在的页面获取的实时数据 包括值机 登机口以及状态
        /// </summary>
        /// <returns></returns>
        public JsonResult GetDepCheckinsAndGate()
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                //接收FDIDs
                string fdids = DNTRequest.GetString("fdids");
                string[] idSplit = fdids.TrimEnd(',').Split(',');
                var ids = idSplit.Select<string, int>(s => Convert.ToInt32(s)).ToList();
                var fdList = ef.Flight_Dynamic.Where(p => ids.Contains(p.FDID)).ToList();

                //资源表 ->值机
                var flightNos = fdList.Select(p => p.FLIGHT_NO).ToList();
                var resList = ef.Flight_Resource_Allocation.Where(p => flightNos.Contains(p.FLIGHT_NO)).ToList();
                var checkInsList = resList.Where(p => p.FacilityType == 2).ToList();

                //登机口
                var gateList = resList.Where(p => p.FacilityType == 1).ToList();

                //航班状态
                var newStatusList = ef.F_Status.ToList();
                List<Filght_DynamicModels> models = new List<Filght_DynamicModels>();
                foreach (var item in fdList)
                {
                    Filght_DynamicModels model = new Filght_DynamicModels();
                    var status = newStatusList.Where(p => p.Code == item.Status_Code).FirstOrDefault();
                    var checkins = checkInsList.Where(p => p.FLIGHT_NO == item.FLIGHT_NO).FirstOrDefault();
                    var gate = gateList.Where(p => p.FLIGHT_NO == item.FLIGHT_NO).FirstOrDefault();


                    model.FDID = item.FDID;

                    if (status != null)
                    {
                        model.Status_Code = status.Code;
                        model.Status_Color = status.Color;
                        model.Status_CHINESE_NAME = status.CHINESE_NAME;
                        model.Status_ENGLISH_NAME = status.ENGLISH_NAME;
                    }
                    //优先级:实际->预计->计划->无
                    if (checkins != null)
                    {
                        if (checkins.R_Facility != null)//实际分配的值机
                        {
                            model.CheckIns_Display_Symbol = checkins.R_Facility.Display_Symbol;
                        }
                        else if (checkins.R_Facility1 != null)//预计分配的值机
                        {
                            model.CheckIns_Display_Symbol = checkins.R_Facility.Display_Symbol;
                        }
                        else if (checkins.R_Facility2 != null)//计划分配的值机
                        {
                            model.CheckIns_Display_Symbol = checkins.R_Facility.Display_Symbol;
                        }
                    }
                    if (gate != null)
                    {
                        if (gate.R_Facility != null)//实际分配的值机
                        {
                            model.Gate_Display_Symbol = gate.R_Facility.Display_Symbol;
                        }
                        else if (gate.R_Facility1 != null)//预计分配的值机
                        {
                            model.Gate_Display_Symbol = gate.R_Facility.Display_Symbol;
                        }
                        else if (gate.R_Facility2 != null)//计划分配的值机
                        {
                            model.Gate_Display_Symbol = gate.R_Facility.Display_Symbol;
                        }
                    }
                    models.Add(model);
                }
                result.Status = 1;
                result.Message = "is success";
                result.Data = models;
               
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/Message/GetDepCheckinsAndGate error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取离港所在的页面获取的实时数据  行李转盘 以及状态
        /// </summary>
        /// <returns></returns>
        public JsonResult GetArrBaggage()
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                //接收FDIDs
                string fdids = DNTRequest.GetString("fdids");
                string[] idSplit = fdids.TrimEnd(',').Split(',');
                var ids = idSplit.Select<string, int>(s => Convert.ToInt32(s)).ToList();
                var fdList = ef.Flight_Dynamic.Where(p => ids.Contains(p.FDID)).ToList();

                //资源表 -> 行李转盘
                var flightNos = fdList.Select(p => p.FLIGHT_NO).ToList();
                var resList = ef.Flight_Resource_Allocation.Where(p => flightNos.Contains(p.FLIGHT_NO)).ToList();
                var baggageList = resList.Where(p => p.FacilityType == 3).ToList();

                // 航班状态
                var newStatusList = ef.F_Status.ToList();
                List<Filght_DynamicModels> models = new List<Filght_DynamicModels>();
                foreach (var item in fdList)
                {
                    Filght_DynamicModels model = new Filght_DynamicModels();
                    var baggage = baggageList.Where(p => p.FLIGHT_NO == item.FLIGHT_NO).FirstOrDefault();
                    var status = newStatusList.Where(p => p.Code == item.Status_Code).FirstOrDefault();
                    model.FDID = item.FDID;

                    if (status != null)
                    {
                        model.Status_Code = status.Code;
                        model.Status_Color = status.Color;
                        model.Status_CHINESE_NAME = status.CHINESE_NAME;
                        model.Status_ENGLISH_NAME = status.ENGLISH_NAME;
                    }

                    //优先级:实际->预计->计划->无
                    if (baggage != null)
                    {
                        if (baggage.R_Facility != null)//实际分配的值机
                        {
                            model.CheckIns_Display_Symbol = baggage.R_Facility.Display_Symbol;
                        }
                        else if (baggage.R_Facility1 != null)//预计分配的值机
                        {
                            model.CheckIns_Display_Symbol = baggage.R_Facility.Display_Symbol;
                        }
                        else if (baggage.R_Facility2 != null)//计划分配的值机
                        {
                            model.CheckIns_Display_Symbol = baggage.R_Facility.Display_Symbol;
                        }
                    }
                    models.Add(model);
                }
                result.Status = 1;
                result.Message = "is success";
                result.Data = models;

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/Message/GetArrBaggage error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// BoardingGateHoriz
        /// </summary>
        public JsonResult Getdy()
        {
            ResultDto<dynamic> result = new ResultDto<dynamic>();
            try
            {
                //判定时否有新数据

                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/Message/GetArrBaggage error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        } 
    }

}
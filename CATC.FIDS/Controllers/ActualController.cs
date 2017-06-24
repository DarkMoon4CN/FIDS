using CATC.FIDS.Factory;
using CATC.FIDS.Model;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using Webdiyer.WebControls.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace CATC.FIDS.Controllers
{

    /// <summary>
    ///  和 Template 真正用于展示
    /// </summary>
    public partial class ActualController : BaseController
    {
        #region 离港信息展示板
        /// <summary>
        /// 离港信息模板
        /// </summary>
        /// <returns></returns>
        public ActionResult Departureflight(int page = 1, int pagesize = 12)
        {
            //获取头编辑信息
            string actionKey = "DepartureFlight";
            int pageIndex = page / 1;
            string sortString = string.Empty;
            List<TemplateTD> tds = null;
            string cstr = string.Empty;//背景色配置
            string hstr = string.Empty;//头部所有元素Html
            string bstr = string.Empty;//主体元素Html
            string maxTime = "00:00";
            var minTime = "00:00";
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            
            //真实离港数据
            var newList=ef.Flight_Dynamic.Where(p=>p.AORD=="D").OrderBy(p => p.STD).ToPagedList(pageIndex, pagesize);

            //所有主键id以 1,2,3,4,5 方式
            var fdids = string.Join(",",newList.Select(s => s.FDID).ToList());
            //航空公司
            var airLine_IATAs = newList.Select(s=>s.AIRLINE_IATA).ToList();
            var airLineList = ef.F_Airline.Where(p => airLine_IATAs.Contains(p.Airline_IATA)).ToList();
            //任务代码
            var task_Codes = newList.Select(s=>s.TASK_CODE).ToList();
            var taskCodeList = ef.F_TaskCode.Where(p => task_Codes.Contains(p.Task_Code)).ToList();

            //资源表 ->值机
            var flightNos = newList.Select(p => p.FLIGHT_NO).ToList();
            var resList = ef.Flight_Resource_Allocation.Where(p => flightNos.Contains(p.FLIGHT_NO)).ToList();
            var checkInsList = resList.Where(p=>p.FacilityType==2).ToList();

            //登机口
            var gateList = resList.Where(p=> p.FacilityType==1).ToList();

            //航班状态
            var newStatusList = ef.F_Status.ToList();

            //Airport 经停城市 进行缓存
            var apCache= HttpRuntime.Cache["Airport"];
            List<F_Airport> airportList = null;
            if (apCache != null)
            {
                airportList = apCache as List<F_Airport>;
            }
            else
            {
                airportList = ef.F_Airport.ToList();
                HttpRuntime.Cache.Add("Airport", airportList, null 
                                      , DateTime.Now.AddMinutes(120)
                                      , System.Web.Caching.Cache.NoSlidingExpiration
                                      , System.Web.Caching.CacheItemPriority.High, null);
            }

            AnalysisConfigXml(actionKey, template.definition, out cstr);
            AnalysisHeaderXml(actionKey, template.definition, out hstr);
            var statusList = ef.Status.ToList();
            if (newList != null && newList.Count != 0)
            {
                //获取分页内最大时间及最小时间
                minTime = Utils.Utils.GetTimeHM(newList.FirstOrDefault().STD.Value);
                maxTime = Utils.Utils.GetTimeHM(newList.LastOrDefault().STD.Value);
            }
            hstr = hstr.Replace("{#stime}", minTime);//方法内的参数过滤
            hstr = hstr.Replace("{#etime}", maxTime);
           
            var fileds = GetDepColumns();

            string tableStr = string.Empty;
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStr, out tds);

            ViewBag.totalPageCount = newList.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.sf = tds;
            ViewBag.airportList = airportList;
            ViewBag.maxTime = maxTime;
            ViewBag.minTime = minTime;
            ViewBag.hstr = hstr;
            ViewBag.tableStr = tableStr;
            ViewBag.statusList = statusList;
            ViewBag.cstr = cstr;
            ViewBag.airLineList = airLineList;
            ViewBag.taskCodeList = taskCodeList;
            ViewBag.checkInsList = checkInsList;
            ViewBag.newStatusList = newStatusList;
            ViewBag.gateList = gateList;
            ViewBag.fdids = fdids;
            
            if (Request.IsAjaxRequest())
            {
                return PartialView("_DepartureFlightList", newList);
            }
            return View(newList);
        }
        #endregion

        #region 到港信息展示板
        /// <summary>
        /// 到港信息展示板
        /// </summary>
        /// <returns></returns>
        public ActionResult Arrivalflight(int page = 1, int pagesize = 12)
        {
            //获取头编辑信息
            string actionKey = "ArrivalFlight";
            int pageIndex = page / 1;
            string sortString = string.Empty;
            List<TemplateTD> tds = null;
            string cstr = string.Empty;//背景色配置
            string hstr = string.Empty;//头部所有元素Html
            string bstr = string.Empty;//主体元素Html
            string maxTime = "00:00";
            var minTime = "00:00";
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();

            //真实到港数据
            var newList = ef.Flight_Dynamic.Where(p => p.AORD == "A").OrderBy(p => p.STA).ToPagedList(pageIndex, pagesize);


            //所有主键id以 1,2,3,4,5 方式
            var fdids = string.Join(",", newList.Select(s => s.FDID).ToList());

            //航空公司
            var airLine_IATAs = newList.Select(s => s.AIRLINE_IATA).ToList();
            var airLineList = ef.F_Airline.Where(p => airLine_IATAs.Contains(p.Airline_IATA)).ToList();
            //任务代码
            var task_Codes = newList.Select(s => s.TASK_CODE).ToList();
            var taskCodeList = ef.F_TaskCode.Where(p => task_Codes.Contains(p.Task_Code)).ToList();
            //资源表 -> 行李提取
            var flightNos = newList.Select(p => p.FLIGHT_NO).ToList();
            var resList = ef.Flight_Resource_Allocation.Where(p => flightNos.Contains(p.FLIGHT_NO)).ToList();
            var baggageList = resList.Where(p => p.FacilityType == 3).ToList();

            //飞机状态
            var newStatusList = ef.F_Status.ToList();


            //Airport 经停城市 进行缓存
            var apCache = HttpRuntime.Cache["Airport"];
            List<F_Airport> airportList = null;
            if (apCache != null)
            {
                airportList = apCache as List<F_Airport>;
            }
            else
            {
                airportList = ef.F_Airport.ToList();
                HttpRuntime.Cache.Add("Airport", airportList, null
                                      , DateTime.Now.AddMinutes(120)
                                      , System.Web.Caching.Cache.NoSlidingExpiration
                                      , System.Web.Caching.CacheItemPriority.High, null);
            }

            //查询出到港信息
            AnalysisConfigXml(actionKey, template.definition, out cstr);
            AnalysisHeaderXml(actionKey, template.definition, out hstr);
            var statusList = ef.Status.ToList();
            if (newList != null && newList.Count != 0)
            {
                //获取分页内最大时间及最小时间
                minTime = Utils.Utils.GetTimeHM(newList.FirstOrDefault().STA.Value);
                maxTime = Utils.Utils.GetTimeHM(newList.LastOrDefault().STA.Value);
            }
            hstr = hstr.Replace("{#stime}", minTime);
            hstr = hstr.Replace("{#etime}", maxTime);

            var fileds = GetArrColumns();
            string tableStr = string.Empty;
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStr, out tds);

            ViewBag.totalPageCount = newList.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.airportList = airportList;
            ViewBag.sf = tds;
            ViewBag.maxTime = maxTime;
            ViewBag.minTime = minTime;
            ViewBag.hstr = hstr;
            ViewBag.tableStr = tableStr;
            ViewBag.statusList = statusList;
            ViewBag.cstr = cstr;
            ViewBag.airLineList = airLineList;
            ViewBag.taskCodeList = taskCodeList;
            ViewBag.baggageList = baggageList;
            ViewBag.newStatusList = newStatusList;
            ViewBag.fdids = fdids;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_ArrivalFlightList", newList);
            }
            return View(newList);
        }
        #endregion

        #region 行李提取展示板
        public ActionResult BaggageClaim(int page = 1, int pagesize = 10)
        {
            //获取头编辑信息
            string actionKey = "BaggageClaim";
            int pageIndex = page / 1;
            string sortString = string.Empty;
            List<TemplateTD> tds = null;
            string cstr = string.Empty;//背景色配置
            string hstr = string.Empty;//头部所有元素Html
            string bstr = string.Empty;//主体元素Html
            string maxTime = "00:00";
            var mintime = "00:00";
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            //查询出离港信息
            var list = ef.DailySchedule.Where(p => p.flightKey.ToLower().Contains("dep_")).OrderByDescending(p => p.scheduleTime).ToPagedList(pageIndex, pagesize);
            AnalysisConfigXml(actionKey, template.definition, out cstr);
            AnalysisHeaderXml(actionKey, template.definition, out hstr);
            var statusList = ef.Status.ToList();
            if (list != null && list.Count != 0)
            {
                //获取分页内最大时间及最小时间
                maxTime = Utils.Utils.GetTimeHM(list.OrderBy(o => o.actualTime).FirstOrDefault().actualTime);
                mintime = Utils.Utils.GetTimeHM(list.OrderBy(o => o.actualTime).LastOrDefault().actualTime);
            }
            hstr = hstr.Replace("{#stime}", mintime);
            hstr = hstr.Replace("{#etime}", maxTime);

            var fileds = GetBaggageClaimColumns();
            string tableStr = string.Empty;
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStr, out tds);
            ViewBag.sf = tds;
            ViewBag.maxTime = maxTime;
            ViewBag.minTime = mintime;
            ViewBag.hstr = hstr;
            ViewBag.tableStr = tableStr;
            ViewBag.statusList = statusList;
            ViewBag.cstr = cstr;
            return View(list);
        }
        #endregion

        #region  值机柜台
        public ActionResult Checkins(int page = 1, int pagesize = 10)
        {
            //获取头编辑信息
            string actionKey = "Checkins";
            int pageIndex = page / 1;
            string sortString = string.Empty;
            List<TemplateTD> tds = null;
            string cstr = string.Empty;//背景色配置
            string hstr = string.Empty;//头部所有元素Html
            string bstr = string.Empty;//主体元素Html
            string maxTime = "00:00";
            var mintime = "00:00";
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            //查询出离港信息
            var list = ef.DailySchedule.Where(p => p.flightKey.ToLower().Contains("dep_")).OrderByDescending(p => p.scheduleTime).ToPagedList(pageIndex, pagesize);
            AnalysisConfigXml(actionKey, template.definition, out cstr);
            AnalysisHeaderXml(actionKey, template.definition, out hstr);
            var statusList = ef.Status.ToList();
            if (list != null && list.Count != 0)
            {
                //获取分页内最大时间及最小时间
                maxTime = Utils.Utils.GetTimeHM(list.OrderBy(o => o.actualTime).FirstOrDefault().actualTime);
                mintime = Utils.Utils.GetTimeHM(list.OrderBy(o => o.actualTime).LastOrDefault().actualTime);
            }
            hstr = hstr.Replace("{#stime}", mintime);
            hstr = hstr.Replace("{#etime}", maxTime);

            var fileds = GetBaggageClaimColumns();
            string tableStr = string.Empty;
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStr, out tds);
            ViewBag.sf = tds;
            ViewBag.maxTime = maxTime;
            ViewBag.minTime = mintime;
            ViewBag.hstr = hstr;
            ViewBag.tableStr = tableStr;
            ViewBag.statusList = statusList;
            ViewBag.cstr = cstr;
            return View(list);
        }
        #endregion

        #region 离港大屏信息展示板 
        public ActionResult BSDepartureAndDeparture(int page = 1, int pagesize =14)
        {
            //获取头编辑信息

            //根据传递的参数获取需要配置的数据 
            string actionKey = "DepartureFlight"; //DNTRequest.GetString("actionKey");
            //双屏数据需把pagesize * 2;
            var doublePageSize = pagesize * 2;
            
            int pageIndex = page / 1;
            string sortString = string.Empty;
            List<TemplateTD> tds = null;
            string cstr = string.Empty;//背景色配置
            string hstr = string.Empty;//头部所有元素Html
            string bstr = string.Empty;//主体元素Html
            string maxTime = "00:00";
            var minTime = "00:00";
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();

            //真实离港数据
            var newList = ef.Flight_Dynamic.Where(p => p.AORD == "D").OrderBy(p => p.STD).ToPagedList(pageIndex, doublePageSize);

            //所有主键id以 1,2,3,4,5 方式
            var fdids = string.Join(",", newList.Select(s => s.FDID).ToList());
            //航空公司
            var airLine_IATAs = newList.Select(s => s.AIRLINE_IATA).ToList();
            var airLineList = ef.F_Airline.Where(p => airLine_IATAs.Contains(p.Airline_IATA)).ToList();
            //任务代码
            var task_Codes = newList.Select(s => s.TASK_CODE).ToList();
            var taskCodeList = ef.F_TaskCode.Where(p => task_Codes.Contains(p.Task_Code)).ToList();

            //资源表 ->值机
            var flightNos = newList.Select(p => p.FLIGHT_NO).ToList();
            var resList = ef.Flight_Resource_Allocation.Where(p => flightNos.Contains(p.FLIGHT_NO)).ToList();
            var checkInsList = resList.Where(p => p.FacilityType == 2).ToList();

            //登机口
            var gateList = resList.Where(p => p.FacilityType == 1).ToList();

            //航班状态
            var newStatusList = ef.F_Status.ToList();

            //Airport 经停城市 进行缓存
            var apCache = HttpRuntime.Cache["Airport"];
            List<F_Airport> airportList = null;
            if (apCache != null)
            {
                airportList = apCache as List<F_Airport>;
            }
            else
            {
                airportList = ef.F_Airport.ToList();
                HttpRuntime.Cache.Add("Airport", airportList, null
                                      , DateTime.Now.AddMinutes(120)
                                      , System.Web.Caching.Cache.NoSlidingExpiration
                                      , System.Web.Caching.CacheItemPriority.High, null);
            }

            AnalysisConfigXml(actionKey, template.definition, out cstr);
            AnalysisHeaderXml(actionKey, template.definition, out hstr);
            var statusList = ef.Status.ToList();
            if (newList != null && newList.Count != 0)
            {
                //获取分页内最大时间及最小时间
                minTime = Utils.Utils.GetTimeHM(newList.FirstOrDefault().STD.Value);
                maxTime = Utils.Utils.GetTimeHM(newList.LastOrDefault().STD.Value);
            }
            hstr = hstr.Replace("{#stime}", minTime);//方法内的参数过滤
            hstr = hstr.Replace("{#etime}", maxTime);

            var fileds = GetDepColumns();

            string tableStrA = string.Empty;
            string tableStrB = string.Empty;
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStrA, out tds,1);
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStrB, out tds, 1);

            ViewBag.totalPageCount = newList.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pagesize;
            ViewBag.sf = tds;
            ViewBag.airportList = airportList;
            ViewBag.maxTime = maxTime;
            ViewBag.minTime = minTime;
            ViewBag.hstr = hstr;
            ViewBag.tableStrA = tableStrA;
            ViewBag.tableStrB = tableStrB;
            ViewBag.statusList = statusList;
            ViewBag.cstr = cstr;
            ViewBag.airLineList = airLineList;
            ViewBag.taskCodeList = taskCodeList;
            ViewBag.checkInsList = checkInsList;
            ViewBag.newStatusList = newStatusList;
            ViewBag.gateList = gateList;
            ViewBag.fdids = fdids;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BSDepartureAndDepartureList", newList);
            }
            return View(newList);
        }
        #endregion

        #region 进港大屏信息展示板 
        #endregion

        #region  离港到港大屏相关 --废弃
        public ActionResult BSDepartureAndArrival()
        {
            return View();
        }
        #endregion

        #region 登机口横版无气象展示板
        public ActionResult BoardingGateInfo()
        {
            string actionKey = "BoardingGateInfo";
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();

            //获取配置
            string cstr = string.Empty;//背景色配置
            string hstr = string.Empty;
            if (template != null)
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr, false);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }
        #endregion

        #region 登机口指引展示板
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
            //获取登机引导数据



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
                AnalysisOtherXml(actionKey, template.definition, out ostr);
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr);
                AnalysisBodyXml(actionKey, template.definition, out bstr);
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
                AnalysisOtherXml(actionKey, template.definition, out ostr);
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr);
                AnalysisBodyXml(actionKey, template.definition, out bstr);
            }
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            ViewBag.ostr = ostr;
            ViewBag.bstr = bstr;
            return View();
        }
        #endregion

        #region 登机口竖版无气象展示板
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

            //设置页面配置信息
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            //获取配置
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null && !string.IsNullOrWhiteSpace(template.definition))
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr);
            }
            ViewBag.logo = airEntity != null ? airEntity.MaxLogo : "/Images/logo.png";
            ViewBag.gateid = gateId > 10 ? gateId.ToString() : "0" + gateId;
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }
        #endregion

        #region 登机口横版含气象展示板
        public ActionResult BoardingGateHoriz()
        {
            string actionKey = "BoardingGateHoriz";
            //Display_Symbol
            string displaySymbol = DNTRequest.GetString("ds"); //登机口
            if (string.IsNullOrWhiteSpace(displaySymbol))
            {
                //如何是空的，将获取一个默认的值机柜台数据
                var flagGate=ef.R_Facility.Where(p=>p.FacilityType == 1).FirstOrDefault();
                displaySymbol = flagGate.Display_Symbol.Trim();
            }
            var gate = ef.R_Facility.Where(p => p.Display_Symbol.Trim() == displaySymbol.Trim() && p.FacilityType == 1).FirstOrDefault();
            var date = DateTime.Now;
            var resource =
                ef.Flight_Resource_Allocation.Where(p => p.FacilityType == 1 
                                    && (p.SCHED_ID == gate.Facility_ID || p.ESTIMATE_ID == gate.Facility_ID || p.ACTUAL_ID == gate.Facility_ID)).ToList();

            //2017-04-10 增加资源表选取航班逻辑

            var res_ActualTimeList = resource.Where(p =>p.ACTUAL_START !=null && p.ACTUAL_END !=null && p.ACTUAL_START.Value <= date && p.ACTUAL_END.Value >= date).ToList();
            var res_EstimateTimeList = resource.Where(p => p.ESTIMATE_START != null && p.ESTIMATE_END != null && p.ESTIMATE_START.Value <= date && p.ESTIMATE_END.Value >= date).ToList();
            var res_SchedTimeList = resource.Where(p => p.SCHED_START.Value <= date && p.SCHED_END.Value >= date).ToList();
            var nowFlightNo = string.Empty;
            if (res_ActualTimeList != null && res_ActualTimeList.Count > 0)
            {
                nowFlightNo = res_ActualTimeList.FirstOrDefault().FLIGHT_NO;
            }
            else if (res_EstimateTimeList != null && res_EstimateTimeList.Count > 0)
            {
                nowFlightNo = res_EstimateTimeList.FirstOrDefault().FLIGHT_NO;
            }
            else if (res_SchedTimeList != null && res_SchedTimeList.Count > 0)
            {
                nowFlightNo = res_SchedTimeList.FirstOrDefault().FLIGHT_NO;
            }

            //航班信息
            var where = PredicateExtensionses.True<Flight_Dynamic>();
            where = where.And(p =>p.FLIGHT_NO== nowFlightNo && p.AORD == "D");
            var nowFlightList = ef.Flight_Dynamic.Where(where).ToList();

            //航空公司
            var airLine_IATAs = nowFlightList.Select(s => s.AIRLINE_IATA).ToList();
            var airLineEntity = ef.F_Airline.Where(p => airLine_IATAs.Contains(p.Airline_IATA)).FirstOrDefault();

            //在此登机口并且比当前航班的起飞时间大
            //下一航班的资源数据
            Flight_Resource_Allocation nextResource = null;
            if (res_SchedTimeList != null && res_SchedTimeList.Count > 0)
            {
                var nextFlightDatetime = res_SchedTimeList.FirstOrDefault().SCHED_END;
                var nextWhere = PredicateExtensionses.True<Flight_Resource_Allocation>();
                nextWhere = nextWhere.And(p => p.FacilityType == 1);
                nextWhere = nextWhere.And(p => (p.SCHED_ID == gate.Facility_ID || p.ESTIMATE_ID == gate.Facility_ID || p.ACTUAL_ID == gate.Facility_ID));
                nextWhere = nextWhere.And(p => p.SCHED_START.Value > nextFlightDatetime);
                nextResource = ef.Flight_Resource_Allocation.Where(nextWhere).FirstOrDefault();
            }

            var nextFlight = new Flight_Dynamic();
            if (nextResource != null)
            {
                //下一航班的动态数据
                 nextFlight = ef.Flight_Dynamic.Where(p => p.FLIGHT_NO == nextResource.FLIGHT_NO).FirstOrDefault();
            }
            var opwModel = new ResultDto<SASP>();
            var dawModel = new ResultDto<SASP>();
            string cloudImage = null;
            try
            {
                //卫星云图
                //cloudChartMethod
                var serverTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var weatherUrl = Utils.Settings.Instance.GetSetting("WeatherDomain");
                var icao = Utils.Settings.Instance.GetSetting("ICAO");
                string ccMethod = "/WeatherApi/GetSTART";
                var urlParms = "?icao=" + icao + "&serverTime=" + serverTime;
                string ccStr = RequestHelper.SendHttpRequest(weatherUrl + ccMethod + urlParms, "POST", string.Empty);
                var ccModel = ccStr.DeserializeJSON<ResultDto<string>>();
                if (!string.IsNullOrWhiteSpace(ccModel.Data))
                {
                    cloudImage = weatherUrl + ccModel.Data;
                }

                if (nowFlightList != null && nowFlightList.Count > 0)
                {
                    //起飞机场 天气数据 
                    var method = "/WeatherApi/GetSASP";
                    //Origin AirPort Weather 起飞机场天机
                    string opw = RequestHelper.SendHttpRequest(weatherUrl + method + urlParms, "POST", string.Empty);
                    opwModel = opw.DeserializeJSON<ResultDto<SASP>>();
                    
                    //落地机场 天气数据
                    string destAirProtIATA = nowFlightList.FirstOrDefault().DEST_AIRPORT_IATA;
                    var airport = ef.F_Airport.Where(p => p.AIRPORT_IATA == destAirProtIATA.Trim()).FirstOrDefault();
                    urlParms = "?icao=" + airport.AIRPORT_ICAO + "&serverTime=" + serverTime;
                    string daw = RequestHelper.SendHttpRequest(weatherUrl + method + urlParms, "POST", string.Empty);
                    dawModel = daw.DeserializeJSON<ResultDto<SASP>>();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Info("请求气象API失败："+ex.Message);
            }

            var apCache = HttpRuntime.Cache["Airport"];
            List<F_Airport> airportList = null;
            if (apCache != null)
            {
                airportList = apCache as List<F_Airport>;
            }
            else
            {
                airportList = ef.F_Airport.ToList();
                HttpRuntime.Cache.Add("Airport", airportList, null
                                      , DateTime.Now.AddMinutes(120)
                                      , System.Web.Caching.Cache.NoSlidingExpiration
                                      , System.Web.Caching.CacheItemPriority.High, null);
            }

            //设置页面配置信息
            var template = ef.Template.Where(p => p.name == actionKey).FirstOrDefault();
            //获取配置
            string hstr = string.Empty;
            string cstr = string.Empty;
            if (template != null && !string.IsNullOrWhiteSpace(template.definition))
            {
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr);
            }
            ViewBag.logo = airLineEntity != null && !string.IsNullOrWhiteSpace(airLineEntity.MaxLogo) ? airLineEntity.MaxLogo : "/Images/logo.png";
            ViewBag.displaySymbol = displaySymbol;
            ViewBag.fd = nowFlightList.FirstOrDefault();
            ViewBag.airportList = airportList;
            ViewBag.nextFlight = nextFlight;
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            ViewBag.cloudImage = cloudImage;//卫星云图
            ViewBag.opwModel = opwModel;//始发城市天气
            ViewBag.dawModel = dawModel;//到达城市天气
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BoardingGateHorizData");
            }
            return View();
        }
        #endregion

        #region 特殊服务页面
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
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr);
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
                AnalysisConfigXml(actionKey, template.definition, out cstr);
                AnalysisHeaderXml(actionKey, template.definition, out hstr);
            }
            //获取航空公司logo
            ViewBag.logo = airEntity != null ? airEntity.MaxLogo : "/Images/logo.png";
            ViewBag.hstr = hstr;
            ViewBag.cstr = cstr;
            return View();
        }

        #endregion
    }
}
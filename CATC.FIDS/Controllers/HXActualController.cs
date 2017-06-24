using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Webdiyer.WebControls.Mvc;
using CATC.FIDS.Factory;
using CATC.FIDS.Model;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;

namespace CATC.FIDS.Controllers
{
    public partial class ActualController
    {
        /// <summary>
        /// 进港大屏信息展示板
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public ActionResult BSArrivalAndArrival(int page = 1, int pagesize = 14)
        {
            //获取头编辑信息
            string actionKey = "ArrivalFlight";

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

            string tableStrA = string.Empty;
            string tableStrB = string.Empty;
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStrA, out tds, 1);
            AnalysisTableXml(actionKey, template.definition, fileds, out tableStrB, out tds, 1);

            ViewBag.totalPageCount = newList.TotalPageCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pagesize;
            ViewBag.airportList = airportList;
            ViewBag.sf = tds;
            ViewBag.maxTime = maxTime;
            ViewBag.minTime = minTime;
            ViewBag.hstr = hstr;
            ViewBag.tableStrA = tableStrA;
            ViewBag.tableStrB = tableStrB;
            ViewBag.statusList = statusList;
            ViewBag.cstr = cstr;
            ViewBag.airLineList = airLineList;
            ViewBag.taskCodeList = taskCodeList;
            ViewBag.baggageList = baggageList;
            ViewBag.newStatusList = newStatusList;
            ViewBag.fdids = fdids;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_BSArrivalAndArrivalList", newList);
            }
            return View(newList);
        }

    }
}
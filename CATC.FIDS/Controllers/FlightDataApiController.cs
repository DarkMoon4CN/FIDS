using CATC.FIDS.Factory;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CATC.FIDS.Model;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace CATC.FIDS.Controllers
{
    [ControllerAuth]
    public class FlightDataApiController : BaseController
    {

        #region 航班信息
        /// <summary>
        /// 航班数据接口
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetFlightDataList(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<Flight_Dynamic, bool>> where = PredicateExtensionses.True<Flight_Dynamic>();
                string tid = DNTRequest.GetString("startId");//下标ID
                string aord = DNTRequest.GetString("aord");//A到港 D离港
                string planType = DNTRequest.GetString("planType");//0次日计划 1今日计划 
                string keywrod = DNTRequest.GetString("keyword");//搜索
                if (!string.IsNullOrWhiteSpace(tid))
                {
                    where = where.And(p => p.ID > tid.ToInt());
                }
                if (!string.IsNullOrWhiteSpace(aord) && (aord.ToUpper() == "A" || aord.ToUpper() == "D"))
                {
                    where = where.And(p => p.AORD == aord);
                    if (!string.IsNullOrWhiteSpace(planType))
                    {
                        var dstr = DateTime.Now.ToShortDateString();
                        if (planType == "0")//次日计划的航班
                        {
                            var stime = dstr.ToDateTime().AddDays(1);
                            var etime = stime.AddDays(1).AddSeconds(-1);
                            if (aord.ToUpper() == "A")//到港
                            {
                                where = where.And(p => p.STA >= stime && p.STA <= etime);
                            }
                            else //离港
                            {
                                where = where.And(p => p.STD >= stime && p.STD <= etime);
                            }
                        }
                        else if (planType == "1")//当日计划
                        {
                            var stime = dstr.ToDateTime();
                            var etime = stime.AddDays(1).AddSeconds(-1);
                            if (aord.ToUpper() == "A")//到港
                            {
                                where = where.And(p => p.STA >= stime && p.STA <= etime);
                            }
                            else //离港
                            {
                                where = where.And(p => p.STD >= stime && p.STD <= etime);
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(keywrod))
                {
                    //过滤关键字
                    var cKeyword = Utils.Utils.CheckSQLHtml(keywrod);
                    bool isEqual = cKeyword == keywrod ? true : false;
                    where = where.And(p => cKeyword.Contains(p.FLIGHT_NO));
                }
                var list = ef.Flight_Dynamic.Where(where).OrderByDescending(o => o.ID).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.Flight_Dynamic.Where(where).Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/FlightDataList error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增航班数据
        /// </summary>
        /// <returns></returns>
        public JsonResult AddFlightData()
        {
            var result = new ResultDto<dynamic>();
            string FDID = DNTRequest.GetString("FDID");
            string FLG_IDU = DNTRequest.GetString("FLG_IDU");//航班标识
            string OPERATION_DATE = DNTRequest.GetString("OPERATION_DATE");//运营日
            string FLIGHT_NO = DNTRequest.GetString("FLIGHT_NO");//航班号
            string AORD = DNTRequest.GetString("AORD");//到离港标识
            string DORI = DNTRequest.GetString("DORI");//国际/国内标识
            string TASK_CODE = DNTRequest.GetString("TASK_CODE");//任务代码
            string TERMINAL_NO = DNTRequest.GetString("TERMINAL_NO");//航站楼 
            string AIRLINE_IATA = DNTRequest.GetString("AIRLINE_IATA");//航空公司IATA代码
            string AIRCRAFT_TYPE_IATA = DNTRequest.GetString("AIRCRAFT_TYPE_IATA");//机型IATA代码
            string AC_REG_NO = DNTRequest.GetString("AC_REG_NO");//机号 
            string SERVICE_CLASS = DNTRequest.GetString("SERVICE_CLASS");//服务类型 
            string FLG_VIP = DNTRequest.GetString("SERVICE_CLASS");//要客标识 
            string ORIGIN_AIRPORT_IATA = DNTRequest.GetString("ORIGIN_AIRPORT_IATA");//起飞机场IAIA代码
            string STD = DNTRequest.GetString("STD");
            string ETD = DNTRequest.GetString("ETD");
            string ATD = DNTRequest.GetString("ATD");
            string DEST_AIRPORT_IATA = DNTRequest.GetString("DEST_AIRPORT_IATA");
            string STA = DNTRequest.GetString("STA");
            string ETA = DNTRequest.GetString("ETA");
            string ATA = DNTRequest.GetString("ATA");
            string PREVIOUS_FLIGHT = DNTRequest.GetString("PREVIOUS_FLIGHT");
            string NEXT_FLIGHT = DNTRequest.GetString("NEXT_FLIGHT");
            string ABNORMAL_STATUS = DNTRequest.GetString("ABNORMAL_STATUS");
            string AIRPORT1 = DNTRequest.GetString("AIRPORT1");
            string DORI1 = DNTRequest.GetString("DORI1");
            string STD1 = DNTRequest.GetString("STD1");
            string AIRPORT2 = DNTRequest.GetString("AIRPORT2");
            string DORI2 = DNTRequest.GetString("DORI2");
            string STA2 = DNTRequest.GetString("STA2");
            string STD2 = DNTRequest.GetString("STD2");
            string AIRPORT3 = DNTRequest.GetString("AIRPORT3");
            string DORI3 = DNTRequest.GetString("DORI3");
            string STA3 = DNTRequest.GetString("STA3");
            string STD3 = DNTRequest.GetString("STD3");
            string AIRPORT4 = DNTRequest.GetString("AIRPORT4");
            string DORI4 = DNTRequest.GetString("DORI4");
            string STA4 = DNTRequest.GetString("STA4");
            string CODE_SHARE1 = DNTRequest.GetString("CODE_SHARE1");
            string CODE_SHARE2 = DNTRequest.GetString("CODE_SHARE2");
            string CODE_SHARE3 = DNTRequest.GetString("CODE_SHARE3");
            string CODE_SHARE4 = DNTRequest.GetString("CODE_SHARE4");


            try
            {
                if (!string.IsNullOrWhiteSpace(FDID))//更新
                {
                    int fdid = FDID.ToInt();
                    var model = ef.Flight_Dynamic.Where(p => p.FDID == fdid).FirstOrDefault();
                    if (model != null)
                    {
                        model.FLG_IDU = FLG_IDU;
                        model.OPERATION_DATE = OPERATION_DATE.ToDateTime();
                        model.FLIGHT_NO = FLIGHT_NO;
                        model.AORD = AORD;
                        model.DORI = DORI;
                        model.TASK_CODE = TASK_CODE;
                        model.TERMINAL_NO = TERMINAL_NO;//航站楼 
                        model.AIRLINE_IATA = AIRLINE_IATA;//航空公司IATA代码
                        model.AIRCRAFT_TYPE_IATA = AIRCRAFT_TYPE_IATA;//机型IATA代码
                        model.AC_REG_NO = AC_REG_NO;//机号 
                        model.SERVICE_CLASS = SERVICE_CLASS;//服务类型 
                        model.FLG_VIP = FLG_VIP;//要客标识 
                        model.ORIGIN_AIRPORT_IATA = ORIGIN_AIRPORT_IATA;//起飞机场IAIA代码
                        model.STD = STD.ToDateTime();
                        model.ETD = ETD.ToDateTime();
                        model.ATD = ATD.ToDateTime();
                        model.DEST_AIRPORT_IATA = DEST_AIRPORT_IATA;
                        model.STA = STA.ToDateTime();
                        model.ETA = ETA.ToDateTime();
                        model.ATA = ATA.ToDateTime();
                        model.PREVIOUS_FLIGHT = PREVIOUS_FLIGHT;
                        model.NEXT_FLIGHT = NEXT_FLIGHT;
                        model.ABNORMAL_STATUS = ABNORMAL_STATUS;
                        model.AIRPORT1 = AIRPORT1;
                        model.DORI1 = DORI1;
                        model.STD1 = STD1.ToDateTime();
                        model.AIRPORT2 = AIRPORT2;
                        model.DORI2 = DORI2;
                        model.STA2 = STA2.ToDateTime();
                        model.STD2 = STD2.ToDateTime();
                        model.AIRPORT3 = AIRPORT3;
                        model.DORI3 = DORI3;
                        model.STA3 = STA3.ToDateTime();
                        model.STD3 = STD3.ToDateTime();
                        model.AIRPORT4 = AIRPORT4;
                        model.DORI4 = DORI4;
                        model.STA4 = STA4.ToDateTime();
                        model.CODE_SHARE1 = CODE_SHARE1;
                        model.CODE_SHARE2 = CODE_SHARE2;
                        model.CODE_SHARE3 = CODE_SHARE3;
                        model.CODE_SHARE4 = CODE_SHARE4;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new Flight_Dynamic();
                    model.ID = 0;
                    model.FLG_IDU = FLG_IDU;
                    model.OPERATION_DATE = OPERATION_DATE.ToDateTime();
                    model.FLIGHT_NO = OPERATION_DATE;
                    model.AORD = AORD;
                    model.DORI = DORI;
                    model.TASK_CODE = TASK_CODE;
                    model.TERMINAL_NO = TERMINAL_NO;//航站楼 
                    model.AIRLINE_IATA = AIRLINE_IATA;//航空公司IATA代码
                    model.AIRCRAFT_TYPE_IATA = AIRCRAFT_TYPE_IATA;//机型IATA代码
                    model.AC_REG_NO = AC_REG_NO;//机号 
                    model.SERVICE_CLASS = SERVICE_CLASS;//服务类型 
                    model.FLG_VIP = FLG_VIP;//要客标识 
                    model.ORIGIN_AIRPORT_IATA = ORIGIN_AIRPORT_IATA;//起飞机场IAIA代码
                    model.STD = STD.ToDateTime();
                    model.ETD = ETD.ToDateTime();
                    model.ATD = ATD.ToDateTime();
                    model.DEST_AIRPORT_IATA = DEST_AIRPORT_IATA;
                    model.STA = STA.ToDateTime();
                    model.ETA = ETA.ToDateTime();
                    model.ATA = ATA.ToDateTime();
                    model.PREVIOUS_FLIGHT = PREVIOUS_FLIGHT;
                    model.NEXT_FLIGHT = NEXT_FLIGHT;
                    model.ABNORMAL_STATUS = ABNORMAL_STATUS;
                    model.AIRPORT1 = AIRPORT1;
                    model.DORI1 = DORI1;
                    model.STD1 = STD1.ToDateTime();
                    model.AIRPORT2 = AIRPORT2;
                    model.DORI2 = DORI2;
                    model.STA2 = STA2.ToDateTime();
                    model.STD2 = STD2.ToDateTime();
                    model.AIRPORT3 = AIRPORT3;
                    model.DORI3 = DORI3;
                    model.STA3 = STA3.ToDateTime();
                    model.STD3 = STD3.ToDateTime();
                    model.AIRPORT4 = AIRPORT4;
                    model.DORI4 = DORI4;
                    model.STA4 = STA4.ToDateTime();
                    model.CODE_SHARE1 = CODE_SHARE1;
                    model.CODE_SHARE2 = CODE_SHARE2;
                    model.CODE_SHARE3 = CODE_SHARE3;
                    model.CODE_SHARE4 = CODE_SHARE4;
                    model.ADD_TYPE = 1;
                    ef.Flight_Dynamic.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 单条航班数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFlightData()
        {
            var result = new ResultDto<Flight_Dynamic>();
            try
            {
                Expression<Func<Flight_Dynamic, bool>> where = PredicateExtensionses.True<Flight_Dynamic>();
                string FDID = DNTRequest.GetString("FDID");
                if (!string.IsNullOrWhiteSpace(FDID))
                {
                    int id = FDID.ToInt();
                    where = where.And(p => p.FDID == id);
                }
                var model = ef.Flight_Dynamic.Where(where).FirstOrDefault();
                result.Status = 1;
                result.Message = "is success";
                result.Data = model;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 航班资源管理信息
        /// <summary>
        /// 资源管理数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFlightResourceDataList(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<Flight_Dynamic, bool>> where = PredicateExtensionses.True<Flight_Dynamic>();
                string tid = DNTRequest.GetString("startId");//下标ID
                string planType = DNTRequest.GetString("planType");//0次日计划 1今日计划 
                string keywrod = DNTRequest.GetString("keyword");//搜索
                if (!string.IsNullOrWhiteSpace(tid))
                {
                    where = where.And(p => p.ID > tid.ToInt());
                }
                if (!string.IsNullOrWhiteSpace(planType))
                {
                    var dstr = DateTime.Now.ToShortDateString();
                    if (planType == "0")//次日计划的航班
                    {
                        var stime = dstr.ToDateTime().AddDays(1);
                        var etime = stime.AddDays(1).AddSeconds(-1);
                        where = where.And(p => p.STA >= stime && p.STA <= etime || p.STD >= stime && p.STD <= etime);
                    }
                    else if (planType == "1")//当日计划
                    {
                        var stime = dstr.ToDateTime();
                        var etime = stime.AddDays(1).AddSeconds(-1);
                        where = where.And(p => p.STA >= stime && p.STA <= etime || p.STD >= stime && p.STD <= etime);
                    }
                }
                if (!string.IsNullOrWhiteSpace(keywrod))
                {
                    //过滤关键字
                    var cKeyword = Utils.Utils.CheckSQLHtml(keywrod);
                    bool isEqual = cKeyword == keywrod ? true : false;
                    where = where.And(p => cKeyword.Contains(p.FLIGHT_NO));
                }
                //相应条件的航班数据
                var resource_list = ef.Flight_Dynamic.Where(where).ToList();
                var where_res = PredicateExtensionses.True<Flight_Resource_Allocation>();
                if (resource_list.Count > 0)
                {
                    //取航班号
                    var flight_no = resource_list.Select(s => s.FLIGHT_NO).ToList();
                    //航班号当作条件放入res表的查询
                    where_res = where_res.And(p => flight_no.Contains(p.FLIGHT_NO));
                }
                else
                {
                    where_res = where_res.And(p => p.FLIGHT_NO == "");
                }
                //取到资源的结果集
                var list = ef.Flight_Resource_Allocation.Where(where_res).OrderByDescending(o => o.ID).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.Flight_Resource_Allocation.Where(where_res).Count();
                //处理结果集序列化
                JsonSerializerSettings setting = new JsonSerializerSettings()                {                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore                };                var ret = JsonConvert.SerializeObject(list, setting).DeserializeJSON<List<Flight_Resource_Allocation>>();

                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = ret, Count = rowCount };

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/FlightDataList error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 新增资源管理数据
        /// </summary>
        /// <returns></returns>
        public JsonResult AddFlightResourceData()
        {
            var result = new ResultDto<dynamic>();
            string FRID = DNTRequest.GetString("FRID");
            string OPERATION_DATE = DNTRequest.GetString("OPERATION_DATE");//运营日
            string FacilityType = DNTRequest.GetString("FacilityType");//设备类型
            string FLIGHT_NO = DNTRequest.GetString("FLIGHT_NO");//航班号
            string AORD = DNTRequest.GetString("AORD");//到离港标识
            string SCHED_ID = DNTRequest.GetString("SCHED_ID");//资源ID
            string SCHED_START = DNTRequest.GetString("SCHED_START");//计划开始时间
            string SCHED_END = DNTRequest.GetString("SCHED_END");//计划结束时间
            string ESTIMATE_ID = DNTRequest.GetString("ESTIMATE_ID");//资源ID
            string ESTIMATE_START = DNTRequest.GetString("ESTIMATE_START");//预计开始时间
            string ESTIMATE_END = DNTRequest.GetString("ESTIMATE_END");//预计结束时间
            string ACTUAL_ID = DNTRequest.GetString("ACTUAL_ID");// 资源ID
            string ACTUAL_START = DNTRequest.GetString("ACTUAL_START");//实际开始时间
            string ACTUAL_END = DNTRequest.GetString("ACTUAL_END");//实际结束时间
            try
            {
                if (!string.IsNullOrWhiteSpace(FRID))//更新
                {
                    int frid = FRID.ToInt();
                    var model = ef.Flight_Resource_Allocation.Where(p => p.FRID == frid).FirstOrDefault();
                    if (model != null)
                    {
                        model.OPERATION_DATE = OPERATION_DATE.ToDateTime();
                        model.FLIGHT_NO = OPERATION_DATE;
                        model.AORD = AORD;
                        model.FacilityType = FacilityType.ToInt();
                        model.SCHED_ID = SCHED_ID.ToInt();
                        model.SCHED_START = SCHED_START.ToDateTime();
                        model.SCHED_END = SCHED_END.ToDateTime();
                        model.ESTIMATE_ID = ESTIMATE_ID.ToInt();
                        model.ESTIMATE_START = ESTIMATE_START.ToDateTime();
                        model.ESTIMATE_END = ESTIMATE_END.ToDateTime();
                        model.ACTUAL_ID = ACTUAL_ID.ToInt();
                        model.ACTUAL_START = ACTUAL_START.ToDateTime();
                        model.ACTUAL_END = ACTUAL_END.ToDateTime();
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new Flight_Resource_Allocation();
                    model.ID = 0;
                    model.OPERATION_DATE = OPERATION_DATE.ToDateTime();
                    model.FLIGHT_NO = OPERATION_DATE;
                    model.AORD = AORD;
                    model.FacilityType = FacilityType.ToInt();
                    model.SCHED_ID = SCHED_ID.ToInt();
                    model.SCHED_START = SCHED_START.ToDateTime();
                    model.SCHED_END = SCHED_END.ToDateTime();
                    model.ESTIMATE_ID = ESTIMATE_ID.ToInt();
                    model.ESTIMATE_START = ESTIMATE_START.ToDateTime();
                    model.ESTIMATE_END = ESTIMATE_END.ToDateTime();
                    model.ACTUAL_ID = ACTUAL_ID.ToInt();
                    model.ACTUAL_START = ACTUAL_START.ToDateTime();
                    model.ACTUAL_END = ACTUAL_END.ToDateTime();
                    ef.Flight_Resource_Allocation.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 单条资源管理数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetFlightResourceData()
        {
            var result = new ResultDto<dynamic>();
            try
            {
                Expression<Func<Flight_Resource_Allocation, bool>> where = PredicateExtensionses.True<Flight_Resource_Allocation>();
                string ID = DNTRequest.GetString("ID");
                if (!string.IsNullOrWhiteSpace(ID))
                {
                    int id = ID.ToInt();
                    where = where.And(p => p.FRID == id);
                }
                var model = ef.Flight_Resource_Allocation.Where(where).FirstOrDefault();
                //处理结果集序列化
                JsonSerializerSettings setting = new JsonSerializerSettings()                {                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore                };                var ret = JsonConvert.SerializeObject(model, setting);

                result.Status = 1;
                result.Message = "is success";
                result.Data = ret;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        public JsonResult UploadFile()
        {
            var result = new ResultDto<string>();
            Response.ContentType = "text/plain";
            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            List<Flight_Dynamic> fdList = new List<Flight_Dynamic>();
            //转stream
            try
            {
                if (hfc.Count > 0)
                {
                    Stream fs = hfc[0].InputStream;
                    //读取Excel然后进行处理
                    var dt = ExcelHelper.UltraExcelToDataTable(hfc[0].FileName, fs);


                    //从基础数据表根据 ICAO 找到 IATA
                    var airportList = ef.F_Airport.ToList();
                    //进行数据写入
                    foreach (DataRow dr in dt.Rows)
                    {
                        Flight_Dynamic arrival = new Flight_Dynamic();
                        arrival.ID = 0;
                        arrival.FLG_IDU = "I";
                        arrival.AORD = "A";
                        arrival.DORI = "D";//国内
                        arrival.OPERATION_DATE = dr["OPERATION_DATE"].ToDateTime();//运营日
                        arrival.FLIGHT_NO = dr["A_FLIGHT_NO"].ToString();//进港航班号

                        var aoai = airportList.Where(p => p.AIRPORT_ICAO == dr["A_ORIGIN_AIRPORT_IATA"].ToString()).FirstOrDefault();
                        var A_ORIGIN_AIRPORT_IATA = aoai != null ? aoai.AIRPORT_IATA : string.Empty;
                        arrival.ORIGIN_AIRPORT_IATA = A_ORIGIN_AIRPORT_IATA;//起飞机场

                        var adai = airportList.Where(p => p.AIRPORT_ICAO == dr["A_DEST_AIRPORT_IATA"].ToString()).FirstOrDefault();
                        var A_DEST_AIRPORT_IATA = adai != null ? adai.AIRPORT_IATA : string.Empty;
                        arrival.DEST_AIRPORT_IATA = A_DEST_AIRPORT_IATA;//本地机场

                        arrival.TASK_CODE = dr["A_TASK_CODE"].ToString();
                        arrival.STA = dr["A_STA"].ToDateTime();
                        arrival.ETA = dr["A_ETA"].ToDateTime();
                        arrival.ADD_TYPE = 1;
                        fdList.Add(arrival);

                        Flight_Dynamic dep = new Flight_Dynamic();
                        dep.ID = 0;
                        dep.FLG_IDU = "I";
                        dep.AORD = "D";
                        dep.DORI = "D";//国内
                        dep.OPERATION_DATE = dr["OPERATION_DATE"].ToDateTime();//运营日
                        dep.FLIGHT_NO = dr["D_FLIGHT_NO"].ToString();//离港航班号

                        dep.ORIGIN_AIRPORT_IATA = A_ORIGIN_AIRPORT_IATA;//从本地机场起飞
                        var ddai = airportList.Where(p => p.AIRPORT_ICAO == dr["D_DEST_AIRPORT_IATA"].ToString()).FirstOrDefault();
                        var D_DEST_AIRPORT_IATA = ddai != null ? aoai.AIRPORT_IATA : string.Empty;
                        dep.DEST_AIRPORT_IATA = D_DEST_AIRPORT_IATA;//着陆机场

                        dep.TASK_CODE = dr["D_TASK_CODE"].ToString();
                        dep.STD = dr["D_STD"].ToDateTime();
                        dep.ETD = dr["D_ETD"].ToDateTime();
                        dep.ADD_TYPE = 1;
                        fdList.Add(dep);
                    }
                    //在导入之前把已经存在的数据进行移除，根据航班号
                    var readyFDIDs = fdList.Select(s => s.FLIGHT_NO).ToList();
                    var existData = ef.Flight_Dynamic.Where(p => readyFDIDs.Contains(p.FLIGHT_NO)).ToList();
                    ef.Flight_Dynamic.RemoveRange(existData);
                    ef.Flight_Dynamic.AddRange(fdList);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "航班导入成功！";
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/UploadFile error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 基础数据获取
        /// <summary>
        /// 获取飞机数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAirCraftData(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_Aircraft.OrderByDescending(o => o.AC_REG_NO).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_Aircraft.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };

            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取机型数据
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetAirCraftType(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_AircraftType.OrderByDescending(o => o.iataCode).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_AircraftType.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 航空公司
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetAirLineData(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_Airline.OrderByDescending(o => o.NAME_CHINESE).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_Airline.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 航空联盟
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetAllianceData(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_Alliance.OrderByDescending(o => o.NAME_CHINESE).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_Alliance.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 机场
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetAirportData(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_Airport.OrderByDescending(o => o.NAME_CHINESE).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_Airport.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 城市
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetCity(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_City.OrderByDescending(o => o.Name_Chinese).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_City.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  国家
        /// </summary> 
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetCountry(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_Country.OrderByDescending(o => o.Name_Chinese).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_Country.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 省份
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetProvince(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_Province.OrderByDescending(o => o.Name_Chinese).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_Province.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 延误代码
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetDelayCode(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_DelayCode.OrderByDescending(o => o.Code_Chinese).Skip(startRow).Take(pageSize).ToList();
                var rowCount = ef.F_DelayCode.ToList().Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 任务代码
        /// </summary>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public JsonResult GetTaskCode(int startRow = 0, int pageSize = 10)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                var list = ef.F_TaskCode.Where(p => p.Is_Civil == "Y").OrderByDescending(o => o.Name_Chinese).Skip(startRow).Take(pageSize).ToList();//任务代码
                var rowCount = ef.F_TaskCode.Where(p => p.Is_Civil == "Y").Count();
                result.Status = 1;
                result.Message = "is success";
                result.Data = new { List = list, Count = rowCount };
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/GetFlightBasicsData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 基础数据新增/编辑

        public JsonResult AddairCraftData()
        {
            var result = new ResultDto<dynamic>();
            string air_ID = DNTRequest.GetString("ID");
            string AC_REG_NO = DNTRequest.GetString("AC_REG_NO");
            string AC_TYPE_IATA = DNTRequest.GetString("AC_TYPE_IATA");
            string AIRLINE_IATA = DNTRequest.GetString("AIRLINE_IATA");
            //string SUBAIRLINE_ID = DNTRequest.GetString("SUBAIRLINE_ID");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            string EXT_CODE = DNTRequest.GetString("EXT_CODE");
            try
            {
                if (!string.IsNullOrWhiteSpace(air_ID))//更新
                {
                    int id = air_ID.ToInt();
                    var model = ef.F_Aircraft.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.AC_REG_NO = AC_REG_NO;
                        model.AC_TYPE_IATA = AC_TYPE_IATA;
                        model.AIRLINE_IATA = AIRLINE_IATA;
                        //model.SUBAIRLINE_ID = SUBAIRLINE_ID.ToInt();
                        model.FLG_DELETED = FLG_DELETED;
                        model.EXT_CODE = EXT_CODE;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_Aircraft();
                    model.AC_REG_NO = AC_REG_NO;
                    model.AC_TYPE_IATA = AC_TYPE_IATA;
                    model.AIRLINE_IATA = AIRLINE_IATA;
                    int? sub_id = null;
                    model.SUBAIRLINE_ID = sub_id;
                    model.FLG_DELETED = FLG_DELETED;
                    model.EXT_CODE = EXT_CODE;
                    ef.F_Aircraft.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                } 
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddairCraftType()
        {
            var result = new ResultDto<dynamic>();
            string air_ID = DNTRequest.GetString("ID");
            string iataCode = DNTRequest.GetString("iataCode");
            string icaoCode = DNTRequest.GetString("icaoCode");
            string name_english = DNTRequest.GetString("name_english");
            string name_chinese = DNTRequest.GetString("name_chinese");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(air_ID))//更新
                {
                    int id = air_ID.ToInt();
                    var model = ef.F_AircraftType.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.iataCode = iataCode;
                        model.icaoCode = icaoCode;
                        model.name_english = name_english;
                        model.name_chinese = name_chinese;
                        model.FLG_DELETED = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_AircraftType();
                    model.iataCode = iataCode;
                    model.icaoCode = icaoCode;
                    model.name_english = name_english;
                    model.name_chinese = name_chinese;
                    model.FLG_DELETED = FLG_DELETED;
                    ef.F_AircraftType.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddairLineData()
        {
            var result = new ResultDto<dynamic>();
            string air_ID = DNTRequest.GetString("ID");
            string Airline_IATA = DNTRequest.GetString("Airline_IATA");
            string Airline_ICAO = DNTRequest.GetString("Airline_ICAO");
            string Short_Name = DNTRequest.GetString("Short_Name");
            string Host_AirPort_IATA = DNTRequest.GetString("Host_AirPort_IATA");
            //string MaxLogo = DNTRequest.GetString("MaxLogo");
            //string MinLogo = DNTRequest.GetString("MinLogo");
            string DORI = DNTRequest.GetString("DORI");
            string NAME_ENGLISH = DNTRequest.GetString("NAME_ENGLISH");
            string NAME_CHINESE = DNTRequest.GetString("NAME_CHINESE");
            string ALLIANCE_CODE = DNTRequest.GetString("ALLIANCE_CODE");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(air_ID))//更新
                {
                    int id = air_ID.ToInt();
                    var model = ef.F_Airline.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.Airline_IATA = Airline_IATA;
                        model.Airline_ICAO = Airline_ICAO;
                        model.Short_Name = Short_Name;
                        model.Host_AirPort_IATA = Host_AirPort_IATA;
                        //model.MaxLogo = MaxLogo;
                        //model.MinLogo = MinLogo;
                        model.DORI = DORI;
                        model.NAME_ENGLISH = NAME_ENGLISH;
                        model.NAME_CHINESE = NAME_CHINESE;
                        model.ALLIANCE_CODE = ALLIANCE_CODE;
                        model.FLG_DELETED = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_Airline();
                    model.Airline_IATA = Airline_IATA;
                    model.Airline_ICAO = Airline_ICAO;
                    model.Short_Name = Short_Name;
                    model.Host_AirPort_IATA = Host_AirPort_IATA;
                    //model.MaxLogo = MaxLogo;
                    //model.MinLogo = MinLogo;
                    model.DORI = DORI;
                    model.NAME_ENGLISH = NAME_ENGLISH;
                    model.NAME_CHINESE = NAME_CHINESE;
                    model.ALLIANCE_CODE = ALLIANCE_CODE;
                    model.FLG_DELETED = FLG_DELETED;
                    ef.F_Airline.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddallianceData()
        {
            var result = new ResultDto<dynamic>();
            string all_ID = DNTRequest.GetString("ID");
            string ALLIANCE_NAME = DNTRequest.GetString("ALLIANCE_NAME");
            string NAME_CHINESE = DNTRequest.GetString("NAME_CHINESE");
            string NAME_ENGLISH = DNTRequest.GetString("NAME_ENGLISH");
            string REMARK = DNTRequest.GetString("REMARK");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(all_ID))//更新
                {
                    int id = all_ID.ToInt();
                    var model = ef.F_Alliance.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.ALLIANCE_NAME = ALLIANCE_NAME;
                        model.NAME_CHINESE = NAME_CHINESE;
                        model.NAME_ENGLISH = NAME_ENGLISH;
                        model.REMARK = REMARK;
                        model.FLG_DELETED = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_Alliance();
                    model.ALLIANCE_NAME = ALLIANCE_NAME;
                    model.NAME_CHINESE = NAME_CHINESE;
                    model.NAME_ENGLISH = NAME_ENGLISH;
                    model.REMARK = REMARK;
                    model.FLG_DELETED = FLG_DELETED;
                    ef.F_Alliance.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddairPortData()
        {
            var result = new ResultDto<dynamic>();
            string air_ID = DNTRequest.GetString("ID");
            string AIRPORT_IATA = DNTRequest.GetString("AIRPORT_IATA");
            string AIRPORT_ICAO = DNTRequest.GetString("AIRPORT_ICAO");
            string SHORT_NAME = DNTRequest.GetString("SHORT_NAME");
            string CITY_IATA = DNTRequest.GetString("CITY_IATA");
            string DORI = DNTRequest.GetString("DORI");
            string NAME_ENGLISH = DNTRequest.GetString("NAME_ENGLISH");
            string NAME_CHINESE = DNTRequest.GetString("NAME_CHINESE");
            string REGION_CODE = DNTRequest.GetString("REGION_CODE");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(AIRPORT_IATA))//更新
                {
                    int id = air_ID.ToInt();
                    var model = ef.F_Airport.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.AIRPORT_IATA = AIRPORT_IATA;
                        model.AIRPORT_ICAO = AIRPORT_ICAO;
                        model.SHORT_NAME = SHORT_NAME;
                        model.CITY_IATA = CITY_IATA;
                        model.DORI = DORI;
                        model.NAME_ENGLISH = NAME_ENGLISH;
                        model.NAME_CHINESE = NAME_CHINESE;
                        model.REGION_CODE = REGION_CODE;
                        model.FLG_DELETED = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_Airport();
                    model.AIRPORT_IATA = AIRPORT_IATA;
                    model.AIRPORT_ICAO = AIRPORT_ICAO;
                    model.SHORT_NAME = SHORT_NAME;
                    model.CITY_IATA = CITY_IATA;
                    model.DORI = DORI;
                    model.NAME_ENGLISH = NAME_ENGLISH;
                    model.NAME_CHINESE = NAME_CHINESE;
                    model.REGION_CODE = REGION_CODE;
                    model.FLG_DELETED = FLG_DELETED;
                    ef.F_Airport.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCityData()
        {
            var result = new ResultDto<dynamic>();
            string city_ID = DNTRequest.GetString("ID");
            string City_IATA = DNTRequest.GetString("City_IATA");
            string Country_IATA = DNTRequest.GetString("Country_IATA");
            string City_ICAO = DNTRequest.GetString("City_ICAO");
            string Name_Chinese = DNTRequest.GetString("Name_Chinese");
            string Name_English = DNTRequest.GetString("Name_English");
            string Short_Chinese = DNTRequest.GetString("Short_Chinese");
            string Province_IS = DNTRequest.GetString("Province_IS");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(City_IATA))//更新
                {
                    int id = city_ID.ToInt();
                    var model = ef.F_City.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.City_IATA = City_IATA;
                        model.Country_IATA = Country_IATA;
                        model.City_ICAO = City_ICAO;
                        model.Name_Chinese = Name_Chinese;
                        model.Name_English = Name_English;
                        model.Short_Chinese = Short_Chinese;
                        model.Province_IS = Province_IS.ToInt();
                        model.FLG_Deleted = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_City();
                    model.City_IATA = City_IATA;
                    model.Country_IATA = Country_IATA;
                    model.City_ICAO = City_ICAO;
                    model.Name_Chinese = Name_Chinese;
                    model.Name_English = Name_English;
                    model.Short_Chinese = Short_Chinese;
                    model.Province_IS = Province_IS.ToInt();
                    model.FLG_Deleted = FLG_DELETED;
                    ef.F_City.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCountryData()
        {
            var result = new ResultDto<dynamic>();
            string country_ID = DNTRequest.GetString("ID");
            string Country_IATA = DNTRequest.GetString("Country_IATA");
            string Country_ICAO = DNTRequest.GetString("Country_ICAO");
            string Name_Chinese = DNTRequest.GetString("Name_Chinese");
            string Name_English = DNTRequest.GetString("Name_English");
            string FLG_DELETED = DNTRequest.GetString("FLG_Deleted");
            try
            {
                if (!string.IsNullOrWhiteSpace(Country_IATA))//更新
                {
                    int id = country_ID.ToInt();
                    var model = ef.F_Country.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.Country_IATA = Country_IATA;
                        model.Country_ICAO = Country_ICAO;
                        model.Country_ICAO = Country_ICAO;
                        model.Name_Chinese = Name_Chinese;
                        model.Name_English = Name_English;
                        model.FLG_Deleted = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_Country();
                    model.Country_IATA = Country_IATA;
                    model.Country_ICAO = Country_ICAO;
                    model.Country_ICAO = Country_ICAO;
                    model.Name_Chinese = Name_Chinese;
                    model.Name_English = Name_English;
                    model.FLG_Deleted = FLG_DELETED;
                    ef.F_Country.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProvinceData()
        {
            var result = new ResultDto<dynamic>();
            string Province_ID = DNTRequest.GetString("Province_ID");
            string Name_Chinese = DNTRequest.GetString("Name_Chinese");
            string Name_English = DNTRequest.GetString("Name_English");
            string Short_Name = DNTRequest.GetString("Short_Name");
            string DORI = DNTRequest.GetString("DORI");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(Province_ID))//更新
                {
                    int pro_id = Province_ID.ToInt();
                    var model = ef.F_Province.Where(p => p.Province_ID == pro_id).FirstOrDefault();
                    if (model != null)
                    {
                        model.Name_Chinese = Name_Chinese;
                        model.Name_English = Name_English;
                        model.Short_Name = Short_Name;
                        model.DORI = DORI;
                        model.FLG_DELETED = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_Province();
                    model.Name_Chinese = Name_Chinese;
                    model.Name_English = Name_English;
                    model.Short_Name = Short_Name;
                    model.DORI = DORI;
                    model.FLG_DELETED = FLG_DELETED;
                    ef.F_Province.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddTaskCode()
        {
            var result = new ResultDto<dynamic>();
            string ID = DNTRequest.GetString("ID");
            string Task_Code = DNTRequest.GetString("Task_Code");
            string Name_Chinese = DNTRequest.GetString("Name_Chinese");
            string Name_English = DNTRequest.GetString("Name_English");
            string Description = DNTRequest.GetString("Description");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(ID))//更新
                {
                    int id = ID.ToInt();
                    var model = ef.F_TaskCode.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.Task_Code = Task_Code;
                        model.Name_Chinese = Name_Chinese;
                        model.Name_English = Name_English;
                        model.Description = Description;
                        model.Is_Civil = "Y";
                        model.FLG_Deleted = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_TaskCode();
                    model.Task_Code = Task_Code;
                    model.Name_Chinese = Name_Chinese;
                    model.Name_English = Name_English;
                    model.Description = Description;
                    model.Is_Civil = "Y";
                    model.FLG_Deleted = FLG_DELETED;
                    ef.F_TaskCode.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddDelayCode()
        {
            var result = new ResultDto<dynamic>();
            string delay_ID = DNTRequest.GetString("ID");
            string Delay_Code = DNTRequest.GetString("Delay_Code");
            string Type = DNTRequest.GetString("Type");
            string Code_Chinese = DNTRequest.GetString("Code_Chinese");
            string Code_English = DNTRequest.GetString("Code_English");
            string Description = DNTRequest.GetString("Description");
            string FLG_DELETED = DNTRequest.GetString("FLG_DELETED");
            try
            {
                if (!string.IsNullOrWhiteSpace(Delay_Code))//更新
                {
                    int id = delay_ID.ToInt();
                    var model = ef.F_DelayCode.Where(p => p.ID == id).FirstOrDefault();
                    if (model != null)
                    {
                        model.Delay_Code = Delay_Code;
                        model.Type = Type;
                        model.Code_Chinese = Code_Chinese;
                        model.Code_English = Code_English;
                        model.Description = Description;
                        model.FLG_Deleted = FLG_DELETED;
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "更新成功！";
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "更新失败，无法查询到此航班数据！";
                    }
                }
                else //新增
                {
                    var model = new F_DelayCode();
                    model.Delay_Code = Delay_Code;
                    model.Type = Type;
                    model.Code_Chinese = Code_Chinese;
                    model.Code_English = Code_English;
                    model.Description = Description;
                    model.FLG_Deleted = FLG_DELETED;
                    ef.F_DelayCode.Add(model);
                    ef.SaveChanges();
                    result.Status = 1;
                    result.Message = "添加成功！";
                }
                
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/FlightDataApi/AddFlightData error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
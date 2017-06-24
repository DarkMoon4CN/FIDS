using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Text;
using System.Web.Mvc;
using CATC.FIDS.Models;
using CATC.FIDS.Model;
using CATC.FIDS.Utils;
using Newtonsoft.Json;
using CATC.FIDS.Factory;
using System.Data.Entity.Infrastructure;

namespace CATC.FIDS.Controllers
{
    [ControllerAuth]
    public class DisplayInfoesController : BaseController
    {
        private CATC_FIDS_DBEntities db = new CATC_FIDS_DBEntities();

        // GET: DisplayInfoes
        public ActionResult Index()
        {
            return View();
        }

        //查找所有记录
        public JsonResult SelectAllRecord()
        {
            try
            {
                string isPrimary = DNTRequest.GetString("isPrimary_select");
                //
                string displayGroup_name = DNTRequest.GetString("displayGroup_name");

                //string dddd = Json(db.DisplayInfo.ToList());
                //List<R_DisplayInfo> json_str = null;
                var query = from a in db.R_DisplayInfo from b in db.R_DeviceGroup where a.displayGroup == b.ID select new { a.ip, a.displayID, a.displayName, a.displayState, a.displayLuminance, a.displayMark, a.isPrimary, a.displayGroup, b.name };
                if (isPrimary != "")
                {
                    int int_is_p = int.Parse(isPrimary);
                    query = query.Where(d => d.isPrimary == int_is_p);
                    if (displayGroup_name != "")
                    {
                        int int_displayGroup_name = int.Parse(displayGroup_name);
                        query = query.Where(d => d.displayGroup == int_displayGroup_name);
                    }
                }

                var result = new ResultDto<dynamic>()
                {
                    Status = 1,
                    Message = "is success",
                    Data = query.ToList()  //此数据带入至父页面
                };

                return Json(query.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("SelectAllRecord:" + ex.Message);
                return null;
            }
        }

        //修改一条记录
        public JsonResult UpdateOneRecord()
        {
            try
            {
                //json字符串实体反序列化
                //string json = "{\"displayID\":39,\"ip\":\"192.9.200.35\",\"displayName\":\".DISPLAY1\",\"x\":0,\"y\":0,\"width\":1920,\"height\":1080,\"index\":0,\"isPrimary\":1,\"url\":\"http://192.9.200.36/Departure\",\"createTime\":\"2017-02-21 10:50:33\",\"displayLuminance\":140,\"displayState\":1,\"displayGroup\":22,\"displayMAC\":\"C8:5B:76:18:31:23\"}";
                //var req = JsonConvert.DeserializeObject<DisplayInfo>(json);

                var result = new ResultDto<string>();
                int displayID_name = int.Parse(DNTRequest.GetString("displayID_name"));
                string displayName_name = DNTRequest.GetString("displayName_name");
                string url_name = DNTRequest.GetString("url_name");
                string displayLuminance_name = DNTRequest.GetString("displayLuminance_name");
                string displayGroup_name = DNTRequest.GetString("displayGroup_name");
                string displayMark_name = DNTRequest.GetString("displayMark_name");
                string isPrimary_name = DNTRequest.GetString("isPrimary_name");

                //发送到设备的信息
                var entFlag = ef.R_DisplayInfo.Where(p => p.displayID == displayID_name).FirstOrDefault();
                if (displayName_name != "")
                {
                    entFlag.displayName = displayName_name;
                }
                if (displayLuminance_name != "")
                {
                    entFlag.displayLuminance = int.Parse(displayLuminance_name);
                }
                if (displayGroup_name != "")
                {
                    entFlag.displayGroup = int.Parse(displayGroup_name);
                }
                if (displayMark_name != "")
                {
                    entFlag.displayMark = displayMark_name;
                }

                if (isPrimary_name != "")
                {
                    entFlag.isPrimary = int.Parse(isPrimary_name);
                }

                List<R_DisplayInfo> resultlist = new List<R_DisplayInfo>();
                ResultDto<string> result_to = new ResultDto<string>();
                resultlist.Add(entFlag);

                try
                {
                    result_to.Status = 1;
                    result_to.Message = "设备更新操作";
                    result_to.Data = resultlist.SerializeJSON();
                    string resultStr = RequestHelper.SendHttpRequest("http://" + entFlag.ip + ":8888/Client/ExecuteCommand", "POST", new { Json = result_to.SerializeJSON() }.SerializeJSON());

                    result.Status = 1;
                    result.Message = "修改信息命令发送成功！";
                    result.Data = "";

                }
                catch (Exception ex)
                {
                    result.Status = 1;
                    result.Message = "修改信息命令发送失败！";
                    result.Data = "";
                    LoggerHelper.Error("修改信息命令发送失败" + ex.Message);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("设备更新失败" + ex.Message);
                var result = new ResultDto<string>()
                {
                    Status = 1,
                    Message = "设备更新失败!",
                    Data = ex.Message  //此数据带入至父页面
                };
                return Json(result);
            }
        }

        //操作函数
        public JsonResult ControlMethod()
        {
            ResultDto<string> result = new ResultDto<string>();
            try
            {
                string msg_type = DNTRequest.GetString("message");
                string data_str = DNTRequest.GetString("data");
                //req.Message = "start_on";
                string[] id_list_str = data_str.Split(',');
                List<int> id_list_int = new List<int>();
                foreach (string id_str in id_list_str)
                {
                    id_list_int.Add(int.Parse(id_str));
                }
                List<R_DisplayInfo> displayinforlist = ef.R_DisplayInfo.Where(c => id_list_int.Contains(c.displayID)).ToList<R_DisplayInfo>();
                //开机操作
                try
                {
                    if (msg_type == "start_on")
                    {
                        foreach (R_DisplayInfo displayinfobj in displayinforlist)
                        {
                            WakeUpTools.WakeUp(displayinfobj.displayMAC);
                            displayinfobj.createTime = DateTime.Now;
                            displayinfobj.displayState = 1;
                        }
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = "启动完毕!";
                        result.Data = "";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("启动设备异常!", ex);
                }

                //关机操作
                try
                {
                    if (msg_type == "shut_down")
                    {
                        StringBuilder msg_list = new StringBuilder();
                        foreach (R_DisplayInfo displayinfobj in displayinforlist)
                        {
                            try
                            {
                                displayinfobj.displayState = 4;

                                ResultDto<string> result_to = new ResultDto<string>();
                                result_to.Status = 1;
                                result_to.Message = "shut_down";
                                result_to.Data = displayinfobj.SerializeJSON();
                                string retrun_resultStr = RequestHelper.SendHttpRequest("http://" + displayinfobj.ip + ":8888/Client/ExecuteCommand", "POST", new { Json = result_to.SerializeJSON() }.SerializeJSON());
                                ResultDto<string> return_json = retrun_resultStr.DeserializeJSON<ResultDto<string>>();
                                msg_list.AppendLine(displayinfobj.displayName + " " + return_json.Message);
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Error(displayinfobj.displayName + ":关机失败!" + ex.Message);
                                msg_list.AppendLine(displayinfobj.displayName + "关机失败");
                            }
                        }
                        result.Status = 1;
                        result.Message = msg_list.ToString();
                        result.Data = "";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("关机操作异常!", ex);
                }

                //重启
                try
                {
                    if (msg_type == "do_reboot")
                    {
                        StringBuilder msg_list = new StringBuilder();
                        foreach (R_DisplayInfo displayinfobj in displayinforlist)
                        {
                            try
                            {
                                displayinfobj.displayState = 3;

                                ResultDto<string> result_to = new ResultDto<string>();
                                result_to.Status = 1;
                                result_to.Message = "doreboot";
                                result_to.Data = displayinfobj.SerializeJSON();
                                string retrun_resultStr = RequestHelper.SendHttpRequest("http://" + displayinfobj.ip + ":8888/Client/ExecuteCommand", "POST", new { Json = result_to.SerializeJSON() }.SerializeJSON());
                                ResultDto<string> return_json = retrun_resultStr.DeserializeJSON<ResultDto<string>>();
                                msg_list.AppendLine(displayinfobj.displayName + " " + return_json.Message);
                            }
                            catch (Exception ex)
                            {
                                LoggerHelper.Error(displayinfobj.displayName + ":重启失败!" + ex.Message);
                                msg_list.AppendLine(displayinfobj.displayName + "重启失败");
                            }
                        }
                        result.Status = 1;
                        result.Message = msg_list.ToString();
                        result.Data = "";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("重启操作异常!", ex);
                }

                //置为无效状态
                try
                {
                    if (msg_type == "invalid")
                    {
                        StringBuilder msg_list = new StringBuilder();
                        foreach (R_DisplayInfo displayinfobj in displayinforlist)
                        {

                            displayinfobj.isPrimary = 0;
                            msg_list.AppendLine(displayinfobj.displayName);

                        }
                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = msg_list.ToString();
                        result.Data = "";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("置为无效状态设置失败!", ex);
                }

                //置为有效状态
                try
                {
                    if (msg_type == "valid")
                    {
                        StringBuilder msg_list = new StringBuilder();
                        foreach (R_DisplayInfo displayinfobj in displayinforlist)
                        {
                            displayinfobj.isPrimary = 1;
                            msg_list.AppendLine(displayinfobj.displayName);
                        }

                        ef.SaveChanges();
                        result.Status = 1;
                        result.Message = msg_list.ToString();
                        result.Data = "";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("置为有效状态设置失败!", ex);
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("ControlMethod：" + ex.Message);
                result.Status = 1;
                result.Message = ex.Message;
                result.Data = "";
                return Json(result);
            }
        }

        //信息修改操作响应
        public JsonResult UpdateDeviceStates()
        {
            var result = new ResultDto<string>();
            try
            {
                string bodyJson = DNTRequest.GetString("send_message");
                var req = JsonConvert.DeserializeObject<ResultDto<string>>(bodyJson);
                if (string.IsNullOrWhiteSpace(bodyJson) || string.IsNullOrWhiteSpace(req.Data))
                {
                    result.Status = 1;
                    result.Message = "请求中显示器数量为 0 !";
                    return Json(result);
                }

                R_DisplayInfo di = req.Data.DeserializeJSON<R_DisplayInfo>();

                if (di == null)
                {
                    result.Status = 1;
                    result.Message = "设备集合为null 或者 count=0 !";
                    return Json(result);
                }

                //修改数据库
                var entFlag = ef.R_DisplayInfo.Where(p => p.displayID == di.displayID).FirstOrDefault();
                //修改数据库时间
                entFlag.displayName = di.displayName;
                entFlag.createTime = DateTime.Now;
                entFlag.displayLuminance = di.displayLuminance;
                entFlag.displayState = di.displayState;
                entFlag.displayGroup = di.displayGroup;
                entFlag.displayMark = di.displayMark;
                entFlag.displayConnectTime = di.displayConnectTime;
                ef.SaveChanges();
                result.Status = 0;
                result.Message = "修改成功!";
                return Json(result);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UpdateDeviceStates：" + ex.Message);
                result.Status = 1;
                result.Message = "修改失败";
                result.Data = "";
                return Json(result);
            }
        }

        //握手操作函数
        public JsonResult UpdateConnectStates()
        {
            var result = new ResultDto<string>();
            try
            {
                string bodyJson = DNTRequest.GetString("connect_message");
                var req = JsonConvert.DeserializeObject<ResultDto<string>>(bodyJson);
                if (string.IsNullOrWhiteSpace(bodyJson) || string.IsNullOrWhiteSpace(req.Data))
                {
                    result.Status = 1;
                    result.Message = "请求中显示器数量为 0 !";
                    return Json(result);
                }

                R_DisplayInfo di = req.Data.DeserializeJSON<R_DisplayInfo>();

                if (di == null)
                {
                    result.Status = 1;
                    result.Message = "设备集合为null 或者 count=0 !";
                    return Json(result);
                }

                //修改数据库
                var entFlag = ef.R_DeviceMonitoring.Where(p => p.pk_DeviceID == di.displayID).FirstOrDefault();
                //修改数据库时间
                entFlag.connectedTime = DateTime.Now;
                entFlag.ExceptionMsg = di.displayMark;
                ef.SaveChanges();
                result.Status = 0;
                result.Message = "修改成功!";
                return Json(result);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("UpdateDeviceStates：" + ex.Message);
                result.Status = 1;
                result.Message = "修改失败";
                result.Data = "";
                return Json(result);
            }
        }

        //修改客户端握手时间
        public JsonResult UpdateConnectTimeALL()
        {
            ResultDto<string> result = new ResultDto<string>();
            try
            {
                string request_time = DNTRequest.GetString("request_time");
                if (request_time == "")
                {
                    result.Status = 1;
                    result.Message = "空值";
                    result.Data = "";
                    return Json(result);
                }
                List<R_DisplayInfo> displayinforlist = ef.R_DisplayInfo.ToList();
                List<string> errorlist = new List<string>();
                if (displayinforlist.Count > 0)
                {
                    foreach (R_DisplayInfo objR_DisplayInfo in displayinforlist)
                    {
                        //数据临时存储
                        int temp_displayConnectTime = objR_DisplayInfo.displayConnectTime;
                        DateTime temp_createTime = objR_DisplayInfo.createTime;

                        objR_DisplayInfo.displayConnectTime = int.Parse(request_time) * 1000;
                        //修改数据库时间
                        objR_DisplayInfo.createTime = DateTime.Now;

                        ResultDto<string> result_to = new ResultDto<string>();
                        //发送到一般设置文件夹发送过去的是列表
                        List<R_DisplayInfo> mylist = new List<R_DisplayInfo>();
                        mylist.Add(objR_DisplayInfo);
                        result_to.Status = 1;
                        result_to.Message = "";
                        result_to.Data = mylist.SerializeJSON();
                        string retrun_resultStr = null;
                        try
                        {
                            retrun_resultStr = RequestHelper.SendHttpRequest("http://" + objR_DisplayInfo.ip + ":8888/Client/ExecuteCommand", "POST", new { Json = result_to.SerializeJSON() }.SerializeJSON());
                        }
                        catch (Exception)
                        {
                            objR_DisplayInfo.displayConnectTime = temp_displayConnectTime;
                            //修改数据库时间
                            objR_DisplayInfo.createTime = temp_createTime;
                        }
                        if (retrun_resultStr != null)
                        {
                            ResultDto<string> return_json = retrun_resultStr.DeserializeJSON<ResultDto<string>>();
                            if (return_json.Message != "文件已接收，正在设置显示器...")
                            {
                                errorlist.Add(objR_DisplayInfo.ip);
                                objR_DisplayInfo.displayConnectTime = temp_displayConnectTime;
                                //修改数据库时间
                                objR_DisplayInfo.createTime = temp_createTime;
                            }
                        }
                    }
                    //修改数据库
                    ef.SaveChanges();
                    if (errorlist.Count == 0)
                    {
                        result.Status = 1;
                        result.Message = "设备更新成功!";
                        result.Data = "";
                    }
                    else
                    {
                        result.Status = 1;
                        result.Message = errorlist.SerializeJSON();
                        result.Data = errorlist.SerializeJSON();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("ControlMethod：" + ex.Message);
                result.Status = 1;
                result.Message = ex.Message;
                result.Data = "";
                return Json(result);
            }

            return Json(result);
        }

        //刷新方法
        public JsonResult RefreshDeviceState()
        {
            List<DisplaysInforStates> listDisplaysInforStates = new List<DisplaysInforStates>();

            try
            {
                string isPrimary = DNTRequest.GetString("isPrimary_select");
                //
                string displayGroup_name = DNTRequest.GetString("displayGroup_name");
                //当前时间
                DateTime now_time = DateTime.Now;
                //string dddd = Json(db.DisplayInfo.ToList());
                //List<R_DisplayInfo> json_str = null;
                var query = from r_di in db.R_DisplayInfo from r_dm in db.R_DeviceMonitoring  from r_ds in db.R_DeviceStatus where r_di.displayID == r_dm.pk_DeviceID && r_dm.pk_DeviceStatusID==r_ds.status_id select new {r_di.displayID,r_di.ip,r_di.displayName,r_di.isPrimary,r_dm.connectedTime,r_ds.Status,r_dm.ExceptionMsg};
                if (isPrimary != "")
                {
                    int int_is_p = int.Parse(isPrimary);
                    query = query.Where(d => d.isPrimary == int_is_p);
                    DisplaysInforStates objDisplaysInforStates = null;
                    foreach (var obj_query in query)
                    {
                        objDisplaysInforStates = new DisplaysInforStates();
                        objDisplaysInforStates.DisplayID = obj_query.displayID;
                        objDisplaysInforStates.Ip = obj_query.ip;
                        objDisplaysInforStates.DisplayName = obj_query.displayName;
                        objDisplaysInforStates.IsPrimary = obj_query.isPrimary;
                        objDisplaysInforStates.ConnectedTime = obj_query.connectedTime.ToString();
                        objDisplaysInforStates.ExceptionMsg = obj_query.ExceptionMsg;

                        TimeSpan ts = now_time - (DateTime)obj_query.connectedTime;
                        if (Math.Abs(ts.TotalSeconds) < 5)
                        {
                            objDisplaysInforStates.Status1 =  "正常!";
                        }
                        else
                        {
                            objDisplaysInforStates.Status1 = "异常!";
                        } 
                    }
                }

                var result = new ResultDto<dynamic>()
                {
                    Status = 1,
                    Message = "is success",
                    Data = listDisplaysInforStates  //此数据带入至父页面
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("SelectAllRecord:" + ex.Message);
                return null;
            }
        }

        //添加组数据
        public JsonResult AddGroupInfor()
        {
            var result = new ResultDto<string>();
            try
            {
                string group_type = DNTRequest.GetString("group_type");
                string group_name = DNTRequest.GetString("group_name");
                string group_description = DNTRequest.GetString("group_description");
                R_DeviceGroup objR_DeviceGroup = new R_DeviceGroup();
                db.R_DeviceGroup.Add(objR_DeviceGroup);
                db.SaveChanges();
                result.Status = 1;
                result.Message = "组数据添加成功!";
                result.Data = "";
                return Json(result);

            }
            catch (Exception ex)
            {
                result.Status = 1;
                result.Message = "组数据添加失败!";
                result.Data = "";
                return Json(result);
            }
        }

        //删除组数据
        public JsonResult DeleteGroupInfor()
        {
            var result = new ResultDto<string>();
            try
            {
                string groupinfor_id = DNTRequest.GetString("groupinfor_id");
                string group_type = DNTRequest.GetString("group_type");
                string group_name = DNTRequest.GetString("group_name");
                string group_description = DNTRequest.GetString("group_description");

                if (groupinfor_id != "")
                {
                    int groupinfor_id_int = int.Parse(groupinfor_id);
                    var entFlag = ef.R_DeviceGroup.Where(p => p.ID == groupinfor_id_int).FirstOrDefault();
                    db.R_DeviceGroup.Remove(entFlag);
                    db.SaveChanges();
                }
                result.Status = 1;
                result.Message = "组数据删除成功!";
                result.Data = "";
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Status = 1;
                result.Message = "组数据删除失败!";
                result.Data = "";
                return Json(result);
            }
        }

        //修改组数据
        public JsonResult UpdateGroupInfor()
        {
            var result = new ResultDto<string>();
            try
            {
                string groupinfor_id = DNTRequest.GetString("groupinfor_id");
                string group_type = DNTRequest.GetString("group_type");
                string group_name = DNTRequest.GetString("group_name");
                string group_description = DNTRequest.GetString("group_description");

                if (groupinfor_id != "")
                {
                    int groupinfor_id_int = int.Parse(groupinfor_id);
                    var entFlag = ef.R_DeviceGroup.Where(p => p.ID == groupinfor_id_int).FirstOrDefault();
                    entFlag.type = group_type;
                    entFlag.name = group_name;
                    entFlag.description = group_description;
                    db.SaveChanges();
                }
                result.Status = 1;
                result.Message = "组数据修改成功!";
                result.Data = "";
                return Json(result);
            }
            catch (Exception ex)
            {
                result.Status = 1;
                result.Message = "组数据修改失败!";
                result.Data = "";
                return Json(result);
            }
        }

        //查找组数据
        public string SelectGroupInfor()
        {
            string json_str = "";
            try
            {
                json_str = db.R_DeviceGroup.ToList().SerializeJSON();
                return json_str;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

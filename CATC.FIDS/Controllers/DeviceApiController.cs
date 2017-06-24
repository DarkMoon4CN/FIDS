using CATC.FIDS.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using CATC.FIDS.Models;
using System.Net.NetworkInformation;

namespace CATC.FIDS.Controllers
{
    /// <summary>
    /// 此控制器用于网内设备协调
    /// </summary>
    public class DeviceApiController : Controller
    {
        CATC_FIDS_DBEntities ef = new CATC_FIDS_DBEntities();
        /// <summary>
        /// 设备进行注册
        /// </summary>
        /// <returns></returns>
        public JsonResult RegisterDevice()
        {
            string bodyJson=DNTRequest.GetString("body");
            LoggerHelper.Info(bodyJson);
            var result = new ResultDto<string>();
            var req = JsonConvert.DeserializeObject<ResultDto<string>>(bodyJson);
            //1.如果库中有数据则返回最新数据信息
            if (string.IsNullOrWhiteSpace(bodyJson) || string.IsNullOrWhiteSpace(req.Data))
            {
                result.Status = 0;
                result.Message = "请求中显示器数量为 0 !";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            List<R_DisplayInfo> di = req.Data.DeserializeJSON<List<R_DisplayInfo>>();
            if (di == null || di.Count== 0)
            {
                result.Status = 1;
                result.Message = "设备集合为null 或者 count=0 !";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            string ip=di.FirstOrDefault().ip;
            var list=ef.R_DisplayInfo.Where(p => p.ip==ip).ToList();
            LoggerHelper.Info(list.Count+"____"+ip);
            //2.没有则插入后，返回数据信息
            if (list == null || list.Count==0) 
            {
                //执行新设备写入
                for (int i = 0; i < di.Count; i++)
                {
                    di[i].createTime = DateTime.Now;
                    di[i].url = Utils.Settings.Instance.GetSetting("SiteDomain") + Utils.Settings.Instance.GetSetting("DefaultPage");
                    ef.R_DisplayInfo.Add(di[i]);
                    ef.SaveChanges();
                }
            }
            
            //再次查询库中数据
            list = ef.R_DisplayInfo.Where(p => p.ip == ip).ToList();

            int d_id = list.FirstOrDefault().displayID;
            //如果状态监控表没有数据，则插入状态监控表
            var query1 = from R_D_M in ef.R_DeviceMonitoring where R_D_M.pk_DeviceID == d_id select R_D_M.pk_DeviceID;
            if (query1.Count<int>() == 0)
            {
                R_DeviceMonitoring objR_DeviceMonitoring = new R_DeviceMonitoring();
                objR_DeviceMonitoring.pk_DeviceID = d_id;
                objR_DeviceMonitoring.pk_DeviceStatusID = 1;
                objR_DeviceMonitoring.connectedTime = DateTime.Now;
                ef.R_DeviceMonitoring.Add(objR_DeviceMonitoring);
                ef.SaveChanges();
            }

            result.Status = 1;
            result.Message = "数据接收已完成！";
            result.Data = list.SerializeJSON();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PingClient()
        {
            var ipList= ef.R_DisplayInfo.GroupBy(g=>new {g.ip}).Select(s=>s.Key).ToList();
            List<object> deviceStatus = new List<object>();
            foreach (var item in ipList)
            {
                Ping ping = new Ping();
                string ip= item.ip;
                PingReply pingReply = ping.Send(ip);
                if (pingReply.Status == IPStatus.Success)
                {
                    string url = "http://" + ip + ":8888/Client/HelloWorld";
                    try
                    {
                        RequestHelper.SendHttpRequest(url, "POST", string.Empty);
                        deviceStatus.Add(new { ip = ip, Status = 1,Message="客户端在线" });
                    }
                    catch 
                    {
                        //客户端 window 服务没有开启
                        deviceStatus.Add(new { ip = ip, Status = 2, Message = "客户端服务未开启" });
                    }
                    
                }
                else
                {
                    deviceStatus.Add(new { ip = ip, Status = 0,Message="客户端离线" });
                }
            }
            ResultDto<List<object>> result = new ResultDto<List<object>>();
            result.Status = 1;
            result.Message = "查询设备状态已完成";
            result.Data = deviceStatus;
            return Json(deviceStatus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendCommand()
        {
            string ip = DNTRequest.GetString("ip");
            string json = DNTRequest.GetString("json");
            var displayJson = string.Empty;
            if (string.IsNullOrWhiteSpace(ip))
            {
                ip = "192.9.200.36";
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                displayJson = ef.R_DisplayInfo.Where(p => p.ip == ip).ToList().SerializeJSON();
                ResultDto<string> cmd = new ResultDto<string>();
                cmd.Status = 1;
                cmd.Message = "下发命令";
                cmd.Data = displayJson;
                json = new { Json = cmd.SerializeJSON() }.SerializeJSON();
            }
            ResultDto<string> result = new ResultDto<string>();
            try
            {
                string resultStr = RequestHelper.SendHttpRequest("http://" + ip + ":8888/Client/ExecuteCommand", "POST", json);
                result.Status = 1;
                result.Message = "更新客户端设备已完成！";
                result.Data = resultStr;
            }
            catch (Exception ex)
            {
                result.Status = 0;
                result.Message = "远程服务未打开："+ip;
                result.Data = json;
                LoggerHelper.Error(ex.Message);
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}
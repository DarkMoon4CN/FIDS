using CATC.FIDS.Utils;
using CATC.FIDS.Weather.Factory;
using CATC.FIDS.Weather.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CATC.FIDS.Weather.Controllers
{
    [ControllerAuth]
    public class WeatherApiController : Controller
    {
        public string _PATH=Utils.Settings.Instance.GetSetting("WeatherPath");//气象文件目录
        public int _VALIDTIME=Utils.Settings.Instance.GetSetting("ValidTime").ToInt();//有效时间
        // GET: Weather
        public JsonResult GetSASP()
        {
            string folderName = "SASP";
            var result = new ResultDto<dynamic>();
            string AIRPORT_ICAO = DNTRequest.GetString("icao");
            string serverTime =DNTRequest.GetString("serverTime");
            try
            {
                DateTime sTime = string.IsNullOrWhiteSpace(serverTime) == true 
                    ? DateTime.Now.AddMinutes(-_VALIDTIME)
                    : serverTime.ToDateTime().AddMinutes(-_VALIDTIME);
                //遍历目录查找出符合要求的数据文件名
                string sasp_path = _PATH + folderName;
                var files = GetLastWriteFiles(sasp_path, AIRPORT_ICAO, 5, sTime);
                //无可用气象数据
                if (files.Length == 0)
                {
                    result.Status = 0;
                    result.Message = "No Available Data";
                    result.Data = null;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //进行xml数据解析
                var tempFile = files[0];
                string[] fileSplit = tempFile.Split('_');
                Utils.XmlHelper xh = new XmlHelper();
                xh.Load(tempFile);
                var child = xh.SelectSingleNode("SASP").OuterXml;
                var model = child.DeserializeXML<SASP>();
                model.FileName = fileSplit[2];
                model.SearchTime = DateTime.Now.ToString();
                result.Status = 1;
                result.Message = "is success";
                result.Data = model;
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/WeatherApi/GetSASP error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSTART()
        {
            string folderName = "STAR";
            var result = new ResultDto<dynamic>();
            string AIRPORT_ICAO = DNTRequest.GetString("icao"); //无效参数
            string serverTime = DNTRequest.GetString("serverTime");
            try
            {
                DateTime sTime = string.IsNullOrWhiteSpace(serverTime) == true 
                    ? DateTime.Now.AddMinutes(-_VALIDTIME) 
                    : serverTime.ToDateTime().AddMinutes(-_VALIDTIME);
                string start_path = _PATH + folderName;

                //文件命名规则 STAR_201703202304,keyword不是使用
                var files = GetLastWriteFiles(start_path, string.Empty, 5, sTime);
                if (files.Length == 0)
                {
                    result.Status = 0;
                    result.Message = "No Available Data";
                    result.Data = null;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var tempFile = files[0];
                string[] fileSplit = tempFile.Split('_');

                if (!Directory.Exists(Server.MapPath("/Images")))
                {
                    Directory.CreateDirectory(Server.MapPath("/Images"));
                }
                string fileName = "/Images/" + fileSplit[1];
                string savePath = Server.MapPath(fileName);
                if (System.IO.File.Exists(savePath) ==false)
                {
                    System.IO.File.Copy(tempFile, savePath);
                }
                result.Status = 1;
                result.Message = "is success";
                result.Data = "/Images/" + fileSplit[1];
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/WeatherApi/GetStart error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 遍历后按最后的写入时间进行排序
        /// </summary>
        /// <param name="path"></param>
        /// <param name="keyword"></param>
        /// <param name="count"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private string[] GetLastWriteFiles(string path,string keyword,int count,DateTime date)
        {
            var query = (from f in Directory.GetFiles(path)
                         let fi = new FileInfo(f)
                         where fi.FullName.Contains(keyword) && fi.LastWriteTime >= date
                         orderby fi.LastWriteTime descending
                         select fi.FullName).Take(count);
            return query.ToArray();

        }
    }
}
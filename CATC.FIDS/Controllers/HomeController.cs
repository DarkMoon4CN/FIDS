using CATC.FIDS.Factory;
using CATC.FIDS.Model;
using CATC.FIDS.Models;
using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace CATC.FIDS.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            string actionKey = DNTRequest.GetString("ak");
            var isShowLeft = true;
            if (string.IsNullOrWhiteSpace(actionKey))
            {
                actionKey = "DepartureFlight";
            }
            List<DailySchedule_ExtFileds> fileds = null;
            string sortString = string.Empty;
            var template = ef.Template.Where(p => p.name == actionKey).ToList().FirstOrDefault();
            List<TemplateTD> tds = null;
            string definition = string.Empty;
            if (actionKey == "DepartureFlight")
            {
                fileds = GetDepColumns();
            }
            else if (actionKey == "ArrivalFlight")
            {
                fileds = GetArrColumns();
            }
            else if (actionKey == "BaggageClaim")
            {
                isShowLeft = false;
                fileds = GetBaggageClaimColumns();
            }
            else if (actionKey == "BSArrivalAndArrival")
            {
                fileds = GetArrColumns();
            }
            else if (actionKey == "BSDepartureAndDeparture")
            {
                fileds = GetBSDepartureAndDepartureColumns();
            }
            else
            {
                isShowLeft = false;
            }

            //当页面所有元素没有初始化的时候进行初始化操作
            if (template == null)
            {
                string description = string.Empty;
                definition = TemplateInitEntrance(actionKey, ref description);
                template = new Template()
                {
                    code = actionKey,
                    name = actionKey,
                    description = string.IsNullOrWhiteSpace(description) == true ? actionKey : description,
                    creator = actionKey,
                    definition = definition,
                    modifier = actionKey,
                    creationtime = DateTime.Now,
                    modifytime = DateTime.Now,
                    isValid = 1
                };
                ef.Template.Add(template);
                ef.SaveChanges();
            }
            else if (string.IsNullOrWhiteSpace(template.definition))
            {
                string description = string.Empty;
                definition = TemplateInitEntrance(actionKey, ref description);
                template.definition = definition;
                ef.SaveChanges();
            }

            if (actionKey == "DepartureFlight" 
                || actionKey == "ArrivalFlight" 
                || actionKey == "BaggageClaim" 
                || actionKey == "BSArrivalAndArrival"
                || actionKey == "BSDepartureAndDeparture")
            {
                string tableStr = string.Empty;
                AnalysisTableXml(actionKey, template.definition, fileds, out tableStr, out tds);
                if (tds != null)
                {
                    for (int i = 0; i < tds.Count; i++)
                    {
                        if (tds[i].FiledID == 10)
                        {
                            continue;
                        }
                        sortString += tds[i].FiledID + ",";
                    }
                }
            }
            else if (actionKey == "Checkins")
            {

            }

            ViewBag.isShowLeft = isShowLeft;
            ViewBag.sf = tds;//Selected Fildes
            ViewBag.sortString = sortString.TrimEnd(',');
            ViewBag.fildes = fileds;
            ViewBag.ak = actionKey;
            return View();
        }

        public ActionResult NewLayout()
        {
            return View();
        }

        public ActionResult EditMain()
        {
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登陆请求
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="user_pwd"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoginSys(string user_name, string user_pwd)
        {
            return Json(new { resault = "1" });
            //var login_model = ef.Sy_UserInfo.Where(p => p.login_id == user_name).FirstOrDefault();
            //if (login_model == null)
            //{
            //    return Json(new { resault = "0" });
            //}
            //if (login_model.login_password != user_pwd)
            //{
            //    return Json(new { resault = "0" });
            //}
            //return RedirectToAction("EditMain", "Home");
        }

        /// <summary>
        /// 新增用户/修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser()
        {
            var result = new ResultDto<dynamic>();
            string userID = DNTRequest.GetString("userID");//用户ID
            string login_id = DNTRequest.GetString("login_id");//用户账号
            string pwd = DNTRequest.GetString("login_password");//密码
            string password = Utils.Utils.MD5(pwd);
            string pwd_return = DNTRequest.GetString("login_password_wage");//重复密码
            string password_w = Utils.Utils.MD5(pwd_return);
            string remark = DNTRequest.GetString("login_remark");
            if (password != password_w)
            {
                return Json(new { resault = "0" });//两次密码输入不一致
            }
            try
            {
                if (!string.IsNullOrWhiteSpace(userID))//修改密码
                {
                    int user_id = userID.ToInt();
                    var model = ef.Sy_UserInfo.Where(p => p.userID == user_id).FirstOrDefault();
                    if (model != null)
                    {
                        model.login_id = login_id;
                        model.login_password = password;
                        model.login_password_wage = password_w;
                        ef.SaveChanges();
                    }
                    else
                    {
                        result.Status = 0;
                        result.Message = "修改密码失败，用户不存在！";
                    }
                }
                else //新增用户
                {
                    var model = new Sy_UserInfo();
                    model.login_status = "1";
                    model.login_id = login_id;
                    model.login_password = password;
                    model.login_password_wage = password_w;
                    model.login_remark = remark;
                    ef.Sy_UserInfo.Add(model);
                    ef.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error("/Home/AddUser error:" + ex.Message);
                result.Status = 0;
                result.Message = "is error";
                result.Data = null;
            }
            return null;
        }


    }
}
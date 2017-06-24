using CATC.FIDS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
namespace CATC.FIDS.Factory
{
    public class ControllerAuth : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            var result = new ResultDto<dynamic>();
            try
            {
                //安全验证模块
                var request=actionContext.HttpContext.Request;
            }
            catch (Exception ex)
            {
                result.Status = 1000002;
                result.Message = "验证签名失败";
                result.Data = null;

                //日志记录
                LoggerHelper.Info(result.Status +":"+ex.Message);
                //提交空页面
                actionContext.HttpContext.Response.Write(result.SerializeJSON());
                actionContext.HttpContext.Response.End();
                actionContext.Result = new EmptyResult();
            }
            actionContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            base.OnActionExecuting(actionContext);
        }
    }
}
using System;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CATC.FIDS.Models
{
    public class LogOn
    {
        [Required()]
        [Display(Name = "User ID")]
        public string user_name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string user_pass { get; set; }

        [Display(Name = "Minutes after auto logout")]
        public string login_timeout { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }

        /// <summary>
        /// 是否是263跳转验证
        /// </summary>
        public bool IsValidate = false;
    }
}

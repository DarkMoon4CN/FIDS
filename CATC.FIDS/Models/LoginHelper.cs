using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MH.Common.Data.Entity;
using System.Data;
using MH.Common;
using MH.Common.Data;
using System.Data.SqlClient;

namespace CATC.FIDS.Models
{
    public class LoginHelper
    {
        //用户模块缓存KEY
        public static string sy_helpPage_ds_cachekey = "sy_helpPage_cache_" + UserInfo.emp_no;
        //模块导航
        public static string sy_pgm_nav_cachekey = "sy_pgm_nav_cache";
        //模块操作
        public static string sy_Operation_cachekey = "sy_Operation_cache";

        /// <summary>
        /// 当前登陆用户
        /// </summary>
        public static UserLoginInfo UserInfo
        {
            get
            {
                if (HttpContext.Current.Session["UserInfo"] == null)
                {
                    HttpContext.Current.Session["UserInfo"] = new UserLoginInfo();
                }
                return HttpContext.Current.Session["UserInfo"] as UserLoginInfo;
            }
            set
            {
                HttpContext.Current.Session["UserInfo"] = value;
            }
        }

        /// <summary>
        /// 用户模块列表
        /// </summary>
        public static DataSet sy_pgm_ds
        {
            get
            {
                if (DataCache.Contains("sy_pgm_ds_cache_" + UserInfo.userID))
                {
                    return DataCache.GetCache<DataSet>("sy_pgm_ds_cache_" + UserInfo.userID);
                }
                else
                {
                    //执行存储过程 
                    DataSet _sy_ds = new DataSet();
                    SqlParameter[] param = new SqlParameter[] { new SqlParameter("@in_aut_user", UserInfo.userID), new SqlParameter("@in_include_fav", 1) };
                    _sy_ds = SqlHelper.ExecuteDataset("sy102", Parameters: param);
                    if (_sy_ds.Tables.Count > 0)
                    {
                        DataCache.SetCache("sy_pgm_ds_cache_" + UserInfo.userID, _sy_ds, 300);
                        return _sy_ds;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 清除用户模块列表缓存
        /// </summary>
        public static void Remove_Sy_ds_cache()
        {
            DataCache.RemoveCache("sy_pgm_ds_cache_" + UserInfo.userID);
        }

        /// <summary>
        ///模块导航列表
        /// </summary>
        public static Dictionary<string, string> sy_pgm_nav
        {
            get
            {
                if (DataCache.Contains(sy_pgm_nav_cachekey))
                {
                    return DataCache.GetCache<Dictionary<string, string>>(sy_pgm_nav_cachekey);
                }
                else
                {
                    Dictionary<string, string> dic_nav = new Dictionary<string, string>();
                    using (SqlDataReader reader = SqlHelper.ExecuteReader("sy_get_nav"))
                    {
                        while (reader.Read())
                        {
                            dic_nav.Add(reader["pgm_code"].ToString().Trim(), reader["pgm_navigation"].ToString().Trim());
                        }
                    }
                    if (dic_nav.Count > 0)
                    {
                        DataCache.SetCache(sy_pgm_nav_cachekey, dic_nav, 3600);
                        return dic_nav;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
        }

        public static void Remove_sy_pgm_nav()
        {
            DataCache.RemoveCache(sy_pgm_nav_cachekey);
        }

        
        public static void Remove_sy_Operation()
        {
            DataCache.RemoveCache(sy_Operation_cachekey);
        }
    }

    [Serializable]
    public class UserLoginInfo
    {
        public int? init_seq { get; set; }
        public string login_id { get; set; }
        public string emp_no { get; set; }
        public string user_cname { get; set; }
        public string user_ename { get; set; }
       
        public string login_mode { get; set; }
        public string login_timeout { get; set; }
        [NotMapped]
        public DateTime login_time { get; set; }
        [NotMapped]
        public DateTime active_time { get; set; }
        [NotMapped]
        public string sy_cur_pay_mth { get; set; }
        [NotMapped]
        public string sy_cur_mth_start { get; set; }
        [NotMapped]
        public string sy_cur_mth_end { get; set; }
        //新增
        public string userID { get; set; }
    }
}

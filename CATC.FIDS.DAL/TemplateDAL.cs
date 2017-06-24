using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CATC.FIDS.Utils;
using Dapper;

namespace CATC.FIDS.DAL
{

    /// <summary>
    /// 调用Dapper 示例
    /// </summary>
    public class TemplateDAL
    {
        public List<dynamic> GetTemplateByID(int templateID)
        {
            IList<dynamic> items = new List<dynamic>();
            try
            {
                string sql = " SELECT * FROM Template WHERE  templateID='{0}' ";
                sql = string.Format(sql, templateID);
                using (SqlConnection con = new SqlConnection(SQlHelper.MyConnectStr))
                {
                    return con.Query<dynamic>(sql, new { TemplateID = templateID }).ToList();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Error(ex.Message);
                return null;
            }
        }
    }
}

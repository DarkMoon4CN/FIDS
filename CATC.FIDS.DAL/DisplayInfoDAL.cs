using CATC.FIDS.DAL.DataAccess;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.DAL
{
    public class DisplayInfoDAL
    {
        private static Database dbr = SqlServerData.Writer;
        private static Database dbw = SqlServerData.Reader;

        public List<dynamic> GetDisplayInfoByID(string displayID)
        {
            var sql = " SELECT * FROM Template WHERE  displayID='{0}' ";
            sql = string.Format(sql, displayID);
            var cmd = dbr.GetSqlStringCommand(sql);
            var list = new List<dynamic>();

            using (var read = dbr.ExecuteReader(CommandType.Text, sql))
            {
                while (read.Read())
                {
                    list.Add(this.RecoverModel(read));
                }
            }
            return list;
        }
        private dynamic RecoverModel(IDataReader dataReader)
        {
            //var model = new Model();
            //model.DisplayID = int.Parse(dataReader["DisplayID"].ToInt());
            return null;
        }
    }
}

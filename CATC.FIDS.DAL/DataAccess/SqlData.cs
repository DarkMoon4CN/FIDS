using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CATC.FIDS.DAL.DataAccess
{
    public class SqlServerData
    {
        public static Database Writer
        {
            get
            {
                return DatabaseFactory.CreateDatabase("Sql_CATC_FIDS_DB"); ;
            }
        }
        public static Database Reader
        {
            get
            {
                return DatabaseFactory.CreateDatabase("Sql_CATC_FIDS_DB"); ;
            }
        }
    }
}

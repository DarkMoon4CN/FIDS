using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{

    public enum OperationLogEnum
    {
        用户日志=1,
        接口日志=2,
        系统日志=3
    }


    public enum OperationLogModuleEnum
    {
        航班数据编辑=101,
        航班数据导入=102,
        航班数据增加=103,
        航班数据删除=104,

        基础数据编辑=001,
        基础数据导入=002,
        基础数据增加=003,
        基础数据删除=004,

        模板编辑 =201,
        模板重置 =202,

        设备编辑=301,
        设备启动=302,
        设备关闭=302,
        设备重启=304,
        设备启用=305,
        设备停用=306,

        模板分配新增=401,
        模板分配编辑=402,
        模板分配删除=403,
        模板分配发布=404,

        航班资源分配编辑=601,
        航班资源分配导入 = 602,
        航班资源分配增加 = 603,
        航班资源分配删除 = 604,

        日志写入 =501,
    }

    public enum OperationLogLevelEnum
    {
        Info=1,
        Warning=2,
        Error=3
    }

}

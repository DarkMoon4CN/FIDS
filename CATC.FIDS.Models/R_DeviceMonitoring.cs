//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CATC.FIDS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class R_DeviceMonitoring
    {
        public int ID { get; set; }
        public int pk_DeviceID { get; set; }
        public Nullable<int> pk_DeviceStatusID { get; set; }
        public Nullable<System.DateTime> connectedTime { get; set; }
        public string ExceptionMsg { get; set; }
    }
}

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
    
    public partial class ResourceAllocation
    {
        public int resourceAllocationID { get; set; }
        public string name { get; set; }
        public string resourceID { get; set; }
        public System.DateTime allocationStart { get; set; }
        public System.DateTime allocatonEnd { get; set; }
        public string category { get; set; }
        public System.DateTime preAllocationBufferDuration { get; set; }
        public System.DateTime postAllocationBufferDurationTime { get; set; }
    }
}
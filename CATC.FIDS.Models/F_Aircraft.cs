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
    
    public partial class F_Aircraft
    {
        public int ID { get; set; }
        public string AC_REG_NO { get; set; }
        public string AC_TYPE_IATA { get; set; }
        public string AIRLINE_IATA { get; set; }
        public Nullable<int> SUBAIRLINE_ID { get; set; }
        public string FLG_DELETED { get; set; }
        public string EXT_CODE { get; set; }
    }
}

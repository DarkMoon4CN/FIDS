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
    
    public partial class EventMessage
    {
        public int emid { get; set; }
        public System.DateTime strartTime { get; set; }
        public System.DateTime endTime { get; set; }
        public System.DateTime createTime { get; set; }
        public string creator { get; set; }
        public int level { get; set; }
        public int dataType { get; set; }
        public string chineseContent { get; set; }
        public string englishContent { get; set; }
        public string otherContent { get; set; }
        public string actionKeys { get; set; }
        public string fontColor { get; set; }
    }
}
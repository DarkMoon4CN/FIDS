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
    
    public partial class R_DisplayInfo
    {
        public int displayID { get; set; }
        public string ip { get; set; }
        public string displayName { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int index { get; set; }
        public int isPrimary { get; set; }
        public string url { get; set; }
        public System.DateTime createTime { get; set; }
        public Nullable<int> displayLuminance { get; set; }
        public Nullable<int> displayState { get; set; }
        public Nullable<int> displayGroup { get; set; }
        public string displayMAC { get; set; }
        public string displayMark { get; set; }
        public int displayConnectTime { get; set; }
    }
}

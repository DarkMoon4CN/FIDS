using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CATC.FIDS.Models
{
    public class Filght_DynamicModels
    {
        public int FDID { get; set; }
        public string Status_Code { get; set; }
        public string Status_CHINESE_NAME { get; set; }
        public string Status_ENGLISH_NAME { get; set; }
        public string Status_Color { get; set; }
        public string CheckIns_Display_Symbol { get; set; }
        public string Gate_Display_Symbol { get; set; }
    }
}
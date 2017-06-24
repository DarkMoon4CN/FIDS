using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CATC.FIDS.Models
{
    public class TemplateModels
    {
        public int templateID { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public object value { get; set; }
    }
}
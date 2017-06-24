using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CATC.FIDS.Models
{
    public class TemplateDisplayModels
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public dynamic DisplayInfos { get; set; }
    }

    public class GroupTemplateDisplayModels
    {
        public int GroupID { get; set; }

        public string GroupName { get; set; }

        public string TemplateIDs { get; set; }
        public dynamic DisplayInfos { get; set; }
    }

    public class SendTemplateDisplayInfoMdoels
    {
        public int ID { get; set; }
        public int TemplateID { get; set; }
        public int DisplayID { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public string Weeks { get; set; }
        public short IsCover { get; set; }
        public short IsAdvert { get; set; }
        public string AdvertUrl { get; set; }
        public short Sort { get; set; }
        public Nullable<System.DateTime> SpaceStartTime { get; set; }
        public Nullable<System.DateTime> SpaceEndTime { get; set; }
        public int IntervalSecond { get; set; }
        public int Index { get; set; }
        public string PageName { get; set; }
        public int TopScreenCode { get; set; }
        public int Count { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string TemplateName { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
    }
}
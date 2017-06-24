using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CATC.FIDS.Model
{
    public class TemplateTable:TemplateBase
    {

        public int Border { get; set; }
        private string _color { get; set; }
        public List<TemplateTD> TDs { get; set; }
        public string Color
        {
            set { _color = value; }
            get
            {
                if (string.IsNullOrEmpty(_color))
                {
                    return "#000";
                }
                return _color;
            }
        }
    }
    public class TemplateTD:TemplateBase
    {
        public int FiledID { get; set; }
        public int Index { get; set; }

        public string Remarks { get; set; }
    }
}

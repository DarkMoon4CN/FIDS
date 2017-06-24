using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class TemplateTime:TemplateBase
    {
        public int TimeType { get; set; }
        public bool IsBold { get; set; }
        private string _color;
        private string _fontSize;
        public string FontSize
        {

            set { _fontSize = value; }
            get
            {
                if (string.IsNullOrEmpty(_fontSize))
                {
                    return "16px";
                }
                return _fontSize;
            }
        }
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
}

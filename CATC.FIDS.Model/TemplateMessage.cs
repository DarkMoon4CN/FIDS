using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class TemplateMessage: TemplateBase
    {
        public string Content { get; set; }
        public string OtherContent { get; set; }
        private string _color;
        public string Color
        {
            set { _color = value; }
            get
            {
                if (string.IsNullOrEmpty(_color))
                {
                    return "#fffs";
                }
                return _color;
            }
        }
    }
}

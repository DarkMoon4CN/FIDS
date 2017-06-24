using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    /// <summary>
    ///  用于解析text元素
    /// </summary>
    public class TemplateText:TemplateBase
    {
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
        private string _color;
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

        public string Content { get; set; }
    }
}

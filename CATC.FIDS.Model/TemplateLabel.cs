using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class TemplateLabel:TemplateBase
    {
       
        public string Content { get; set; }
        public string OtherContent { get; set; }
        public bool IsBold { get; set; }
        public bool IsSETime { get; set; }
        private string _fontSize;
        private string _color;
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

        /// <summary>
        /// 背景色
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// 背景色透明度
        /// </summary>
        public string Opacity { get; set; }
    }
}

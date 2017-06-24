using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class TemplateBgColor : TemplateBase
    {

        /// <summary> 
        /// 背景色 根据 BgType 判定是否有值
        /// </summary>
        public string BgColor { get; set; }

        /// <summary>
        /// 背景色透明度
        /// </summary>
        public string Opacity { get; set; }

        /// <summary>
        /// 背景图片 根据 BgType 判定是否有值
        /// </summary>
        public string BgImageUrl { get; set; }

        /// <summary>
        /// 背景类型 1.图片 2.背景色（使用 Opacity 字段）
        /// </summary>
        public int BgType { get; set; }


    }
}

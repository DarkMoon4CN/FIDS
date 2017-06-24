using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class TemplateContent:TemplateBase
    {

        /// <summary>
        /// 类型: 1.默认文本，2.pdf，3.图片
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 包含html标签的文本
        /// </summary>
        public string Content { get; set; }
    }
}

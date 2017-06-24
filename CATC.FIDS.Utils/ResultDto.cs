using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Utils
{
    public class ResultDto<T>
    {
        public ResultDto() { }
        /// <summary>
        /// 1.表示成功，其他失败，自定义
        /// </summary>
        public int Status { get; set; }
        public string Message { get; set; }

        public T Data { get; set; }
    }
}

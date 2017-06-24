using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace CATC.FIDS.Utils
{
    /// <summary>
    /// 此类做时间分配的公共类
    /// </summary>
    public class TimeAllocation
    {
        /// <summary>
        /// 获取已经被占用时间列表
        /// </summary>
        /// <param name="startTime">开始年月日 时分秒</param>
        /// <param name="endTime">结束年月日 时分秒</param>
        /// <param name="spaceStartTime">当天开始 时分秒</param>
        /// <param name="spaceEndTime">当天结束 时分秒</param>
        /// <param name="url">地址</param>
        /// <param name="index">展示的顺序</param>
        /// <returns></returns>
        public static List<DisplayUsedTime> GetUsedTime(DateTime startTime, DateTime endTime, DateTime spaceStartTime, DateTime spaceEndTime,string url,int index=1)
        {
            List<DisplayUsedTime> uList = new List<DisplayUsedTime>();
            var minTime =(startTime.ToShortDateString() + " " + spaceStartTime.ToShortTimeString()).ToDateTime();
            var maxTime = (endTime.ToShortDateString() + " " + spaceEndTime.ToShortTimeString()).ToDateTime();
            //查找出被占用的列表
            while (minTime < maxTime)
            {
                DisplayUsedTime uTime = new DisplayUsedTime();
                uTime.StartTime = minTime;
                uTime.EndTime =(uTime.StartTime.ToShortDateString() + " " + spaceEndTime.ToShortTimeString()).ToDateTime();
                uTime.Url= url;
                uList.Add(uTime);
                minTime=minTime.AddDays(1);
            }
            return uList;
        }
    }


    public class DisplayUsedTime
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Url { get; set; }
    }
}

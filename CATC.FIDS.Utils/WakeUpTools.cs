using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace CATC.FIDS.Utils
{
    public class WakeUpTools
    {
        //通过正则表达式设定MAC地址筛选标准，关于正则表达式请自行百度
        const string macCheckRegexString = @"^([0-9a-fA-F]{2})(([/\s:-][0-9a-fA-F]{2}){5})$";

        private static readonly Regex MacCheckRegex = new Regex(macCheckRegexString);

        //唤醒主要逻辑方法
        public static bool WakeUp(string mac)
        {
            //查看该MAC地址是否匹配正则表达式定义，（mac，0）前一个参数是指mac地址，后一个是从指定位置开始查询，0即从头开始
            if (MacCheckRegex.IsMatch(mac, 0))
            {
                byte[] macByte = FormatMac(mac);
                WakeUpCore(macByte);
                return true;
            }

            return false;

        }

        private static void WakeUpCore(byte[] mac)
        {
            //发送方法是通过UDP
            UdpClient client = new UdpClient();
            //Broadcast内容为：255,255,255,255.广播形式，所以不需要IP
            client.Connect(System.Net.IPAddress.Broadcast, 50000);
            //下方为发送内容的编制，6遍“FF”+17遍mac的byte类型字节。
            byte[] packet = new byte[17 * 6];
            for (int i = 0; i < 6; i++)
                packet[i] = 0xFF;
            for (int i = 1; i <= 16; i++)
                for (int j = 0; j < 6; j++)
                    packet[i * 6 + j] = mac[j];
            //唤醒动作
            int result = client.Send(packet, packet.Length);
        }

        private static byte[] FormatMac(string macInput)
        {
            byte[] mac = new byte[6];

            string str = macInput;
            //消除MAC地址中的“-”符号
            string[] sArray = str.Split(':');


            //mac地址从string转换成byte
            for (var i = 0; i < 6; i++)
            {
                var byteValue = Convert.ToByte(sArray[i], 16);
                mac[i] = byteValue;
            }

            return mac;
        }
    }
}

using HidSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidManager
{
    /// <summary>
    /// HidStream扩展方法
    /// </summary>
    public static class HidStreamExtensions
    {
        /// <summary>
        /// 发送报告
        /// </summary>
        /// <param name="stream">HidStream对象</param>
        /// <param name="reportId">报告ID</param>
        /// <param name="data">报告数据</param>
        public static void SendReport(this HidStream stream, byte reportId, byte[] data)
        {
            byte[] buffer = new byte[data.Length + 1];
            buffer[0] = reportId;
            Array.Copy(data, 0, buffer, 1, data.Length);
            stream.Write(buffer);
        }

        /// <summary>
        /// 发送特征报告
        /// </summary>
        /// <param name="stream">HidStream对象</param>
        /// <param name="reportId">报告ID</param>
        /// <param name="data">报告数据</param>
        public static void SendFeatureReport(this HidStream stream, byte reportId, byte[] data)
        {
            byte[] buffer = new byte[data.Length + 1];
            buffer[0] = reportId;
            Array.Copy(data, 0, buffer, 1, data.Length);
            stream.SetFeature(buffer);
        }
    }

    /// <summary>
    /// HidDevice扩展方法
    /// </summary>
    public static class HidDeviceExtensions
    {
        /// <summary>
        /// 获取设备的Usage信息
        /// </summary>
        /// <param name="device">HidDevice对象</param>
        /// <returns>UsageInfo对象</returns>
        public static UsageInfo GetUsageInfo(this HidDevice device)
        {
            try
            {
                var descriptor = device.GetReportDescriptor();
                var a = descriptor.DeviceItems[0].Usages.GetAllValues().First();

                return new UsageInfo
                {
                    UsagePage = (ushort)(a >> 16),  // 取高16位    UsagePage                : 0xFF00 (Vendor Defined)
                    Usage = (ushort)(a & 0xffff),   // 取低16位    Usage                    : 0x00 (unk)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Usage信息
    /// </summary>
    public class UsageInfo
    {
        /// <summary>
        /// UsagePage属性
        /// </summary>
        public ushort UsagePage { get; set; }
        /// <summary>
        /// Usage属性
        /// </summary>
        public ushort Usage { get; set; }
    }
}

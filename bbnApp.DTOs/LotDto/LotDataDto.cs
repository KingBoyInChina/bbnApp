using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.LotDto
{
    /// <summary>
    /// 物联网数据采集表结构体
    /// </summary>
    public class LotDataDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 数据ID
        /// </summary>
        public int DataId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 网关ID
        /// </summary>
        public string GetWayId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 传感器地址
        /// </summary>
        public string SensorAddr { get; set; } = string.Empty;

        /// <summary>
        /// 编号,用于区分是否同一组数据
        /// </summary>
        public string GroupNumber { get; set; } = string.Empty;

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 接收到的数据
        /// </summary>
        public decimal ReciveValue { get; set; }

        /// <summary>
        /// 接收时间
        /// </summary>
        public string ReciveDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        /// <summary>
        /// 原始数据
        /// </summary>
        public string OriginalData { get; set; } = string.Empty;

        /// <summary>
        /// 特殊数据
        /// </summary>
        public string? MeatData { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
    }
}

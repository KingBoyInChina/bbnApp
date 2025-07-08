using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.LotDto
{
    /// <summary>
    /// 设备动作结构体
    /// </summary>
    public class LotDeviceActionDto
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
        /// 动作ID
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// 编号,区分是否手动触发的同组操作
        /// </summary>
        public string? GroupNumber { get; set; } = string.Empty;

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
        /// 动作类型
        /// </summary>
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// 记录时间
        /// </summary>
        public string ActionDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

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

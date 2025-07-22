using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 设备命令结构体
    /// </summary>
    public class DeviceCommandDto
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
        /// 指令ID
        /// </summary>
        public string CommandId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 命令名称
        /// </summary>
        public string CommandName { get; set; } = string.Empty;

        /// <summary>
        /// 命令代码
        /// </summary>
        public string CommandCode { get; set; } = string.Empty;

        /// <summary>
        /// 硬件通信协议
        /// </summary>
        public string HardwareCP { get; set; } = string.Empty;
        /// <summary>
        /// 硬件通信协议名称
        /// </summary>
        public string HardwareCPName { get; set; } = string.Empty;

        /// <summary>
        /// 应用通信协议
        /// </summary>
        public string ApplicationCP { get; set; } = string.Empty;
        /// <summary>
        /// 应用通信协议名称
        /// </summary>
        public string ApplicationCPName { get; set; } = string.Empty;

        /// <summary>
        /// 设备地址
        /// </summary>
        public string? DeviceAddr { get; set; } = string.Empty;

        /// <summary>
        /// 功能码
        /// </summary>
        public string? FunctionCode { get; set; } = string.Empty;

        /// <summary>
        /// 起始地址,大小端
        /// </summary>
        public string? StartAddr { get; set; } = string.Empty;

        /// <summary>
        /// 寄存器数量
        /// </summary>
        public string? RegCount { get; set; } = string.Empty;

        /// <summary>
        /// 校验码
        /// </summary>
        public bool CRC { get; set; } = false;

        /// <summary>
        /// 手动构造命令
        /// </summary>
        public string? CommandInfo { get; set; } = string.Empty;

        /// <summary>
        /// 命令参数说明
        /// </summary>
        public string? CommandDescription { get; set; } = string.Empty;

        /// <summary>
        /// 停用
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 锁定时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 锁定原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
    }
    /// <summary>
    /// 根据设备ID获取设备命令列表请求参数
    /// </summary>
    public class DeviceCommandListRequestDto
    {
        public string DeviceId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 根据设备ID获取设备命令列表响应参数
    /// </summary>
    public class DeviceCommandListResponseDto
    {
        public bool Code { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public List<DeviceCommandDto> DeviceCommands { get; set; } = new List<DeviceCommandDto>();
    }
    /// <summary>
    /// 设备命令保存请求参数
    /// </summary>
    public class DeviceCommandSaveRequestDto
    {
        public DeviceCommandDto DeviceCommand { get; set; } = new DeviceCommandDto();
    }
    /// <summary>
    /// 设备命令保存响应参数
    /// </summary>
    public class DeviceCommandSaveResponseDto
    {
        public bool Code { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public DeviceCommandDto DeviceCommand { get; set; } = new DeviceCommandDto();
    }
    /// <summary>
    /// 设备命令状态变更请求参数
    /// </summary>
    public class DeviceCommandStateRequestDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string CommandId { get; set; } = string.Empty;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 设备命令状态变更响应参数
    /// </summary>
    public class DeviceCommandStateResponseDto
    {
        public bool Code { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public DeviceCommandDto DeviceCommand { get; set; } = new DeviceCommandDto();
    }
}
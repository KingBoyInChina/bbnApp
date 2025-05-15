using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.BusinessDto
{
    /// <summary>
    /// 文件上传
    /// </summary>
    public class UploadFileItemDto
    {
        /// <summary>
        /// 对应表
        /// </summary>
        public string LinkTable { get; set; } = string.Empty;
        /// <summary>
        /// 对应表主键
        /// </summary>
        public string LinkKey { get; set; } = string.Empty;
        /// <summary>
        /// 备注
        /// </summary>
        public string ReMarks { get; set; } = string.Empty;
        /// <summary>
        /// 文件
        /// </summary>
        public List<FileItemsDto> Files { get; set; } =new List<FileItemsDto>();
    }
    /// <summary>
    /// 
    /// </summary>
    public class FileItemsDto
    {
        /// <summary>
        /// 二进制文件
        /// </summary>
        public byte[] FileByes { get; set; } = Array.Empty<byte>();
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileExt { get; set; } = string.Empty;
        /// <summary>
        /// 文件ID
        /// </summary>
        public string? FileId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 文件上传请求
    /// </summary>
    public class UploadFileRequestDto { 
        /// <summary>
        /// 
        /// </summary>
        public UploadFileItemDto Item = new UploadFileItemDto();
    }
    /// <summary>
    /// 文件上传响应
    /// </summary>
    public class UploadFileResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public UploadFileItemDto Item = new UploadFileItemDto();
    }
    /// <summary>
    /// 上传文件状态变更请求
    /// </summary>
    public class UploadFileStateRequestDto { 
        /// <summary>
        /// 文件ID
        /// </summary>
        public string FileId { get; set; } = string.Empty;
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 锁定原因-锁定操作时需要
        /// </summary>
        public string LockReason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 上传文件状态变更响应
    /// </summary>
    public class UploadFileStateResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public FileItemsDto Item = new FileItemsDto();
    }
    /// <summary>
    /// 上传文件读取变更请求-通过key+table读取
    /// </summary>
    public class UploadFileReadRequestDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public string LinkKey { get; set; } = string.Empty;
        /// <summary>
        /// 操作类型
        /// </summary>
        public string LinkTable { get; set; } = string.Empty;
    }
    /// <summary>
    /// 上传文件状态变更响应
    /// </summary>
    public class UploadFileReadResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public UploadFileItemDto Item = new UploadFileItemDto();
    }
}

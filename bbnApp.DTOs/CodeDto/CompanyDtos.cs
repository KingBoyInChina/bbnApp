using bbnApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 请求对象
    /// </summary>
    public class CompanyRequestDto
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string? Version { get; set; }
        /// <summary>
        /// 默认机构id
        /// </summary>
        public string? CompanyId { get; set; }
    }
    /// <summary>
    /// 机构返回信息
    /// </summary>
    public class CompanyResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 机构清单
        /// </summary>
        public List<CompanyItemDto> CompanyItems { get; set; }
    }
}


namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 应用请求对象
    /// </summary>
    public class AppsRequestDto
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; }
    }
    /// <summary>
    /// 机构返回信息
    /// </summary>
    public class AppsResponseDto
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
        public List<TopMenuItemDto> Items { get; set; }
    }
}

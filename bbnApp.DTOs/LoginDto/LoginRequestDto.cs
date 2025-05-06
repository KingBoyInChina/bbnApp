using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.DTOs.LoginDto
{
    /// <summary>
    /// 登录请求信息
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; }
        /// <summary>
        /// CompanyID
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }
        /// <summary>
        /// 二次验证
        /// </summary>
        public string TwoFactorCode { get; set; }
        /// <summary>
        /// 请求来源
        /// </summary>
        public string LoginFrom { get; set; }
        /// <summary>
        /// 请求IP
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public string AreaInfo { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 令牌有效期
        /// </summary>
        public DateTime Expires { get; set; }
    }
    /// <summary>
    /// 登录反馈信息
    /// </summary>
    public class LoginResponseDto
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
        /// 身份信息
        /// </summary>
        public UserInfoDto UserInfo { get; set; }
        /// <summary>
        /// 顶部菜单
        /// </summary>
        public List<TopMenuItemDto> TopMenus { get; set; }
    }
}

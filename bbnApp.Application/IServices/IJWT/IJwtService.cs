using bbnApp.Common.Models;

namespace bbnApp.Application.IServices.IJWT
{
    /// <summary>
    /// JWT服务接口
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// 获取Jwt配置
        /// </summary>
        /// <returns></returns>
        public JwtModel GetJwtModel();
        /// <summary>
        /// token令牌
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JwtToken GetJwtToken(string userid);
    }
}

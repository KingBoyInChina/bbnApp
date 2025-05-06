using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Common.Models
{
    /// <summary>
    /// Jwt配置
    /// </summary>
    public class JwtModel
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 签发者
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 受众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public double expires { get; set; }
    }
    /// <summary>
    /// Jwt令牌
    /// </summary>
    public class JwtToken
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 令牌有效期
        /// </summary>
        public DateTime Expires { get; set; }
    }
}

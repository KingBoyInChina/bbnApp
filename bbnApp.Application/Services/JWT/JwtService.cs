using bbnApp.Application.IServices.IJWT;
using bbnApp.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace bbnApp.Application.Services.JWT
{
    public class JwtService:IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtModel _jwtModel;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtModel = new JwtModel
            {
                SecretKey = _configuration["Jwt:Key"],
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                expires = double.Parse(_configuration["Jwt:ExpiresMinutes"])
            };
        }
        /// <summary>
        /// 获取Jwt配置
        /// </summary>
        /// <returns></returns>
        public JwtModel GetJwtModel()
        {
            return _jwtModel;
        }
        /// <summary>
        /// token令牌
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public JwtToken GetJwtToken(string userid)
        {
            var token = GenerateToken(_jwtModel, userid);
            JwtToken jwttoken = new JwtToken
            {
                Token = token,
                Expires = DateTime.Now.AddMinutes(_jwtModel.expires)
            };
            return jwttoken;
        }

        /// <summary>
        /// jwt 令牌
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static string GenerateToken(JwtModel model, string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(model.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: model.Issuer,
                audience: model.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(model.expires), // 设置过期时间
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

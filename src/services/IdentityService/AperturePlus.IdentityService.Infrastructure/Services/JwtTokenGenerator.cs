using AperturePlus.IdentityService.Api.Options;
using AperturePlus.IdentityService.Application.Interfaces;
using AperturePlus.IdentityService.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IOptionsSnapshot<JwtSettings> optionsSnapshot;

        public JwtTokenGenerator(IOptionsSnapshot<JwtSettings> optionsSnapshot)
        {
            this.optionsSnapshot = optionsSnapshot;
        }

        public string GenerateToken(ApplicationUser user)
        {
            SecurityKey securityKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(optionsSnapshot.Value.Key));//创建密钥

            List<Claim> claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//Token的唯一标识，方便以后管理JWT
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
                //new Claim(ClaimTypes.Role,"admin"),
            };//创建Claims
            
            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(optionsSnapshot.Value.ExpiryMinutes),//令牌过期时间
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
                issuer: optionsSnapshot.Value.Issuer,
                audience: optionsSnapshot.Value.Audience
            );//打包成Token

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
            
    }
}

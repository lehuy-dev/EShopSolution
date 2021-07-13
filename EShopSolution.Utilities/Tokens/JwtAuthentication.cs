using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Utilities.Tokens
{
    public class JwtAuthentication
    {
        public JwtTokenConfig _jwtTokenConfig;
        public JwtAuthentication(JwtTokenConfig jwtTokenConfig)
        {
            _jwtTokenConfig = jwtTokenConfig;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public string BuildToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
            var creds = new SigningCredentials(key: key, algorithm: SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtTokenConfig.Issuer,
                audience: _jwtTokenConfig.Audience,
                notBefore: DateTime.Now,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                signingCredentials: creds
                ) ;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// Build refresh token let create new access token
        /// </summary>
        /// <returns></returns>
        public string BuildRefreshToken()
        {
            var randomNumber = new byte[32];
            using(var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            JwtSecurityTokenHandler tokenValidator = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret));
            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };
            return tokenValidator.ValidateToken(token, parameters, out var securityToken);
            
        }


    }
}

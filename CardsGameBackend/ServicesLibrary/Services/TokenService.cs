using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelsLibrary.Models;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ITokenDAO _tokenDAO;

        public TokenService(IConfiguration configuration, ITokenDAO tokenDAO)
        {
            _configuration = configuration;
            _tokenDAO = tokenDAO;
        }

        // Methods

        // Generate a Jwt Token based on the user 
        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };



            var token = new JwtSecurityToken
                (
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(10),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task SaveRefreshToken(int userId, string refreshToken)
        {
            await _tokenDAO.CreateRefreshToken(userId, refreshToken, DateTime.Now.AddDays(7));
        }


        public async Task<int?> ValidateRefreshToken(string refreshToken)
        {
            var tokenData = await _tokenDAO.GetRefreshToken(refreshToken);
            if (tokenData == null || tokenData.Expiration < DateTime.UtcNow)
            {
                return null;
            }
            return tokenData.UserId;
        }
    }
}

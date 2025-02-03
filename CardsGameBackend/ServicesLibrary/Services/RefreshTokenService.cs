using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.IdentityModel.Tokens;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenDAO _refreshTokenDAO;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public RefreshTokenService(ITokenDAO tokenDAO, ITokenService tokenService, IUserService userService)
        {
            _refreshTokenDAO = tokenDAO;
            _tokenService = tokenService;
            _userService = userService;
        }

        // devuelvo un nuevo token JWT para el usuario a partir del refresh token
        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            // Buscar el token válido
            var existingToken = await _refreshTokenDAO.GetRefreshToken(refreshToken);

            if (existingToken == null)
                throw new SecurityTokenException("Token inválido o expirado");

            // Obtener el usuario
            var user = await _userService.GetById(existingToken.UserId);

            // Generar nuevo JWT
            var newAccessToken = _tokenService.GenerateToken(user);

            return newAccessToken;
        }

        public async Task<string> GenerateRefreshToken(int userId)
        {
            // Generar un token único
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            // Establecer expiración
            var expiration = DateTime.UtcNow.AddDays(7);

            // Guardar el token
            await _refreshTokenDAO.CreateRefreshToken(userId, token, expiration);

            return token;
        }
    }
}

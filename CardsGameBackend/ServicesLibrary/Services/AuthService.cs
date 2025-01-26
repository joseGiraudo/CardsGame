using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Auth;
using ServicesLibrary.Response;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserDAO _userDAO;
        private readonly ITokenService _tokenService;

        public AuthService(IUserDAO userDAO, ITokenService tokenService)
        {
            _userDAO = userDAO;
            _tokenService = tokenService;
        }

        public async Task<string> Authenticate(LoginDTO loginDTO)
        {
            var user = await _userDAO.GetByEmail(loginDTO.Email);

            if (user == null)
            {
                return null; // usuario o contraseña invalidos
            }

            //if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password)) {
            //    return null;
            //}

            var token = _tokenService.GenerateToken(user);
            return token;
        }
    }
}

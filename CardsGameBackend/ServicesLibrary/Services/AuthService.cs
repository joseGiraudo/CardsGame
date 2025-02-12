using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Auth;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Response;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserDAO _userDAO;
        private readonly ITokenService _tokenService;
        private readonly ITokenDAO _tokenDAO;

        public AuthService(IUserDAO userDAO, ITokenService tokenService, ITokenDAO tokenDAO)
        {
            _userDAO = userDAO;
            _tokenService = tokenService;
            _tokenDAO = tokenDAO;
        }

        public async Task<string> Authenticate(LoginDTO loginDTO)
        {
            var user = await _userDAO.GetByEmail(loginDTO.Email);

            if (user == null)
            {
                throw new NotFoundException("Email o Password incorrectos");
            }

            // ver que pasa cuando la password no se guardo hasheada en la BD

            if (!BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                throw new NotFoundException("Email o Password incorrectos");
            }

            var token = _tokenService.GenerateToken(user);
            return token;
        }
    }
}

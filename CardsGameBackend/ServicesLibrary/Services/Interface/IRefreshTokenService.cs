using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.Services.Interface
{
    public interface IRefreshTokenService
    {
        public Task<string> RefreshAccessToken(string refreshToken);
        public Task<string> GenerateRefreshToken(int userId);
    }
}

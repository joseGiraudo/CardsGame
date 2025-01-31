using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface ITokenDAO
    {
        public Task<RefreshToken?> GetRefreshToken(string token);
        public Task SaveRefreshToken(int userId, string refreshToken);
        public Task<int?> ValidateRefreshToken(string refreshToken);
        public Task DeleteRefreshToken(string refreshToken);
    }
}

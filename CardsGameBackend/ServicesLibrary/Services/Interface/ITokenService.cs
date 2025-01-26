using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.DTOs.Auth;
using ServicesLibrary.Response;

namespace ServicesLibrary.Services.Interface
{
    public interface IAuthService
    {
        public Task<string> Authenticate(LoginDTO loginDTO);
    }
}

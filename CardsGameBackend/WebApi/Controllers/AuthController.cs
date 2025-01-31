using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Auth;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.Authenticate(loginDTO);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(new { token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Log Out" });
        }

        //[HttpPost("refresh-token")]
        //public async Task<IActionResult> RefreshTokenAsync()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


        //}
    }
}

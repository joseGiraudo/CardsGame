using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Users;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        // Obtener datos del token
        private (int userId, UserRole role) GetCurrentUserData()
        {
            // Obtener el UserId de los claims de manera segura
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.TryParse(userIdClaim, out int parsedUserId) ? parsedUserId : 0;

            // Obtener el rol de los claims de manera segura
            var roleStr = User.FindFirst(ClaimTypes.Role)?.Value;
            var role = Enum.TryParse<UserRole>(roleStr, out var parsedRole) ? parsedRole : UserRole.Player;

            return (userId, role);
        }



        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllCards(int id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        // dejo eso así junto? o armo otro meteod para registrarse que lo usen unicamente jugadores?

        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> CreateUser(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var (creatorId, creatorRole) = GetCurrentUserData();
                var createdUser = await _userService.CreateUser(userDTO, creatorId, creatorRole);

                return CreatedAtAction("Usuario creado correctamente", createdUser);
            }
            catch (UnauthorizedRoleException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (UserException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
        }

        [HttpPost("/register")]
        [AllowAnonymous] // por los players que pueden autoregistrarse
        public async Task<IActionResult> RegisterPlayer(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var (creatorId, creatorRole) = GetCurrentUserData();
                var createdUser = await _userService.CreateUser(userDTO, creatorId, creatorRole);

                return CreatedAtAction("Usuario creado correctamente", createdUser);
            }
            catch (UnauthorizedRoleException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (UserException ex)
            {
                return StatusCode(ex.StatusCode, new { message = ex.Message });
            }
        }
    }
}

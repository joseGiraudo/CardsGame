using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Users;
using ModelsLibrary.Models;
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

        [HttpPost("players")]
        public async Task<IActionResult> CreatePlayer(UserDTO playerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userCreated = await _userService.CreatePlayer(playerDto);
            return Ok(userCreated);
        }

        // metodo para crear jueces
        [HttpPost("judges")]
        public async Task<IActionResult> CreateUser(UserDTO judgeDTO)
        {
            if (judgeDTO == null)
            {
                return BadRequest("Los campos son incorrectos");
            }

            var judgeCreated = await _userService.CreateJudge(judgeDTO);
            return Ok(judgeCreated);
        }
    }
}

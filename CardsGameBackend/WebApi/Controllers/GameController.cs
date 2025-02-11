using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Models;
using ServicesLibrary.Services;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("games")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAll();

            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var game = await _gameService.GetById(id);

            return Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] Game game)
        {
            var gameId = await _gameService.Create(game);

            return Ok(gameId);
        }


    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Enums;
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


        // ESTE METODO NO DEBE QUEDAR EXPUESTO (eliminar)
        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] Game game)
        {
            var gameId = await _gameService.Create(game);

            return Ok(gameId);
        }

        [Authorize(Roles = nameof(UserRole.Judge))]
        [HttpPost("{id}/finalize")]
        public async Task<IActionResult> FinalizeGame([FromBody] FinalizeGameDTO finalizeGameDTO, int id)
        {
            // Obtener el ID del usuario logueado desde los claims del token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int judgeId))
            {
                return Unauthorized("No se pudo obtener el ID del juez.");
            }

            if (await _gameService.FinalizeGame(id, finalizeGameDTO.WinnerId, judgeId))
                return Ok("Partida oficializada correctamente");

            return BadRequest("No se pudo oficializar la partida");
        }


    }
}

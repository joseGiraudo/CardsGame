﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Tournament;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("tournaments")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IGameService _gameService;

        public TournamentController(ITournamentService tournamentService, IGameService gameService)
        {
            _tournamentService = tournamentService;
            _gameService = gameService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tournaments = await _tournamentService.GetAll();

            return Ok(tournaments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tournament = await _tournamentService.GetById(id);

            return Ok(tournament);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentDTO tournamentDTO)
        {
            var tournament = await _tournamentService.Create(tournamentDTO);

            return Ok(tournament);
        }


        // registro de un player a un torneo
        [AllowAnonymous]
        [HttpPost("{tournamentId}/register/{deckId}")]
        public async Task<IActionResult> TournamentRegistration(int tournamentId, int deckId)
        {
            string userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(); // Retorna 401 si el usuario no está autenticado
            }

            await _tournamentService.RegisterPlayer(tournamentId, userId, deckId);

            return Ok("Registro exitoso");
        }


        [HttpGet("{tournamentId}/games")]
        public async Task<IActionResult> GetGamesByTournamentId(int tournamentId)
        {
            var games = await _gameService.GetByTournamentId(tournamentId);

            return Ok(games);
        }


        [HttpGet("max-players/{tournamentId}")]
        public async Task<IActionResult> GetMaxPlayers(int tournamentId)
        {
            try
            {
                var tournament = await _tournamentService.GetById(tournamentId);
                var maxPlayers = _tournamentService.CalculateMaxPlayersAsync2(tournament.StartDate, tournament.EndDate);
                return Ok(maxPlayers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}

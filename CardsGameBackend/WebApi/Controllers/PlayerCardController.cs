using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Cards;
using ModelsLibrary.DTOs.Users;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
using ServicesLibrary.Exceptions;
using ServicesLibrary.Services;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("decks")]
    public class PlayerCardController : ControllerBase
    {
        private readonly IPlayerCardService _playerCardService;
        public PlayerCardController(IPlayerCardService playerCardService)
        {
            _playerCardService = playerCardService;
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

        [Authorize(Roles = nameof(UserRole.Player))]
        [HttpPost]
        public async Task<IActionResult> CreateDeck([FromBody] CreateDeckDTO createDeckDto)
        {
            if (createDeckDto.Name == null)
            {
                return BadRequest("Los campos son incorrectos");
            }
            try
            {
                var (playerId, playerRole) = GetCurrentUserData();

                if (await _playerCardService.CreateDeck(createDeckDto.Name, playerId))
                {
                    return Created();
                }
                return BadRequest("No se pudo crear el mazo");


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

        [Authorize(Roles = nameof(UserRole.Player))]
        [HttpPost("{deckId}/assign")]
        public async Task<IActionResult> AssignCardToDeck([FromBody] AssignCardDTO cardDTO, int deckId)
        {
            try
            {
                var (playerId, playerRole) = GetCurrentUserData();

                if (await _playerCardService.AssignCardsToDeck(cardDTO.CardIds, deckId))
                {
                    return Ok("Cartas asignadas correctamente");
                }
                return BadRequest("No se pudieron asignar las cartas al mazo");


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

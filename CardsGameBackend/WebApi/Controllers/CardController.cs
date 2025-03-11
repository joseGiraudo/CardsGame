using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;
using ServicesLibrary.Services;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("cards")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;
        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCards()
        {
            var cards = await _cardService.GetAll();
            return Ok(cards);
        }

        [HttpGet("/{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetById(id);
            return Ok(card);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] Card card)
        {
            if (card == null)
            {
                return BadRequest("Los campos son incorrectos");
            }

            var cardCreated = await _cardService.Create(card);
            return Ok(cardCreated);
        }


        // endpoints para crear un mazo

    }
}

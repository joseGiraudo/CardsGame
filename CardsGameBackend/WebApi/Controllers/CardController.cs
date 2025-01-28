using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;
using ServicesLibrary.Services;
using ServicesLibrary.Services.Interface;

namespace WebApi.Controllers
{
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
        public async Task<IActionResult> GetAllCards(int id)
        {
            var card = await _cardService.GetById(id);
            return Ok(card);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(Card card)
        {
            if (card == null)
            {
                return BadRequest("Los campos son incorrectos");
            }

            var cardCreated = await _cardService.Create(card);
            return Ok(cardCreated);
        }
    }
}

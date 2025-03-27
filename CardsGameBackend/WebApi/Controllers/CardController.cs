using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.DTOs.Cards;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCardById(int id)
        {
            var card = await _cardService.GetById(id);
            if (card == null)
            {
                return NotFound(new { message = "Carta no encontrada" });
            }
            return Ok(card);
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPost]
        public async Task<IActionResult> CreateCard([FromBody] CardDTO cardDTO)
        {
            int id = await _cardService.Create(cardDTO);

            return CreatedAtAction(nameof(GetCardById), new { id }, new { id }); // 201 Created
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard([FromBody] CardDTO cardDTO, [FromRoute] int id)
        {
            bool updated = await _cardService.Update(id, cardDTO);

            if (!updated)
            {
                return NotFound(new { message = "Carta no encontrada" });
            }

            return NoContent();
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            bool deleted = await _cardService.DeleteById(id);

            if (!deleted)
            {
                return NotFound(new { message = "Carta no encontrada" });
            }

            return NoContent();
        }
    }
}

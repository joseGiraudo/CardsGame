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
    [Route("series")]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesService _seriesService;
        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        [Authorize(Policy = "AdminOrOrganizer")]
        [HttpPost]
        public async Task<IActionResult> CreateSeries([FromBody] CreateSeriesDTO seriesDTO)
        {
            if(await _seriesService.CreateCardSeries(seriesDTO.Name))
            {
                return CreatedAtAction("Serie creada", seriesDTO.Name);
            }
            return BadRequest("No se pudo crear la serie");
        }


        [Authorize(Policy = "AdminOrOrganizer")]
        [HttpPost("{tournamentId}")]
        public async Task<IActionResult> AssignSeriesToTournament([FromBody] AssignSeriesTournamentDTO assignSeriesDTO, int tournamentId)
        {
            if (await _seriesService.AssignSeriesToTournament(tournamentId, assignSeriesDTO.SeriesIds))
            {
                return Created();
            }
            return BadRequest("No se pudo crear la serie");
        }


        // endpoints para crear un mazo

    }
}

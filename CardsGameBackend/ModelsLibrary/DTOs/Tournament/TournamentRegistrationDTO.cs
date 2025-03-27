using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Tournament
{
    public class TournamentRegistrationDTO
    {
        [Required(ErrorMessage = "Debes seleccionar un mazo")]
        public int DeckId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Tournament
{
    public class DisqualificationDTO
    {

        [Required(ErrorMessage = "Debes seleccionar un jugador a descalificar")]
        public int PlayerId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar el torneo")]
        public int TournamentId { get; set; }

        [Required(ErrorMessage = "La razon de descalificación es obligatoria")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "La razon debe tener entre 3 y 255 caracteres.")]
        public string Reason { get; set; }
    }
}

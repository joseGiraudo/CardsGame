using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Tournament
{
    public class FinalizeGameDTO
    {

        [Required(ErrorMessage = "Debes seleccionar un ganador de la partida")]
        public int WinnerId { get; set; }
    }
}

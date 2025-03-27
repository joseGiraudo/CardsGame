using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Cards
{
    public class CreateSeriesDTO
    {

        [Required(ErrorMessage = "Nombre de la serie obligatorio")]
        public string Name { get; set; }
    }
}

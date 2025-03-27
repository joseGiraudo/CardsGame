using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Cards
{
    public class CreateDeckDTO
    {
        [Required(ErrorMessage = "Nombre del mazo obligatorio.")]
        public string Name { get; set; }
    }
}

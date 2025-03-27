using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Cards
{
    public class CardDTO
    {

        [Required(ErrorMessage = "Nombre de la carta es obligatorio")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El ataque es obligatorio")]
        [Range(0, 100, ErrorMessage = "El valor de ataque debe estar entre 0 y 100")]
        public int Attack { get; set; }

        [Required(ErrorMessage = "La defensa es obligatoria")]
        [Range(0, 100, ErrorMessage = "El valor de defensa debe estar entre 0 y 100")]
        public int Defense { get; set; }


        [Required(ErrorMessage = "La ilustración es obligatoria")]
        public string Illustration { get; set; }
    }
}

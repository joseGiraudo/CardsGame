using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Validators;

namespace ModelsLibrary.DTOs.Tournament
{
    public class CreateTournamentDTO
    {
        [Required(ErrorMessage = "El nombre del torneo es obligatorio.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del torneo debe tener entre 3 y 50 caracteres.")]
        [NoSpecialCharacters]  // Aplicar el validador personalizado
        public string Name { get; set; }

        [Required(ErrorMessage = "Fecha de comienzo obligatoria.")]
        public DateTime LocalStartDate { get; set; }

        [Required(ErrorMessage = "Fecha de fin obligatoria.")]
        public DateTime LocalEndDate { get; set; }


        [Required(ErrorMessage = "Debes seleccionar un pais")]
        public int CountryId { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una zona horaria para le torneo")]
        public string TimeZoneId { get; set; }


        [Required(ErrorMessage = "Debes seleccionar la lista de series habilitadas")]
        public List<int> AvailableSeries {  get; set; }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Tournament
{
    public class CreateTournamentDTO
    {
        public string Name { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int CountryId { get; set; }

        // falta agregar la lista de jueces y series habilitadas
    }
}

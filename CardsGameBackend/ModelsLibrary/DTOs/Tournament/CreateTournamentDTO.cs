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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int OrganizerId { get; set; }
        public int CountryId { get; set; }

        // falta agregar la lista de jueces y series habilitadas
    }
}

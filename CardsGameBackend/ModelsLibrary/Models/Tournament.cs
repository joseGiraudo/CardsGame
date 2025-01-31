using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Enums;

namespace ModelsLibrary.Models
{
    public class Tournament
    {
        public int Id { get; set; } 
        public string Name { get; set; } 
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int? CountryId { get; set; }
        public TournamentPhase Phase { get; set; }
        public int OrganizerId { get; set; }
        public int? WinnerId { get; set; }

    }
}

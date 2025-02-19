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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? CountryId { get; set; }
        public TournamentPhase Phase { get; set; }
        public int OrganizerId { get; set; }
        public int? WinnerId { get; set; }

    }
}

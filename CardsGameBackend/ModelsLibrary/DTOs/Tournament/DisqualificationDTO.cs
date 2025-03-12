using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.DTOs.Tournament
{
    public class DisqualificationDTO
    {
        public int PlayerId { get; set; }
        public int TournamentId { get; set; }
        public string Reason { get; set; }
    }
}

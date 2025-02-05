using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
    public class TournamentPlayer
    {
        public int TournamentId { get; set; }
        public int PlayerId { get; set; }
        public int? DeckId { get; set; }
        public bool IsEliminated { get; set; }
    }
}

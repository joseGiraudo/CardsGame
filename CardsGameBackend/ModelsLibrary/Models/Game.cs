using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsLibrary.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public DateTime? StartDate { get; set; }
        public int Player1 {  get; set; }
        public int? Player2 { get; set; }
        public int? WinnerId { get; set; }
    }
}

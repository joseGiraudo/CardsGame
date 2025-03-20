using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface ITournamentPlayerDAO
    {
        public Task<bool> RegisterPlayerAsync(int tournamentId, int playerId, int deckId);
        public Task<List<int>> GetTournamentPlayersAsync(int tournamentId);
        public Task<List<int>> GetWinnersAsync(int tournamentId);
        public Task<bool> RegisterJudgeAsync(int tournamentId, int judgeId);
        Task<bool> CheckCardsSeries(int deckId, int tournamentId);
        public Task<IEnumerable<Card>> GetInvalidCards(int deckId, int tournamentId);
    }
}

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
        public Task EliminatePlayer(int tournamentId, int playerId);
        public Task<List<TournamentPlayer>> GetTournamentPlayersAsync(int tournamentId);
        public Task<List<TournamentPlayer>> GetRoundWinnersAsync(int tournamentId);
        public Task<bool> RegisterJudgeAsync(int tournamentId, int judgeId);
    }
}

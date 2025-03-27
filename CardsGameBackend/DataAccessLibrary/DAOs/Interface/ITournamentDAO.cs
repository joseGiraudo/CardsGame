using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface ITournamentDAO
    {
        Task<IEnumerable<Tournament>> GetAllAsync();
        Task<Tournament> GetByIdAsync(int id);
        Task<int> CreateAsync(Tournament tournament);
        Task UpdateAsync(Tournament tournament);
        Task<int> DeleteAsync(int id);
        Task<IEnumerable<Tournament>> GetByPhaseAsync(TournamentPhase phase);
        public Task<bool> GetTournamentJudges(int tournamentId);
        Task<bool> DisqualifyPlayer(Disqualification disqualification);
        public Task<bool> CheckRoundFinished(int tournamentId);
        public Task<bool> IsJudgeAuthorizedByGame(int gameId, int judgeId);
        public Task<bool> IsJudgeAuthorizedByTournament(int tournamentId, int judgeId);

    }
}

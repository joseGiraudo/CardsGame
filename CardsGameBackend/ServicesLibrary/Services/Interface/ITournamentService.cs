using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface ITournamentService
    {
        public Task<List<Tournament>> GetAll();
        public Task<Tournament> GetById(int id);
        public Task<Tournament> Create(CreateTournamentDTO tournamentDTO);
        public Task<Tournament> Update(Tournament tournament);
        public Task<string> DeleteById(int id);
        public Task<bool> RegisterPlayer(int tournamentId, int playerId, int deckId);
        public Task<bool> AssignJudgeToTournament(int tournamentId, int judgeId);
        public Task<bool> AssignSeriesToTournament(int tournamentId, int seriesId);
        public Task AdvanceTournamentPhase(int tournamentId);
        public int CalculateMaxPlayers(DateTime start, DateTime end);
    }
}

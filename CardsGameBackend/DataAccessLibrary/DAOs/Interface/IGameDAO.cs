using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface IGameDAO
    {
        public Task<Game> GetById(int id);
        public Task<IEnumerable<Game>> GetAll();
        public Task<IEnumerable<Game>> GetByTournamentId(int tournamentId);
        public Task<bool> SetGameWinner(int gameId, int winnerId);
        public Task<DateTime?> GetLastGameDateAsync(int tournamentId);
        public Task<int> Create(Game game);
        public Task<int> Update(Game game);
        public Task<int> Delete(int id);
    }
}

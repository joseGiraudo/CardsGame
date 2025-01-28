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
        Task<Game> GetById(int id);
        Task<IEnumerable<Game>> GetAll();
        Task<IEnumerable<Game>> GetByTournamentId(int tournamentId);
        
        Task<int> Create(Game game);
        Task<int> Update(Game game);
        Task<int> Delete(int id);
    }
}

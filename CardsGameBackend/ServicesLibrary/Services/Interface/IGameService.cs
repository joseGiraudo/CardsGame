using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface IGameService
    {
        public Task<List<Game>> GetAll();
        public Task<Game> GetById(int id);
        public Task<List<Game>> GetByTournamentId(int tournamentId);
        public Task<Game> Create(Game game);
        public Task<Game> Update(Game Game);
        public Task<string> DeleteById(int id);
    }
}

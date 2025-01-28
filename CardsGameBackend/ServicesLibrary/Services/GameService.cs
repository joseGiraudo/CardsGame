using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Models;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class GameService : IGameService
    {
        private readonly IGameDAO _gameDAO;

        public GameService(IGameDAO gameDAO)
        {
            _gameDAO = gameDAO;
        }


        public async Task<Game> Create(Game game)
        {
            await _gameDAO.Create(game);
            return game;
        }

        public Task<string> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Game>> GetAll()
        {
            var games = (List<Game>) await _gameDAO.GetAll();
            return games;
        }

        public async Task<Game> GetById(int id)
        {
            var game = await _gameDAO.GetById(id);
            return game;
        }

        public async Task<List<Game>> GetByTournamentId(int tournamentId)
        {
            var games = (List<Game>) await _gameDAO.GetByTournamentId(tournamentId);
            return games;
        }

        public Task<Game> Update(Game Game)
        {
            throw new NotImplementedException();
        }
    }
}

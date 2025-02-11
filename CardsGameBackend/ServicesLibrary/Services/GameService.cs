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
        private readonly ITournamentPlayerDAO _tournamentPlayerDAO;

        public GameService(IGameDAO gameDAO, ITournamentPlayerDAO tournamentPlayerDAO)
        {
            _gameDAO = gameDAO;
            _tournamentPlayerDAO = tournamentPlayerDAO;
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

        public async Task FinalizeGame(int gameId, int winnerId)
        {
            Game game = await GetById(gameId);

            if (game.StartDate.AddMinutes(30) < DateTime.Now)
            {
                throw new Exception("El juego no finalizo");
            }

            if(game.WinnerId != null)
            {
                throw new Exception("El juego ya fue oficializado");
            }

            int losserId = 0;

            if (game.Player1Id == winnerId)
            {
                losserId = game.Player2Id;
                game.WinnerId = winnerId;
            } else if (game.Player2Id == winnerId) 
            {
                losserId = game.Player1Id;
                game.WinnerId = winnerId;
            } else
            {
                throw new Exception("El id de ganador no se encentra en esta partida");
            }

            await _gameDAO.Update(game);

            await _tournamentPlayerDAO.EliminatePlayer(game.TournamentId, losserId);

            // que puedo devolver aca??????          

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

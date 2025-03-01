using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Tournament;
using ModelsLibrary.Models;
using ServicesLibrary.Exceptions;
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

        public async Task<bool> FinalizeGame(int gameId, int winnerId, int judgeId)
        {
            // tengo que verificar que el juez pertenezca al torneo?
            // conviene hacerlo directamente en una sentencia SQL?



            Game game = await GetById(gameId);

            if (game.StartDate.AddMinutes(30) < DateTime.Now)
            {
                throw new BadRequestException("El juego no finalizo");
            }

            if(game.WinnerId != null)
            {
                throw new InconsistentException("El juego ya fue oficializado");
            }

            if (game.Player1Id == winnerId)
            {
                game.WinnerId = winnerId;
            } else if (game.Player2Id == winnerId) 
            {
                game.WinnerId = winnerId;
            } else
            {
                throw new InconsistentException("El id de ganador no se encentra en esta partida");
            }

            return await _gameDAO.SetGameWinner(gameId, winnerId);
                       
            // devolver un booleano de que el juego fue oficializado

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

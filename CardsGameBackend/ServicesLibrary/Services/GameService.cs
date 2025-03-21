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
            // El juez puede oficializar el game?
            if (!await _gameDAO.IsJudgeAvailableInTournament(gameId, judgeId))
                throw new Exception("El juez no pertenece a este torneo");


            Game game = await GetById(gameId);

            if(game.WinnerId != null)
                throw new InconsistentException("El juego ya fue oficializado");

            if(game.StartDate < DateTime.UtcNow)
                throw new Exception("El juego no comenzo");

            if (game.Player1 == winnerId)
            {
                game.WinnerId = winnerId;
            } else if (game.Player2 == winnerId) 
            {
                game.WinnerId = winnerId;
            } else
            {
                throw new InconsistentException("El id de ganador no se encentra en esta partida");
            }

            // tengo que verificar si es el ultimo partido de la ronda y crear la siguiente ronda de partidos


            return await _gameDAO.SetGameWinner(gameId, winnerId);

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

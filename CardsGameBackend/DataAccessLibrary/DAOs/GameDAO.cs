using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using MySql.Data.MySqlClient;

namespace DataAccessLibrary.DAOs
{
    public class GameDAO : IGameDAO
    {
        private readonly string _connectionString;

        public GameDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public async Task<int> Create(Game game)
        {
            string query = @"INSERT INTO games (tournamentId, startDate, player1, player2) " +
                " VALUES (@tournamentId, @startDate, @player1, @player2);" +
                " SELECT LAST_INSERT_ID();";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var gameId = await connection.ExecuteScalarAsync<int>(query, new
                {
                    tournamentId = game.TournamentId,
                    startDate = game.StartDate,
                    player1 = game.Player1,
                    player2 = game.Player2
                });

                return gameId;
            }
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Game>> GetAll()
        {
            string query = @"SELECT * FROM games";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var games = await connection.QueryAsync<Game>(query);

                if (games == null || games.Count() == 0)
                {
                    throw new Exception("Juegos no encontrados");
                }
                return games;
            }
        }

        public async Task<Game> GetById(int id)
        {
            string query = @"SELECT * FROM games WHERE id = @id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var game = await connection.QueryFirstOrDefaultAsync<Game>(query, new { id });

                if (game == null)
                {
                    throw new Exception("Juego no encontrado");
                }
                return game;
            }
        }

        public async Task<IEnumerable<Game>> GetByTournamentId(int tournamentId)
        {
            string query = @"SELECT * FROM games WHERE tournamentId = @id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var games = await connection.QueryAsync<Game>(query, new { id = tournamentId });

                if (games == null || games.Count() == 0)
                {
                    throw new Exception("Juegos no encontrados para ese torneo");
                }
                return games;
            }
        }

        public Task<int> Update(Game game)
        {
            throw new NotImplementedException();
        }
    }
}

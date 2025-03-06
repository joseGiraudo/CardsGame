using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
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
                    player1 = game.Player1Id,
                    player2 = game.Player2Id
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

        public async Task<bool> SetGameWinner(int gameId, int winnerId)
        {
            string query = @"UPDATE games SET winnerId = @WinnerId WHERE id = @GameId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        WinnerId = winnerId,
                        GameId  = gameId
                    });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error inesperado: {ex.Message}", ex);
            }
        }

        public async Task<DateTime?> GetLastGameDateAsync(int tournamentId)
        {
            using var connection = new MySqlConnection(_connectionString);
            string query = "SELECT MAX(startDate) FROM games WHERE tournamentId = @TournamentId;";

            var lastDate = await connection.QueryFirstOrDefaultAsync<DateTime?>(query, new { TournamentId = tournamentId });

            if(lastDate == null)
            {
                return null;
            }

            return lastDate;
        }

        public async Task<bool> IsJudgeAvailableInTournament(int gameId, int judgeId)
        {
            string query = @"SELECT EXISTS (
                    SELECT 1
                    FROM tournament_judges tj
                    JOIN games g ON tj.tournamentId = g.tournamentId
                    WHERE g.id = @GameId AND tj.judgeId = @JudgeId
            ) AS isAvailable;";

            using (var connection = new MySqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(query, new { GameId = gameId, JudgeId = judgeId });
            }
        }

        public Task<int> Update(Game game)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using MySqlConnector;

namespace DataAccessLibrary.DAOs
{
    public class TournamentPlayerDAO : ITournamentPlayerDAO
    {
        private readonly string _connectionString;

        public TournamentPlayerDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public async Task<bool> RegisterPlayerAsync(int tournamentId, int playerId, int deckId)
        {
            string query = @"INSERT INTO tournament_players (tournamentId, playerId, deckId) 
                        VALUES (@TournamentId, @PlayerId, @DeckId);"
            ;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        TournamentId = tournamentId,
                        PlayerId = playerId,
                        DeckId = deckId
                    });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                // Verifica si el error es por duplicado (MySQL Error Code 1062)
                if (ex.Number == 1062)
                {
                    throw new DatabaseException("El jugador ya esta registrado.", ex);
                }
                else
                {
                    throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error inesperado: {ex.Message}", ex);
            }
        }

        public async Task EliminatePlayer(int tournamentId, int playerId)
        {
            string query = @"
            UPDATE tournament_players 
            SET isEliminated = TRUE 
            WHERE tournamentId = @TournamentId AND playerId = @PlayerId";

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(query, new
                {
                    TournamentId = tournamentId,
                    PlayerId = playerId
                });
            }
        }


        // falta un metodo para traer a todos los jugadores y sus mazos correspondientes a un torneo
        public async Task<List<TournamentPlayer>> GetTournamentPlayersAsync(int tournamentId)
        {
            string query = @"SELECT * FROM tournament_players WHERE tournamentId = @TournamentId;";

            using (var connection = new MySqlConnection(_connectionString))
            {
                var players = await connection.QueryAsync<TournamentPlayer>(query, new
                {
                    TournamentId = tournamentId
                });
                return players.ToList();
            }

        }

        public async Task<List<TournamentPlayer>> GetRoundWinnersAsync(int tournamentId)
        {
            string query = @"SELECT * FROM tournament_players " +
                            "WHERE tournamentId = @TournamentId AND isEliminated = false;";

            using (var connection = new MySqlConnection(_connectionString))
            {
                var players = await connection.QueryAsync<TournamentPlayer>(query, new
                {
                    TournamentId = tournamentId
                });
                return players.ToList();
            }

        }

        public async Task<bool> RegisterJudgeAsync(int tournamentId, int judgeId)
        {
            string query = @"INSERT INTO tournament_judges (tournamentId, judgeId) 
                        VALUES (@TournamentId, @JudgeId);"
            ;

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        TournamentId = tournamentId,
                        JudgeId = judgeId
                    });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                // Verifica si el error es por duplicado (MySQL Error Code 1062)
                if (ex.Number == 1062)
                {
                    throw new DatabaseException("El jugador ya esta registrado.", ex);
                }
                else
                {
                    throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error inesperado: {ex.Message}", ex);
            }
        }
    }
}

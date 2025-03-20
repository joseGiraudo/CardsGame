using System;
using System.Collections;
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


        // falta un metodo para traer a todos los jugadores y sus mazos correspondientes a un torneo
        public async Task<List<int>> GetTournamentPlayersAsync(int tournamentId)
        {
            string query = @"SELECT playerId FROM tournament_players WHERE tournamentId = @TournamentId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    var players = await connection.QueryAsync<int>(query, new
                    {
                        TournamentId = tournamentId
                    });
                    return players.ToList();
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


        public async Task<List<int>> GetWinnersAsync(int tournamentId)
        {
            string query = @"SELECT DISTINCT u.id
                            FROM users u
                            JOIN games g ON u.id = g.player1 OR (u.id = g.player2 AND g.player2 IS NOT NULL)
                            WHERE g.tournamentId = @TournamentId
                            AND u.id NOT IN (
                                SELECT DISTINCT CASE 
                                    WHEN g.player1 != g.winnerId THEN g.player1
                                    WHEN g.player2 IS NOT NULL AND g.player2 != g.winnerId THEN g.player2
                                END
                                FROM games g
                                WHERE g.tournamentId = @TournamentId
                                AND g.winnerId IS NOT NULL
                                AND (g.player1 != g.winnerId OR (g.player2 IS NOT NULL AND g.player2 != g.winnerId))
                            );";    

            
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var playerIds = await connection.QueryAsync<int>(query, new
                    {
                        TournamentId = tournamentId
                    });
                    return playerIds.ToList();
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
                    throw new DatabaseException("El juez ya esta registrado.", ex);
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

        public async Task<bool> CheckCardsSeries(int deckId, int tournamentId)
        {
            string query = @"
                SELECT 
                    COUNT(dc.cardId) as TotalCards,
                    SUM(CASE WHEN cs.cardId IS NOT NULL THEN 1 ELSE 0 END) as ValidCards
                FROM 
                    decks_cards dc
                    LEFT JOIN (
                        SELECT DISTINCT cs.cardId
                        FROM cards_series cs
                        JOIN tournament_series ts ON cs.seriesId = ts.seriesId
                        WHERE ts.tournamentId = @tournamentId
                    ) cs ON dc.cardId = cs.cardId
                WHERE 
                    dc.deckId = @deckId";

            
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var result = await connection.QuerySingleAsync(query, new { deckId, tournamentId });

                    return result.TotalCards == result.ValidCards;
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


        public async Task<IEnumerable<Card>> GetInvalidCards(int deckId, int tournamentId)
        {
            string query = @"
                SELECT 
                    c.id, 
                    c.name,
                    c.attack,
                    c.defense,
                    c.illustration
                FROM 
                    decks_cards dc
                    JOIN cards c ON dc.cardId = c.id
                    LEFT JOIN (
                        SELECT DISTINCT cs.cardId
                        FROM cards_series cs
                        JOIN tournament_series ts ON cs.seriesId = ts.seriesId
                        WHERE ts.tournamentId = @tournamentId
                    ) available_cards ON dc.cardId = available_cards.cardId
                WHERE 
                    dc.deckId = @deckId
                    AND available_cards.cardId IS NULL";
            
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var parameters = new { deckId, tournamentId };
                    return await connection.QueryAsync<Card>(query, parameters);
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


    }
}

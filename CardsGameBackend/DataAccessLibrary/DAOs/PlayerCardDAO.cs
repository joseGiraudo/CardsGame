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
using MySqlConnector;

namespace DataAccessLibrary.DAOs
{
    public class PlayerCardDAO : IPlayerCardDAO
    {

        private readonly string _connectionString;

        public PlayerCardDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


        public async Task<IEnumerable<Card>> GetPlayerCollection(int playerId)
        {
            string query = @"SELECT c.id, c.name, c.attack, c.defense, c.illustration
                            FROM cards c
                            JOIN collections cl on c.id = cl.cardId
                            WHERE cl.playerId = @PlayerId";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var cards = await connection.QueryAsync<Card>(query, new { PlayerId = playerId});

                    return cards;
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

        public async Task<bool> AssignCardToCollection(int cardId, int playerId)
        {
            string query = @"INSERT INTO collections (playerId, cardId) " +
                " VALUES (@PlayerId, @CardId);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { PlayerId = playerId, CardId = cardId });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                // Verifica si el error es por duplicado (MySQL Error Code 1062)
                if (ex.Number == 1062)
                {
                    throw new DatabaseException("La carta ya se encuentra en la coleccion.", ex);
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

        public async Task<bool> RemoveCardFromCollection(int cardId, int playerId)
        {
            string query = @"DELETE FROM collections WHERE playerId = @PlayerId AND cardId = @CardId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { PlayerId = playerId, CardId = cardId });
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


        public async Task<bool> CheckCardsInCollection(List<int> cardIds, int playerId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = @"SELECT COUNT(*) 
                            FROM collections 
                            WHERE playerId = @PlayerId AND cardId IN @CardIds;";

            int count = await connection.ExecuteScalarAsync<int>(query, new { PlayerId = playerId, CardIds = cardIds });

            return count == cardIds.Count; // Si el número de cartas encontradas es igual al total, todas están en la colección
        }

        public async Task<bool> CreateDeck(string name, int playerId)
        {
            string query = @"INSERT INTO decks (playerId, name) " +
                " VALUES (@PlayerId, @Name);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { PlayerId = playerId, Name = name });
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

        public async Task<bool> UpdateDeck(int deckId, string name, int playerId)
        {
            string query = @"UPDATE decks SET name = @Name WHERE id = @DeckId and playerId = @PlayerId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { Name = name, DeckId = deckId, PlayerId = playerId });
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

        public async Task<bool> AssignCardToDeck(int cardId, int deckId)
        {
            string query = @"INSERT INTO decks_cards (deckId, cardId) VALUES (@DeckId, @CardId);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { DeckId = deckId, CardId = cardId });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                // Verifica si el error es por duplicado (MySQL Error Code 1062)
                if (ex.Number == 1062)
                {
                    throw new DatabaseException("La carta ya se encuentra en el mazo.", ex);
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

        public async Task<bool> AssignCardsToDeck(List<int> cardIds, int deckId)
        {
            if (cardIds == null || !cardIds.Any())
            {
                throw new ArgumentException("La lista de cartas no puede estar vacía.");
            }

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = await connection.BeginTransactionAsync())
                    {
                        try
                        {
                            // primero elimino las cartas que tenga asociadas el mazo
                            string deleteQuery = "DELETE FROM decks_cards WHERE deckId = @DeckId;";
                            await connection.ExecuteAsync(deleteQuery, new { DeckId = deckId }, transaction);


                            // agrego las nuevas cartas al mazo
                            string insertQuery = @"INSERT INTO decks_cards (deckId, cardId) VALUES (@DeckId, @CardId);";

                            int totalInserted = await connection.ExecuteAsync(insertQuery,
                                cardIds.Select(cardId => new { DeckId = deckId, CardId = cardId }),
                                transaction
                            );

                            await transaction.CommitAsync();
                            return totalInserted > 0;
                        }
                        catch (MySqlException ex)
                        {
                            await transaction.RollbackAsync();
                            throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error inesperado: {ex.Message}", ex);
            }
        }

        public async Task<bool> RemoveCardFromDeck(int cardId, int deckId)
        {
            string query = @"DELETE FROM collections WHERE playerId = @PlayerId AND deckId = @DeckId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { DeckId = deckId, CardId = cardId });
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

        public async Task<int> GetDeckCardsQuantity(int deckId)
        {
            string query = @"SELECT COUNT(cardId) FROM decks_cards WHERE deckId = @DeckId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int cardsQuantity = await connection.ExecuteScalarAsync<int>(query, new { DeckId = deckId });
                    return cardsQuantity;
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

        public async Task<IEnumerable<Card>> GetCardsByDeckId(int deckId)
        {
            string query = @"SELECT c.id, c.name, c.attack, c.defense, c.illustration
                            FROM cards c
                            JOIN deck_cards dc on c.id = dc.cardId
                            WHERE dc.deckId = @DeckId";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var cards = await connection.QueryAsync<Card>(query, new { DeckId = deckId });
                    return cards;
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

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
                throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
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
    }
}

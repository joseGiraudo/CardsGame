﻿using System;
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



        // ver si esto los dejo aca o los muevo a otro service

        public async Task<bool> CreateCardSeries(string name)
        {
            string query = @"INSERT INTO series (name, releaseDate) " +
                " VALUES (@Name, @ReleaseDate);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { Name = name, ReleaseDate = DateTime.UtcNow });
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

        public async Task<bool> AssignCardToSeries(int cardId, int seriesId)
        {
            string query = @"INSERT INTO cards_series (cardId, seriesId) VALUES (@CardId, @SeriesId);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { CardId = cardId, SeriesId = seriesId });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                // Verifica si el error es por duplicado (MySQL Error Code 1062)
                if (ex.Number == 1062)
                {
                    throw new DatabaseException("La carta ya se encuentra en la serie.", ex);
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

        public async Task<bool> RemoveCardFromSeries(int cardId, int seriesId)
        {
            string query = @"DELETE FROM cards_series WHERE cardId = @CardId AND seriesId = @SeriesId;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { CardId = cardId, SeriesId = seriesId });
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

        public async Task<bool> ExistsSeries(int seriesId)
        {
            string query = @"SELECT COUNT(1) FROM series WHERE id = @SeriesId);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int count = await connection.QueryFirstOrDefault(query, new { SeriesId = seriesId });

                    return count > 0;
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


        public async Task<bool> AssignSeriesToTournament(int tournamentId, int seriesId)
        {
            string query = @"INSERT INTO tournament_series (tournamentId, seriesId) VALUES (@TournamentId, @SeriesId);";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    int rowsAffected = await connection.ExecuteAsync(query, new { TournamentId = tournamentId, SeriesId = seriesId });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                // Verifica si el error es por duplicado (MySQL Error Code 1062)
                if (ex.Number == 1062)
                {
                    throw new DatabaseException("La carta ya se encuentra en la serie.", ex);
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

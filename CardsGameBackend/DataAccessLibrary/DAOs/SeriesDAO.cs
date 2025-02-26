using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace DataAccessLibrary.DAOs
{
    public class SeriesDAO : ISeriesDAO
    {

        private readonly string _connectionString;

        public SeriesDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


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

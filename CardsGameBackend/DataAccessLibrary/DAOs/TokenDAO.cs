using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using MySqlConnector;

namespace DataAccessLibrary.DAOs
{
    public class TokenDAO : ITokenDAO
    {
        private readonly string _connectionString;

        public TokenDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public async Task<RefreshToken?> GetRefreshToken(string token)
        {
            string query = @"SELECT user_id, token, expiration FROM refresh_tokens WHERE token = @token";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<RefreshToken>(query, new { token = token });
            }
        }

        public async Task DeleteRefreshToken(string refreshToken)
        {
            string query = @"DELETE FROM refresh_tokens WHERE token = @token;";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                await connection.ExecuteAsync(query, new { token = refreshToken });
            }
        }

        public async Task SaveRefreshToken(int userId, string refreshToken)
        {
            string query = @"INSERT INTO refresh_tokens (user_id, token, expiration) VALUES (@userId, @token, @expiration);";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                await connection.ExecuteAsync(query, 
                    new { userId = userId, token = refreshToken, expiration = DateTime.UtcNow.AddDays(7) });
            }

        }

        public async Task<int?> ValidateRefreshToken(string refreshToken)
        {
            string query = @"SELECT user_id, expiration FROM refresh_tokens WHERE token = @token;";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var tokenData = await connection.QueryFirstOrDefaultAsync<RefreshToken>(query, new { token = refreshToken });

                if (tokenData == null || tokenData.Expiration < DateTime.UtcNow)
                {
                    return null;
                }

                return tokenData.UserId;
            }
        }
    }
}

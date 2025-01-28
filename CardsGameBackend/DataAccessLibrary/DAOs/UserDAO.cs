using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using Microsoft.Extensions.Configuration;
using ModelsLibrary.Models;
using MySqlConnector;
using Org.BouncyCastle.Crypto.Generators;

namespace DataAccessLibrary.DAOs
{
    public class UserDAO : IUserDAO
    {

        private readonly string _connectionString;

        public UserDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


        public async Task<int> Create(User user)
        {
            string query = @"INSERT INTO users (name, username, email, password, countryId, avatar, role, createdBy) " +
                            "VALUES(@name, @username, @email, @password, @countryId, @avatar, @role, @createdBy); " +
                            "SELECT LAST_INSERT_ID();";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                return await connection.ExecuteScalarAsync<int>(query, new
                {
                    name = user.Name,
                    username = user.Username,
                    email = user.Email,
                    password = user.Password,
                    countryId = user.CountryId,
                    avatar = user.Avatar,
                    role = user.Role,
                    createdBy = user.CreatedBy
                });
            }
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            string query = "SELECT * FROM users";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var users = await connection.QueryAsync<User>(query);
                if (users == null || users.Count() < 1)
                {
                    throw new Exception("Usuarios no encontrados");
                }

                return users;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            string query = "select * from users " +
                "where email = @email";
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { email = email });
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                return user;
            }
        }

        public async Task<User> GetById(int id)
        {
            string query = @"SELECT * FROM users WHERE id = @id";

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                var user = await connection.QueryFirstOrDefaultAsync<User>(query, new {id = id});
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }

                return user;
            }
        }

        public Task<int> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}

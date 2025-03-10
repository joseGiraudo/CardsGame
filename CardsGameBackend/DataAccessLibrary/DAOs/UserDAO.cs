using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataAccessLibrary.DAOs.Interface;
using DataAccessLibrary.Exceptions;
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
            string query = @"INSERT INTO users (name, username, email, password, countryId, avatar, role, createdBy, createdAt) " +
                            "VALUES(@Name, @Username, @Email, @Password, @CountryId, @Avatar, @Role, @CreatedBy, @CreatedAt); " +
                            "SELECT LAST_INSERT_ID();";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    return await connection.ExecuteScalarAsync<int>(query, new
                    {
                        user.Name,
                        user.Username,
                        user.Email,
                        user.Password,
                        user.CountryId,
                        user.Avatar,
                        Role = user.Role.ToString(),
                        user.CreatedBy,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 1062: // Duplicate entry
                        throw new DatabaseException($"Duplicate entry for email: {user.Email}", ex);
                    case 1452: // Foreign key violation
                        throw new DatabaseException($"Invalid reference: CountryId {user.CountryId} or CreatedBy {user.CreatedBy}", ex);
                    default:
                        throw new DatabaseException($"Database error creating user: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Unexpected error creating user", ex);
            }

        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            string query = "SELECT * FROM users";

            
            try
            {
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
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error de base de datos: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException($"Error inesperado: {ex.Message}", ex);
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            string query = "select * from users " +
                "where email = @email";
            
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { email = email });

                    return user;
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

        public async Task<User> GetById(int id)
        {
            string query = @"SELECT * FROM users WHERE id = @id";

            
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { id = id });
                    if (user == null)
                    {
                        throw new Exception("Usuario no encontrado");
                    }

                    return user;
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

        public Task<int> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}

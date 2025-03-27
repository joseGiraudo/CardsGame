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
                throw new DatabaseException($"Error de Base de datos Creando el usuario: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado creando el usuario", ex);
            }

        }

        public async Task<bool> Delete(int id)
        {
            string query = "DELETE FROM users WHERE id = @id;";
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    int rowsAffected = await connection.ExecuteAsync(query, new { id });
                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error al eliminar el usuario: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al eliminar el usuario", ex);
            }
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

        public async Task<User> GetByUsername(string username)
        {
            string query = "select * from users " +
                "where username = @Username";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { Username = username });

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

        public async Task<bool> Update(User user)
        {
            string query = @"UPDATE users 
                     SET name = @Name, 
                         username = @Username, 
                         email = @Email, 
                         password = @Password, 
                         countryId = @CountryId, 
                         avatar = @Avatar, 
                         role = @Role
                     WHERE id = @Id;";

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    int rowsAffected = await connection.ExecuteAsync(query, new
                    {
                        user.Name,
                        user.Username,
                        user.Email,
                        user.Password,
                        user.CountryId,
                        user.Avatar,
                        Role = user.Role.ToString(),
                        user.Id
                    });

                    return rowsAffected > 0;
                }
            }
            catch (MySqlException ex)
            {
                throw new DatabaseException($"Error de Base de datos actualizando el usuario: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado actualizando el usuario", ex);
            }
        }
    }
}

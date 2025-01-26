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

namespace DataAccessLibrary.DAOs
{
    public class UserDAO : IUserDAO
    {

        private readonly string _connectionString;

        public UserDAO(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }


        public Task<int> Create(User user)
        {
            throw new NotImplementedException();
        }

        public Task<int> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAll()
        {
            throw new NotImplementedException();
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

        public Task<User> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}

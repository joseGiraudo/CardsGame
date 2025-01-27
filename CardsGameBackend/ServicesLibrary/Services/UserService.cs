using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.Models;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class UserService : IUserService
    {
        private readonly IUserDAO _userDAO;

        public UserService(IUserDAO userDAO)
        {
            _userDAO = userDAO;
        }


        public async Task<User> Create(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            
            if(await _userDAO.Create(user) > 0)
            {
                return user;
            }

            return null;
        }

        public Task<string> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAll()
        {
            try
            {
                var users = await _userDAO.GetAll();

                return (List<User>)users;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<User> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}

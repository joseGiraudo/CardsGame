using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.DTOs.Users;
using ModelsLibrary.Enums;
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


        public async Task<User> CreatePlayer(PlayerDTO playerDTO)
        {

            // primero verificar que no exista un user con ese email
            //var user = await GetByEmail(playerDTO.Email);
            //if(user != null)
            //{
            //    throw new Exception("El email ya se encuentra registrado");
            //}


            User player = new User()
            {
                Name = playerDTO.Name,
                Email = playerDTO.Email,
                Username = playerDTO.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(playerDTO.Password),
                Avatar = playerDTO.Avatar,
                Role = UserRole.Player,
            };
            
            if(await _userDAO.Create(player) > 0)
            {
                return player;
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

        public async Task<User> GetById(int id)
        {
            try
            {
                var user = await _userDAO.GetById(id);

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            try
            {
                var user = await _userDAO.GetByEmail(email);

                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<User> Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}

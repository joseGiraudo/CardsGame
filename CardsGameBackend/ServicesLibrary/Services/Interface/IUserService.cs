using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.DTOs.Users;
using ModelsLibrary.Enums;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface IUserService
    {
        public Task<List<User>> GetAll();
        public Task<User> GetById(int id, int userId, UserRole userRole);
        public Task<User> GetById(int id);
        public Task<User> GetByEmail(string email);
        public Task<User> CreateUser(UserDTO userDTO, int? creatorId, UserRole? creatorRole);
        public Task<User> Update(User user);
        public Task<string> DeleteById(int id);

    }
}

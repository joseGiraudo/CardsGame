using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface IUserDAO
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        public Task<User> GetByUsername(string username);
        Task<int> Create(User user);
        Task<bool> Update(User user);
        Task<bool> Delete(int id);
    }
}

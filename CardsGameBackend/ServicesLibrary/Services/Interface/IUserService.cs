using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface IUserService
    {
        public Task<List<User>> GetAll();
        public Task<User> GetById(int id);
        public Task<User> Create(User user);
        public Task<User> Update(User user);
        public Task<string> DeleteById(int id);

    }
}

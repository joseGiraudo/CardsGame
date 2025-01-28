using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface ITournamentDAO
    {
        Task<IEnumerable<Tournament>> GetAll();
        Task<Tournament> GetById(int id);
        Task<int> Create(Tournament tournament);
        Task<int> Update(Tournament tournament);
        Task<int> Delete(int id);
    }
}

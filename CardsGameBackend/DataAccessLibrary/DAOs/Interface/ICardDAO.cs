using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface ICardDAO
    {
        Task<IEnumerable<Card>> GetAll();
        Task<Card> GetById(int id);
        Task<int> Create(Card card);
        Task<bool> Update(Card card);
        Task<bool> Delete(int id);
    }
}

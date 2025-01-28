using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface ICardService
    {
        public Task<List<Card>> GetAll();
        public Task<Card> GetById(int id);
        public Task<Card> Create(Card card);
        public Task<Card> Update(Card card);
        public Task<string> DeleteById(int id);
    }
}

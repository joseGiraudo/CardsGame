using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.DTOs.Cards;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface ICardService
    {
        public Task<List<Card>> GetAll();
        public Task<Card> GetById(int id);
        public Task<int> Create(CardDTO cardDTO);
        public Task<bool> Update(int cardId, CardDTO cardDTO);
        public Task<bool> DeleteById(int id);
    }
}

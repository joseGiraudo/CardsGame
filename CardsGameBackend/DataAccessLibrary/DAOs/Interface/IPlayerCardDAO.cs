using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.Exceptions;
using ModelsLibrary.Models;

namespace DataAccessLibrary.DAOs.Interface
{
    public interface IPlayerCardDAO
    {
        public Task<IEnumerable<Card>> GetPlayerCollection(int playerId);
        public Task<bool> AssignCardToCollection(int cardId, int playerId);
        public Task<bool> RemoveCardFromCollection(int cardId, int playerId);
        public Task<bool> CreateDeck(string name, int playerId);
        public Task<bool> UpdateDeck(int deckId, string name, int playerId);
        public Task<bool> AssignCardToDeck(int cardId, int deckId);
        public Task<bool> RemoveCardFromDeck(int cardId, int deckId);
        public Task<int> GetDeckCardsQuantity(int deckId);
        public Task<IEnumerable<Card>> GetCardsByDeckId(int deckId);
    }
}

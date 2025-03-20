using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelsLibrary.Models;

namespace ServicesLibrary.Services.Interface
{
    public interface IPlayerCardService
    {
        public Task<List<Card>> GetPlayerCollection(int playerId);
        public Task<bool> AssignCardToCollection(int cardId, int playerId);
        public Task<bool> RemoveCardFromCollection(int cardId, int playerId);
        public Task<bool> CreateDeck(string name, int playerId);
        public Task<bool> UpdateDeck(int deckId, string name, int playerId);
        public Task<bool> AssignCardsToDeck(List<int> cardIds, int deckId);
        public Task<bool> RemoveCardFromDeck(int cardId, int deckId);
        public Task<int> GetDeckCardsQuantity(int deckId);
        public Task<List<int>> GetCardsByDeckId(int deckId);

    }
}

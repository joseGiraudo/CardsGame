using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLibrary.DAOs.Interface;
using ModelsLibrary.Models;
using ServicesLibrary.Services.Interface;

namespace ServicesLibrary.Services
{
    public class PlayerCardService : IPlayerCardService
    {
        private readonly IPlayerCardDAO _playerCardDAO;

        public PlayerCardService(IPlayerCardDAO playerCardDAO)
        {
            _playerCardDAO = playerCardDAO;
        }

        public async Task<bool> AssignCardToCollection(int cardId, int playerId)
        {
            // tengo que verificar que la carta exista?

            if (await _playerCardDAO.AssignCardToCollection(cardId, playerId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveCardFromCollection(int cardId, int playerId)
        {
            if (await _playerCardDAO.RemoveCardFromCollection(cardId, playerId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> AssignCardsToDeck(List<int> cardIds, int deckId)
        {
            if(cardIds.Count > 15)
                throw new Exception("El mazo solo puede contener 15 cartas.");

            if (await _playerCardDAO.GetDeckCardsQuantity(deckId) >= 15)
                throw new Exception("No se pueden agregar mas cartas al mazo.");

            foreach (var cardId in cardIds)
            {
                await _playerCardDAO.AssignCardToDeck(cardId, deckId);
            }
            return true;
        }

        public async Task<bool> RemoveCardFromDeck(int cardId, int deckId)
        {
            if (await _playerCardDAO.RemoveCardFromDeck(cardId, deckId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> CreateDeck(string name, int playerId)
        {
            if (await _playerCardDAO.CreateDeck(name, playerId))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateDeck(int deckId, string name, int playerId)
        {
            if (await _playerCardDAO.UpdateDeck(deckId, name, playerId))
            {
                return true;
            }
            return false;
        }

        public async Task<int> GetDeckCardsQuantity(int deckId)
        {
            return await _playerCardDAO.GetDeckCardsQuantity(deckId);
        }

        public async Task<List<int>> GetCardsByDeckId(int deckId)
        {
            throw new NotImplementedException();
        }
    }
}

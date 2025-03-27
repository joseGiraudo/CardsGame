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

        public async Task<List<Card>> GetPlayerCollection(int playerId)
        {
            if (playerId <= 0)
            {
                throw new ArgumentException("El ID del jugador debe ser mayor a 0.", nameof(playerId));
            }

            var cards = await _playerCardDAO.GetPlayerCollection(playerId);

            return cards.ToList();
        }

        public async Task<bool> AssignCardToCollection(int cardId, int playerId)
        {

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

        public async Task<bool> AssignCardsToDeck(List<int> cardIds, int deckId, int playerId)
        {

            if(cardIds.Count != 15)
                throw new Exception("El mazo solo puede contener 15 cartas.");

            // chequeo que pertenezcan a la coleccion del jugador
            if (!await _playerCardDAO.CheckCardsInCollection(cardIds, playerId))
            {
                throw new Exception("Una o más cartas no pertenecen a tu colección. No se puede crear el mazo");
            }

            return await _playerCardDAO.AssignCardsToDeck(cardIds, deckId);
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

        public async Task<List<Card>> GetCardsByDeckId(int deckId)
        {
            if (deckId <= 0)
            {
                throw new ArgumentException("El ID del mazo debe ser mayor a 0.", nameof(deckId));
            }

            var cards = await _playerCardDAO.GetCardsByDeckId(deckId);

            return cards.ToList();
        }
    }
}
